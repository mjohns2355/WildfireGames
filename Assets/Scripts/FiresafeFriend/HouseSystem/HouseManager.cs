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
        public float burnedPercent;
        public HouseGraph houseGraph;
        public FF_Inventory inventory;
        // player budget
        public FF_BudgetManager budgetManager;
        public int initBudget;
        public string playerTag;
        public string player1NameKey = "player1Text";
        public string player2NameKey = "player2Text";
        public Vector3 positionOffset;
        public float scaleMultiplier;
        public GameObject /*craftIcon, */arrowUI, nameText;
        // chance for upgrading at the start of the game
        [SerializeField] private float brickWallChance, stuccoWallChance, compositeRoofChance, otherPartUpgradeChance;
        private int upgradeCount = 0;
        private List<PurchaseFloatingButton> purchaseFloatingButtons = new ();
        public List<BaseHousePartObject> fences;
        //[SerializeField] BoxCollider clickBox;
        [SerializeField] private List<HousePartType> upgradeList = new () { HousePartType.Wall, HousePartType.Roof, HousePartType.Gutter, HousePartType.Vent, HousePartType.Drain, HousePartType.Window, HousePartType.Door };
        public int totalPartsCount, burnedPartsCount = 0;
        public HashSet<FF_Plants> ownedPlants = new();
        //public List<FF_Plants> deadBushes = new();
        //public List<FF_Props> props = new();
        public bool isMoving = false;
        public Dictionary<HousePartType, MaterialClass> upgradeClassDictionary = new ();
        public float burnedWeight, totalWeight = 0f;
        public bool hasMadeDecisions,hasJoinedCouncil;
        BoxCollider clickBox;
        float flammabilityMod, durabilityMod;
        [SerializeField]AudioSource audioSource;
        private IEnumerator Start()
        {
            houseGraph = new HouseGraph();
            budgetManager = new FF_BudgetManager(this, initBudget);
            clickBox = GetComponent<BoxCollider>();
            audioSource = GetComponent<AudioSource>();

            while (StringManager.Instance == null || !StringManager.Instance.IsReady) yield return null;
            while (ResourceManager.Instance == null || ResourceManager.Instance.allAvailableParts == null) yield return null;
            yield return new WaitForEndOfFrame();

            InitHouseManager();
            //foreach(var bush in deadBushes)
            //{
            //    bush.OnCombustibleDestroyed += OnDeadBushRemoved;
            //}

            //foreach(var prop in props)
            //{
            //    prop.OnCombustibleDestroyed += OnPropsMoved;
            //}
            /*DOVirtual.DelayedCall(0.2f, () =>
            {
                InitHouseManager();
            });*/
            
        }

        //void OnDeadBushRemoved(FF_BaseCombustible bush)
        //{
        //    var bushToRemove = (FF_Plants)bush;
        //    //if (!deadBushes.Contains(bushToRemove) ) return;
        //    deadBushes.Remove(bushToRemove);
        //}

        //void OnPropsMoved(FF_BaseCombustible prop)
        //{
        //    var propToRmove = (FF_Props)prop;
        //    //if (!props.Contains(propToRmove)) return;
        //    props.Remove(propToRmove);
        //}

        public void InitHouseManager()
        {
            flammabilityMod = durabilityMod = 0f;
            initBudget = 35000;
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

            StringManager.Instance.OnStringsLoadedEvent -= UpdatePlayerName;
            StringManager.Instance.OnStringsLoadedEvent += UpdatePlayerName;

            UpdatePlayerName();
            //var nameKey = playerTag == "P1" ? player1NameKey : player2NameKey;
            //var name = StringManager.Instance.GetText(nameKey);
            //nameText.GetComponent<TextMeshPro>().text = name;

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
            if (!HH_GameManager.Instance.isTutorial && HH_GameManager.Instance.isNewLevel || isMoving)
            {
                StartCoroutine(RandomizeStartingCondition());
            }
            isMoving = false;
            HH_GameManager.Instance.inputManager.OnHouseSelected -= OnHouseSelected;
            HH_GameManager.Instance.inputManager.OnHouseSelected += OnHouseSelected;
            //houseGraph.PrintGraph();
            ToggleHousePartClickable(false);
        }

        private void UpdatePlayerName()
        {
            var nameKey = playerTag == "P1" ? player1NameKey : player2NameKey;
            var name = StringManager.Instance.GetText(nameKey);
            if(nameText != null)
                nameText.GetComponent<TextMeshPro>().text = name;
        }
        private void InitHouseNode(Dictionary<string, HouseNode> nodeDictionary, BaseHousePartObject part)
        {
            part.InitHousePartObject(this);
            var node = houseGraph.AddHousePart(part);
            part.houseNode = node;
            nodeDictionary[part.name] = node;

            if (part.partInfo != null)
            {
                inventory.AddNewPartToInventory(part.partInfo);
                if (!part.partInfo.isPublic)
                {
                    AddMaterialToDictionary(part.HousePartType, part.partInfo.materialClass);
                }
            }
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
                        oldPart.InitHousePartObject(this, newPart);
                    }
                    inventory.AddNewPartToInventory(newPart);
                }

            }
            burnedWeight = 0f;
            ToggleHousePartClickable(true);
            StartCoroutine(UpdateHouseUI());

        }

        public float GetRepairCost()
        {
            return burnedPercent <= 0.5f ? 5000f : 10000f;
        }
        public void ToggleClickBox(bool toggle)
        {
            clickBox.enabled = toggle;
        }

        public void OnHouseSelected(HouseManager manager)
        {
            if (manager != this) return;

            HH_GameManager.Instance.StartRound(manager);
            HH_GameManager.Instance.uiManager.ToggleEarnMoreMoneyButton(budgetManager.canEarnMoreMoney);
            ToggleClickBox(false);
            arrowUI.SetActive(true);
            nameText.SetActive(false);
            ToggleHousePartClickable(true);
            //UpdateHouseUI();
            StartCoroutine(UpdateHouseUI());
        }

        public void OnHouseDeselected()
        {
            foreach (var icon in purchaseFloatingButtons)
            {
                Destroy(icon.gameObject);
            }
            ToggleClickBox(true);
            ToggleHousePartClickable(false);
            purchaseFloatingButtons.Clear();
            arrowUI.SetActive(false);
        }

        public void CalculateTotalHousePartWeight()
        {
            totalWeight = 0;
            foreach(var node in houseGraph.nodes)
            {
                if(node != null)
                    totalWeight += node.housePart.HousePartType.GetHousePartWeight();
            }
            //Original method before partial burning fix
            /*if (burnedWeight != 0) return;
            totalWeight = 0;
            foreach(var node in houseGraph.nodes)
            {
                if(node != null)
                {
                    totalWeight += node.housePart.HousePartType.GetHousePartWeight();
                }
            }*/
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
                if (node.housePart.partInfo.partID == partInfo.partID && node.housePart.HousePartType == partInfo.housePartType && node.housePart.partInfo.materialClass == partInfo.materialClass)
                {
                    //Debug.Log($"{partInfo.partID} is in use by {node.housePart.name}");
                    return true;
                }
            }
            return false;
        }

        public void ReplaceHousePartObject(HousePartInfo newInfo)
        {
            PlayConstructSound();
            bool hideBubble = ShouldHideBubble(newInfo);

            var oldParts = GetAllHousePartObjectsOf(newInfo.housePartType, newInfo.isPublic);

            // Capture old material class from the first part (assumes uniform class across parts)
            var oldMaterial = oldParts.FirstOrDefault()?.partInfo.materialClass ?? MaterialClass.B;

            if (!newInfo.isPublic)
                AddMaterialToDictionary(newInfo.housePartType, newInfo.materialClass);

            foreach (var part in oldParts)
            {
                ToggleBubble(part, !hideBubble);
                ReplaceNode(part, newInfo);
            }

            if (newInfo.housePartType is HousePartType.Vent or HousePartType.Ground)
            {
                HandleGlobalModifiers(oldMaterial, newInfo.materialClass);
            }
            else
            {
                ApplyStoredModifiers();
            }

            // update inventory
            HH_GameManager.Instance.uiManager
                .inventoryPanel
                .UpdateInventoryUI(newInfo.housePartType, newInfo.isPublic);
        }

        // Helpers
        private void PlayConstructSound()
        {
            var clip = ResourceManager.Instance.RetrunRandomConstructSound();
            audioSource.PlayOneShot(clip);
        }

        private bool ShouldHideBubble(HousePartInfo info)
        {
            int ownedCount = info.isPublic
                ? inventory.ownedPublicParts[info.housePartType].Count
                : inventory.ownedParts[info.housePartType].Count;

            int totalAvailable = ResourceManager.Instance.allAvailableParts[info.housePartType].Count;
            return ownedCount >= totalAvailable;
        }

        private void ToggleBubble(BaseHousePartObject part, bool visible)
        {
            if (HH_GameManager.Instance.isTutorial || part == null) return;
            if (part.shouldDisplayBubble)
            {
                if(part.bubble != null)
                {
                    part.bubble.isActive = visible;
                    part.bubble.gameObject.SetActive(visible);
                    part.shouldDisplayBubble = visible;
                }

            }

        }

        private void ReplaceNode(BaseHousePartObject part, HousePartInfo info)
        {
            if (part.houseNode == null && info.isPublic)
            {
                part.InitHousePartObject(this, info);
                return;
            }

            // Store neighbors, remove node, re-init, re-add node, reconnect
            var neighbors = part.houseNode.neighbourNodes.ToList();
            houseGraph.RemoveHousePart(part.houseNode);

            part.InitHousePartObject(this, info);
            var newNode = houseGraph.AddHousePart(part);
            part.houseNode = newNode;

            foreach (var n in neighbors)
                houseGraph.ConnectParts(newNode, n);
        }

        private void HandleGlobalModifiers(MaterialClass oldClass, MaterialClass newClass)
        {
            // Revert old A-class buffs
            if (oldClass == MaterialClass.A)
            {
                flammabilityMod -= 0.1f;
                durabilityMod -= 0.1f;
                ApplyModifiersToAll(-0.1f, +0.1f, revert: true);
            }

            // Apply new A-class buffs
            if (newClass == MaterialClass.A)
            {
                flammabilityMod += 0.1f;
                durabilityMod += 0.1f;
                ApplyModifiersToAll(+0.1f, -0.1f);
            }
        }

        private void ApplyStoredModifiers()
        {
            ApplyModifiersToAll(durabilityMod, -flammabilityMod);
        }

        private void ApplyModifiersToAll(float durPercent, float flamPercent, bool revert = false)
        {
            foreach (var node in houseGraph.nodes)
            {
                var part = node.housePart;
                if (revert)
                {
                    part.DecreaseDurability(durPercent);
                    part.IncreaseFlammability(flamPercent);
                }
                else
                {
                    part.IncreaseDurability(durPercent);
                    part.DecreaseFlammability(flamPercent);
                }
            }
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
            HH_GameManager.Instance.uiManager.SetModeToggleState(false);
            yield return new WaitForSeconds(1f);

            //HashSet<HousePartType> displayedPartTypes = new HashSet<HousePartType>();
            HH_GameManager.Instance.uiManager.SetModeToggleState(true);

            foreach (var node in houseGraph.nodes)
            {
                //Debug.Log($"Init Bubble for {node.housePart}");
                if (!node.housePart.shouldDisplayBubble || ShouldHideBubble(node.housePart.partInfo)) continue;
                node.housePart.bubble = InitBubble(node.housePart);

            }

            foreach (var fence in fences)
            {
                //Debug.Log("Init Bubble for Fences");
                if (!fence.shouldDisplayBubble || fence == null || ShouldHideBubble(fence.partInfo)) continue;
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
                            oldPart.InitHousePartObject(this, res);
                        }

                        if (res.housePartType == HousePartType.Vent || res.housePartType == HousePartType.Ground)
                        {
                            if (res.materialClass != MaterialClass.A) return;
                            flammabilityMod = durabilityMod = 0.1f;
                            foreach (var node in houseGraph.nodes)
                            {
                                var part = node.housePart;
                                part.IncreaseDurability(0.1f);
                                part.DecreaseFlammability(0.1f);
                            }
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

        public float GetBurnedPercent()
        {
            if (totalWeight <= 0f) return 0;

            float currentDamageWeight = 0f;

            foreach (var node in houseGraph.nodes)
            {
                if (node != null && node.housePart != null)
                {
                    float weight = node.housePart.HousePartType.GetHousePartWeight();
                    currentDamageWeight += weight * node.housePart.GetDamageRatio();
                }
            }

            float totalBurned = burnedWeight + currentDamageWeight;
            
            burnedPercent = Mathf.Clamp01(totalBurned / totalWeight);
            //Debug.Log(burnedPercent * 100f);
            return burnedPercent * 100f;
            //Original code before partial burning fix
            /*if (totalWeight <= 0f)
                return 0;

            float rawPercent = (burnedWeight / totalWeight) * 100f;
            burnedPercent = rawPercent / 100f;
            return rawPercent;*/
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

