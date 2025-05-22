using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using DG.Tweening;
using TMPro;
namespace HappyHouse.HouseSystem
{
    public class HouseManager : MonoBehaviour
    {
        //public HouseBlueprint houseBlueprint;
        public float burnedPercent;
        //public Transform camTransform;
        public HouseGraph houseGraph;
        public FF_Inventory inventory;
        // player budget
        public FF_BudgetManager budgetManager;
        public int initBudget;
        public string playerTag;
        public Vector3 positionOffset;
        public float scaleMultiplier;
        public GameObject /*craftIcon, */arrowUI, nameText;
        // chance for upgrading at the start of the game
        [SerializeField] private float brickWallChance, stuccoWallChance, compositeRoofChance, otherPartUpgradeChance;
        private int upgradeCount = 0;
        private List<PurchaseFloatingButton> purchaseFloatingButtons = new ();
        [SerializeField] private List<BaseHousePartObject> fences;
        //[SerializeField] BoxCollider clickBox;
        [SerializeField] private List<HousePartType> upgradeList = new () { HousePartType.Wall, HousePartType.Roof, HousePartType.Gutter, HousePartType.Vent, HousePartType.Drain, HousePartType.Window, HousePartType.Door };
        public int totalPartsCount, burnedPartsCount = 0;
        public List<FF_Plants> ownedPlants;
        public bool isMoving = false;
        public Dictionary<HousePartType, MaterialClass> upgradeClassDictionary = new ();
        public float burnedWeight, totalWeight = 0f;
        public bool hasMadeDecisions;

        [SerializeField]AudioSource audioSource;
        private void Start()
        {
            houseGraph = new HouseGraph();
            budgetManager = new FF_BudgetManager(this, initBudget);
            audioSource = GetComponent<AudioSource>();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                InitHouseManager();
            });

        }

        public void InitHouseManager()
        {
            houseGraph.nodes.Clear();
            fences = HH_GameManager.Instance.publicFences;
            Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
            nodeDictionary.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).TryGetComponent<BaseHousePartObject>(out var part)){
                    if (part.notInteractable) continue;
                    if (HH_GameManager.Instance.isTutorial)
                    {
                        part.isClickable = false;
                    }
                    InitHouseNode(nodeDictionary, part);
                    
                }
            }


            var name = playerTag == "P1" ? "Player 1" : "Player 2";
            nameText.GetComponent<TextMeshPro>().text = name;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<BaseHousePartObject>(out var part))
                {
                    if (part.notInteractable) continue;
                    if (nodeDictionary.TryGetValue(part.name, out HouseNode currentNode))
                    {

                        foreach (var neighbour in part.CheckNeighbours("Structure"))
                        {
                            if (nodeDictionary.TryGetValue(neighbour.name, out HouseNode connectedNode))
                            {
                                houseGraph.ConnectParts(currentNode, connectedNode);
                            }
                        }
                    }
                }
            }
            // don't roll start condition when it is tutorial or first round
            if (!HH_GameManager.Instance.isTutorial && /*!HH_GameManager.Instance.IsFirstRound &&*/ HH_GameManager.Instance.isNewLevel || isMoving)
            {
                StartCoroutine(RandomizeStartingCondition());
            }
            isMoving = false;
            HH_GameManager.Instance.inputManager.OnHouseSelected -= OnHouseSelected;
            HH_GameManager.Instance.inputManager.OnHouseSelected += OnHouseSelected;
            //houseGraph.PrintGraph();
            ToggleHousePartClickable(false);
        }
        private void InitHouseNode(Dictionary<string, HouseNode> nodeDictionary, BaseHousePartObject part)
        {
            part.InitHousePartObject(this);
            //part.InitHousePartObject(this, allInfos[index]);
            var node = houseGraph.AddHousePart(part);
            part.houseNode = node;
            nodeDictionary[part.name] = node;
            inventory.AddNewPartToInventory(part.partInfo);
            if (!part.partInfo.isPublic)
            {
                AddMaterialToDictionary(part.HousePartType, part.partInfo.materialClass);
            }
            //inventory.AddNewPartToInventory(allInfos[index]);
        }

        public void Repair(Dictionary<HousePartType,MaterialClass> upgradeDict, bool isMoving)
        {
            hasMadeDecisions = true;
            if (!isMoving)
            {
                upgradeClassDictionary = upgradeDict;
                foreach (var pair in upgradeDict)
                {
                    var oldParts = GetAllHousePartObjectsOf(pair.Key);
                    var newPart = ResourceManager.Instance.allAvailableParts[pair.Key].Find(x => x.materialClass == pair.Value);
                    foreach (var oldPart in oldParts)
                    {
                        //inventory.RemovePartFromInventory(oldPart.defaultPartInfo);
                        //if (oldPart.houseNode != null)
                        //{
                        //    houseGraph.RemoveHousePart(oldPart.houseNode);
                        //}
                        oldPart.InitHousePartObject(this, newPart);
                    }
                    inventory.AddNewPartToInventory(newPart);
                }
            }
            burnedWeight = 0f;
            ToggleHousePartClickable(true);
            StartCoroutine(UpdateHouseUI());

        }

        public void ToggleClickBox(bool toggle)
        {
            //clickBox.enabled = toggle;
        }
        public void OnHouseSelected(HouseManager manager)
        {
            if (manager != this) return;

            HH_GameManager.Instance.StartRound(manager);
            HH_GameManager.Instance.uiManager.ToggleEarnMoreMoneyButton(budgetManager.canEarnMoreMoney);
            //ToggleClickBox(false);
            arrowUI.SetActive(true);
            nameText.SetActive(false);
            ToggleHousePartClickable(true);
            //UpdateHouseUI();
            StartCoroutine(UpdateHouseUI());
        }

        public void OnHouseDeselected()
        {
            //HH_GameManager.Instance.inputManager.canClickHouse = true;
            foreach (var icon in purchaseFloatingButtons)
            {
                Destroy(icon.gameObject);
            }
            //ToggleClickBox(true);
            ToggleHousePartClickable(false);
            purchaseFloatingButtons.Clear();
            arrowUI.SetActive(false);

        }

        public void CalculateTotalHousePartWeight()
        {
            if (burnedWeight != 0) return;
            totalWeight = 0;
            foreach(var node in houseGraph.nodes)
            {
                if(node != null)
                {
                    totalWeight += node.housePart.HousePartType.GetHousePartWeight();
                }
            }
        }
        public bool PurchaseHousePart(HousePartInfo partInfo)
        {
            if (!budgetManager.SpendBudget(partInfo.price)) { return false; }
            // add to inventory
            inventory.AddNewPartToInventory(partInfo);

            //HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(partInfo.housePartType);
            return true;
        }

        public void ToggleAllPurchaseIcons(bool state)
        {
            if (purchaseFloatingButtons.Count == 0) return;
            foreach (var icon in purchaseFloatingButtons)
            {
                icon.gameObject.SetActive(state);
            }
        }

        public bool PartIsInUse(HousePartInfo partInfo)
        {
            foreach (var node in houseGraph.nodes)
            {
                if (node.housePart.partInfo.partID == partInfo.partID && node.housePart.HousePartType == partInfo.housePartType)
                {
                    //Debug.Log($"{partInfo.partID} is in use by {node.housePart.name}");
                    return true;
                }
            }
            return false;
        }

        public void ReplaceHousePartObject(/*BaseHousePartObject newPart*/ HousePartInfo housePartInfo)
        {
            //  hide bubble when no purchasing possible
            int ownedPartsCount = -1;

            var constructSfx = ResourceManager.Instance.RetrunRandomConstructSound();
            audioSource.PlayOneShot(constructSfx);
            if (housePartInfo.isPublic)
            {
                ownedPartsCount = inventory.ownedPublicParts[housePartInfo.housePartType].Count;
            }
            else
            {
                ownedPartsCount = inventory.ownedParts[housePartInfo.housePartType].Count;
            }

            bool shouldHideBubble = ResourceManager.Instance.allAvailableParts[housePartInfo.housePartType].Count == ownedPartsCount;

            var oldParts = GetAllHousePartObjectsOf(housePartInfo.housePartType, housePartInfo.isPublic);
            //var oldParts = GetAllHousePartObjects(newPart.HousePartType);
            //Debug.Log($"{oldParts.Count} pieces of {housePartInfo.housePartType} is in use");
            if (!housePartInfo.isPublic)
            {
                AddMaterialToDictionary(housePartInfo.housePartType, housePartInfo.materialClass);
            }
            
            foreach (var oldPart in oldParts)
            {

                if (oldPart.houseNode != null)
                {
                    if (oldPart.shouldDisplayBubble)
                    {
                        oldPart.bubble.isActive = !shouldHideBubble;
                        oldPart.bubble.gameObject.SetActive(!shouldHideBubble);
                        oldPart.shouldDisplayBubble = !shouldHideBubble;
                    }

                    var oldNeighbors = new List<HouseNode>(oldPart.houseNode.neighbourNodes);
                    houseGraph.RemoveHousePart(oldPart.houseNode);  // Remove old node

                    oldPart.InitHousePartObject(this, housePartInfo);
                    var newNode = houseGraph.AddHousePart(oldPart);
                    oldPart.houseNode = newNode;

                    // Reconnect previous neighbors
                    foreach (var neighbor in oldNeighbors)
                    {
                        houseGraph.ConnectParts(newNode, neighbor);
                    }
                }
                else if (housePartInfo.isPublic)
                {
                    if (oldPart.shouldDisplayBubble)
                    {
                        oldPart.bubble.gameObject.SetActive(!shouldHideBubble);
                        oldPart.shouldDisplayBubble = !shouldHideBubble;
                    }
                    oldPart.InitHousePartObject(this, housePartInfo);
                }
            }

            if (HH_GameManager.Instance.isTutorial) return;
            //HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(newPart.HousePartType);
            
            HH_GameManager.Instance.uiManager.inventoryPanel.UpdateInventoryUI(housePartInfo.housePartType, housePartInfo.isPublic);
        }

        public BaseHousePartObject GetCurrentInUseHousePartObjectOf(HousePartType type)
        {
            foreach (var node in houseGraph.nodes)
            {
                if (node.housePart.HousePartType == type)
                {
                    return node.housePart;
                }
            }
            return null;
        }

        public List<BaseHousePartObject> GetAllHousePartObjectsOf(HousePartType type, bool isPublic = false)
        {
            var res = new List<BaseHousePartObject>();

            //Debug.Log($"Replace {type}");
            if (isPublic)
            {
                foreach (var fence in fences)
                {
                    if (fence.partInfo.housePartType == type)
                    {
                        res.Add(fence);
                    }
                }
                return res;
            }

            foreach (var node in houseGraph.nodes)
            {
                if (node.housePart.HousePartType == type)
                {
                    res.Add(node.housePart);
                }
            }

            return res;
        }


        IEnumerator UpdateHouseUI()
        {
            yield return new WaitForSeconds(1f);
            //HashSet<HousePartType> displayedPartTypes = new HashSet<HousePartType>();

            foreach (var node in houseGraph.nodes)
            {
                //Debug.Log($"Init Bubble for {node.housePart}");
                if (!node.housePart.shouldDisplayBubble) continue;
                node.housePart.bubble = InitBubble(node.housePart);

            }

            foreach (var fence in fences)
            {
                //Debug.Log("Init Bubble for Fences");
                if (!fence.shouldDisplayBubble || fence == null) continue;
                fence.bubble = InitBubble(fence);
            }
        }

        private PurchaseFloatingButton InitBubble(BaseHousePartObject part)
        {
            var icon = HH_GameManager.Instance.uiManager.SpawnBubble();
            purchaseFloatingButtons.Add(icon);

            icon.InitBubbleForHousePart(part);
            return icon;
        }
        IEnumerator RandomizeStartingCondition()
        {
            yield return new WaitForSeconds(0.1f);
            RandomizeHouse();
        }
        public void RandomizeHouse()
        {
            while (upgradeCount < 3 && upgradeList.Count > 0)
            {
                System.Random rand = new System.Random();
                var randomTypes = upgradeList.OrderBy(x => rand.Next()).Take(1).ToList();
                foreach (var type in randomTypes)
                {
                    upgradeList.Remove(type);
                    var oldParts = GetAllHousePartObjectsOf(type);
                    HousePartInfo res = null;
                    switch (type)
                    {
                        case HousePartType.Wall:
                            var wallMaterial = RandomizeWall();
                            if (wallMaterial != null)
                            {
                                //Debug.Log($"Randomized Wall: {wallMaterial.name}");
                                res = wallMaterial;
                            }
                            break;
                        case HousePartType.Roof:
                            var roofMaterial = RandomizeRoof();
                            if (roofMaterial != null)
                            {
                                //Debug.Log($"Randomized Roof: {roofMaterial.name}");
                                res = roofMaterial;
                            }
                            break;
                        default:
                            var otherMaterial = RandomizeOtherParts(type);
                            if (otherMaterial != null)
                            {
                                //Debug.Log($"Randomized {type}: {otherMaterial.name}");
                                res = otherMaterial;
                            }
                            break;
                    }


                    if (res != null)
                    {


                        foreach (var oldPart in oldParts)
                        {
                            inventory.RemovePartFromInventory(oldPart.defaultPartInfo.housePartType,oldPart.defaultPartInfo.partID);
                            //if (oldPart.houseNode != null)
                            //{
                            //    houseGraph.RemoveHousePart(oldPart.houseNode);
                            //}
                            oldPart.InitHousePartObject(this, res);
                        }
                        inventory.AddNewPartToInventory(res);
                        AddMaterialToDictionary(res.housePartType, res.materialClass);
                    }


                }


            }
            HousePartInfo RandomizeWall()
            {
                HousePartInfo material = null;
                var anotherHouse = playerTag == "P1" ? HH_GameManager.Instance.p2 : HH_GameManager.Instance.p1;
                var rng = UnityEngine.Random.value;
                var anotherHouseWall = anotherHouse.GetCurrentInUseHousePartObjectOf(HousePartType.Wall).partInfo;
                if (rng < brickWallChance / 10f && anotherHouseWall.materialClass != MaterialClass.B)
                {
                    // brick
                    upgradeCount++;

                    material = ResourceManager.Instance.allAvailableParts[HousePartType.Wall].Find(x => x.materialClass == MaterialClass.B);
                    return material;
                }
                if (rng < stuccoWallChance / 10f)
                {
                    //stucco
                    upgradeCount++;

                    material = ResourceManager.Instance.allAvailableParts[HousePartType.Wall].Find(x => x.materialClass == MaterialClass.C);
                    return material;
                }
                return material;

            }

            HousePartInfo RandomizeRoof()
            {

                HousePartInfo material = null;
                var rng = UnityEngine.Random.value;
                if (rng < compositeRoofChance / 10f)
                {
                    //composite
                    upgradeCount++;
                    material = ResourceManager.Instance.allAvailableParts[HousePartType.Roof].Find(x => x.materialClass == MaterialClass.C);

                }

                return material;
            }

            HousePartInfo RandomizeOtherParts(HousePartType type)
            {

                HousePartInfo material = null;
                var rng = UnityEngine.Random.value;
                if (rng < otherPartUpgradeChance / 10f)
                {
                    upgradeCount++;
                    material = ResourceManager.Instance.allAvailableParts[type].Find(x => x.materialClass == MaterialClass.A);
                }

                return material;
            }
        }

        public void ToggleHousePartClickable(bool state)
        {
            foreach (var node in houseGraph.nodes)
            {
                var part = node.housePart;
                part.isClickable = state;
            }
        }

        public int GetBurnedPercent()
        {
            if (totalWeight <= 0f)
                return 0;

            float rawPercent = (burnedWeight / totalWeight) * 100f;
            return Mathf.CeilToInt(rawPercent);
        }

        public float CalculateRating()
        {
            float totalWeightedScore = 0f;
            float totalWeight = 0f;
            foreach (var pair in upgradeClassDictionary)
            {
                //Debug.Log($"{pair.Key}'s mateiral is {pair.Value}");
                float materialScore = pair.Value.GetMaterialScore();
                totalWeightedScore += materialScore * pair.Key.GetHousePartWeight();
                totalWeight += pair.Key.GetHousePartWeight();
            }

            // no plants should get full score
            if (ownedPlants.Count == 0)
            {
                totalWeightedScore += 10 * 3 * 0.1f;
                totalWeight += 0.3f;
            }
            else
            {
                foreach (var plant in ownedPlants)
                {
                    totalWeightedScore += plant.combustibleInfo.materialClass.GetMaterialScore() * 0.1f;
                    totalWeight += 0.1f;
                }
            }
            var res = totalWeight > 0 ? totalWeightedScore / totalWeight : 0;
            Debug.Log($" {playerTag}'s Rating: {res}");
            return totalWeight > 0 ? totalWeightedScore / totalWeight : 0;
        }

        public void AddMaterialToDictionary(HousePartType type, MaterialClass materialClass)
        {
            if (upgradeClassDictionary.ContainsKey(type))
            {
                upgradeClassDictionary[type] = materialClass;
            }
            else
            {
                upgradeClassDictionary.Add(type, materialClass);
            }
            //Debug.Log($"Added {type} with class {materialClass.GetMaterialScore()} to dictionary");
        }

        //public void OnHousePartDestroyed(BaseHousePartObject part)
        //{
        //    if (part == null) return;
        //    if (part.partInfo.isPublic)
        //    {
        //        HH_GameManager.Instance.publicFences.Remove(part);
        //        return;
        //    }
        //    houseGraph.RemoveHousePart(part.houseNode);

        //}

        private void OnDestroy()
        {
            foreach(var icon in purchaseFloatingButtons)
            {
                if(icon == null) continue;  
                Destroy(icon.gameObject);
            }
            
        }
    }
}

