using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
namespace HappyHouse.HouseSystem
{
    public class HouseManager : MonoBehaviour
    {
        //public HouseBlueprint houseBlueprint;

        //public Transform camTransform;
        public HouseGraph houseGraph;
        public RR_Inventory inventory;
        // player budget
        public FF_BudgetManager budgetManager;
        public int initBudget;
        public string playerTag;
        public Vector3 positionOffset;
        public float scaleMultiplier;
        public GameObject /*craftIcon, */arrowUI, nameText;
        [SerializeField] private float brickWallChance, stuccoWallChance, compositeRoofChance, otherPartUpgradeChance;
        private int upgradeCount = 0;
        private List<PurchaseFloatingButton> purchaseFloatingButtons = new List<PurchaseFloatingButton>();
        private List<BaseHousePartObject> fences;
        //[SerializeField] BoxCollider clickBox;
        [SerializeField] private List<HousePartType> upgradeList = new List<HousePartType> { HousePartType.Wall, HousePartType.Roof, HousePartType.Gutter, HousePartType.Vent, HousePartType.Drain, HousePartType.Window, HousePartType.Door };
        private void Start()
        {

            houseGraph = new HouseGraph();
            budgetManager = new FF_BudgetManager(this, initBudget);

            fences = HH_GameManager.Instance.publicFences;
            Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var part = transform.GetChild(i).GetComponent<BaseHousePartObject>();
                if (part.notInteractable) continue;
                InitHouseNode(nodeDictionary, part);
            }

            //foreach (var f in fences)
            //{
            //    InitHouseNode(nodeDictionary, f.GetComponent<BaseHousePartObject>());
            //}

            for (int i = 0; i < transform.childCount; i++)
            {
                var part = transform.GetChild(i).GetComponent<BaseHousePartObject>();
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

            if (!HH_GameManager.Instance.isTutorial)
            {
                StartCoroutine(RandomizeStartingCondition());
            }

            HH_GameManager.Instance.inputManager.OnHouseSelected += OnHouseSelected;
        }

        private void InitHouseNode(Dictionary<string, HouseNode> nodeDictionary, BaseHousePartObject part)
        {
            part.InitHousePartObject(this);
            //part.InitHousePartObject(this, allInfos[index]);
            var node = houseGraph.AddHousePart(part);
            part.houseNode = node;
            nodeDictionary[part.name] = node;
            inventory.AddNewPartToInventory(part.partInfo);
            //inventory.AddNewPartToInventory(allInfos[index]);
        }

        public void ToggleClickBox(bool toggle)
        {
            //clickBox.enabled = toggle;
        }
        public void OnHouseSelected(HouseManager manager)
        {
            if (manager != this) return;
            HH_GameManager.Instance.StartRound(manager);
            HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(budgetManager.canEarnMoreMoney);
            //ToggleClickBox(false);
            arrowUI.SetActive(true);
            nameText.SetActive(false);
            //UpdateHouseUI();
            StartCoroutine(UpdateHouseUI());
        }

        public void OnHouseDeselected()
        {
            HH_GameManager.Instance.inputManager.canClickHouse = true;
            foreach (var icon in purchaseFloatingButtons)
            {
                Destroy(icon.gameObject);
            }
            //ToggleClickBox(true);
            purchaseFloatingButtons.Clear();
            arrowUI.SetActive(false);

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
            bool shouldHideBubble = ResourceManager.Instance.allAvailableParts[housePartInfo.housePartType].Count == inventory.ownedParts[housePartInfo.housePartType].Count;

            var oldParts = GetAllHousePartObjectsOf(housePartInfo.housePartType, housePartInfo.isPublic);
            //var oldParts = GetAllHousePartObjects(newPart.HousePartType);
            //Debug.Log($"{oldParts.Count} pieces of {housePartInfo.housePartType} is in use");

            foreach (var oldPart in oldParts)
            {
 
                if (oldPart.houseNode != null)
                {
                    if (oldPart.shouldDisplayBubble)
                    {
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
                else if(housePartInfo.isPublic)
                {
                    oldPart.InitHousePartObject(this, housePartInfo);
                }
            }
            //HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(newPart.HousePartType);
            HH_GameManager.Instance.uiManager.inventoryPanel.UpdateInventoryUI(housePartInfo.housePartType);
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
                if (!node.housePart.shouldDisplayBubble) continue;
                node.housePart.bubble = InitBubble(node.housePart);

            }

            foreach(var fence in fences)
            {
                
                if (!fence.shouldDisplayBubble) continue;
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
                            inventory.RemovePartFromInventory(oldPart.defaultPartInfo);
                            //if (oldPart.houseNode != null)
                            //{
                            //    houseGraph.RemoveHousePart(oldPart.houseNode);
                            //}
                            oldPart.InitHousePartObject(this, res);
                        }
                        inventory.AddNewPartToInventory(res);
                    }

                   
                }
            

            }
            HousePartInfo RandomizeWall()
            {
                HousePartInfo material = null;
                var anotherHouse = playerTag == "P1"? HH_GameManager.Instance.p2 : HH_GameManager.Instance.p1;
                var rng = UnityEngine.Random.value;
                var anotherHouseWall = anotherHouse.GetCurrentInUseHousePartObjectOf(HousePartType.Wall).partInfo;
                if (rng < brickWallChance/10f && anotherHouseWall.partClass != MaterialClass.B)
                {
                    // brick
                    upgradeCount++;
                    
                    material = ResourceManager.Instance.allAvailableParts[HousePartType.Wall].Find(x => x.partClass == MaterialClass.B);
                    return material;
                }
                if (rng < stuccoWallChance/10f )
                {
                    //stucco
                    upgradeCount++;
                    
                    material = ResourceManager.Instance.allAvailableParts[HousePartType.Wall].Find(x => x.partClass == MaterialClass.C);
                    return material;
                }
                return material;

            }

            HousePartInfo RandomizeRoof()
            {
                
                HousePartInfo material = null;
                var rng = UnityEngine.Random.value;
                if (rng < compositeRoofChance/10f)
                {
                    //composite
                    upgradeCount++;
                    material = ResourceManager.Instance.allAvailableParts[HousePartType.Roof].Find(x => x.partClass == MaterialClass.C);
                   
                }

                return material;
            }

            HousePartInfo RandomizeOtherParts(HousePartType type)
            {
                
                HousePartInfo material = null;
                var rng = UnityEngine.Random.value;
                if (rng < otherPartUpgradeChance/10f)
                {
                    upgradeCount++;
                    material = ResourceManager.Instance.allAvailableParts[type].Find(x => x.partClass == MaterialClass.A);
                }

                return material;
            }
        }
    }
}

