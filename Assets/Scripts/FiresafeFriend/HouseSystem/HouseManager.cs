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
        
        public Transform camTransform;
        public HouseGraph houseGraph;
        public RR_Inventory inventory;
        // player budget
        public FF_BudgetManager budgetManager;
        public int initBudget;
        public string playerTag;
        public Vector3 positionOffset;
        public float scaleMultiplier;
        public GameObject /*craftIcon, */arrowUI,nameText;
        private List<PurchaseFloatingButton> purchaseFloatingButtons = new List<PurchaseFloatingButton>();
        //[SerializeField] BoxCollider clickBox;
      
        private void Start()
        {
            Debug.Log("test");
            houseGraph = new HouseGraph();
            budgetManager = new FF_BudgetManager(this,initBudget);

            var fences = HH_GameManager.Instance.fences;
            Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var part = transform.GetChild(i).GetComponent<BaseHousePartObject>();
                if (part.notInteractable) continue;
                InitHouseNode(nodeDictionary, part);
            }

            foreach(var f in fences)
            {
                InitHouseNode(nodeDictionary, f.GetComponent<BaseHousePartObject>());
            }

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

            //var allAvailableParts = ResourceManager.Instance.allAvailableParts;

            //// TO DO: Improve the code
            //foreach(var key in allAvailableParts.Keys)
            //{
            //    var allInfos = allAvailableParts[key];
            //    int index = UnityEngine.Random.Range(0, allInfos.Count);
            //    var res = allInfos[index];
            //    var oldParts = GetAllHousePartObjectsOf(res.housePartType);
            //    //var oldParts = GetAllHousePartObjects(newPart.HousePartType);
            //    //Debug.Log($"{oldParts.Count} pieces of {newPart.name} is in use");

            //    foreach (var oldPart in oldParts)
            //    {
            //        //oldPart.partInfo = housePartInfo;
            //        oldPart.InitHousePartObject(this, res);
            //    }
            //    inventory.AddNewPartToInventory(res);
            //}
            
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
            //ToggleClickBox(false);
            arrowUI.SetActive(true);
            nameText.SetActive(false);
            //UpdateHouseUI();
            StartCoroutine(UpdateHouseUI());
        }

        public void OnHouseDeselected()
        {
            HH_GameManager.Instance.inputManager.canClickHouse = true;
            foreach(var icon in purchaseFloatingButtons)
            {
                Destroy(icon.gameObject);
            }
            //ToggleClickBox(true);
            purchaseFloatingButtons.Clear();
            arrowUI.SetActive(false);
            
        }
        //void InitializeDefaultHouseLayout()
        //{
        //    Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
        //    foreach (var part in houseBlueprint.partConnections)
        //    {
        //        var newPartInfo = part.partInfo;
        //        var houseObj = HH_GameManager.Instance.CreateHousePartObject(newPartInfo,this);
        //        houseObj.transform.parent = transform;
        //        houseObj.transform.localPosition = part.localPosition + positionOffset;
        //        houseObj.transform.localRotation = Quaternion.Euler(part.localRotation);
        //        houseObj.transform.localScale = part.localScale * scaleMultiplier;
        //        var node = houseGraph.AddHousePart(houseObj);
        //        houseObj.houseNode = node;
        //        nodeDictionary[part.partID] = node;
        //        inventory.AddNewPartToInventory(newPartInfo);
        //    }

        //    foreach (var part in houseBlueprint.partConnections)
        //    {
        //        if (nodeDictionary.TryGetValue(part.partID, out HouseNode currentNode))
        //        {
        //            foreach (var connectedPartId in part.connectedPartsId)
        //            {
        //                if (nodeDictionary.TryGetValue(connectedPartId, out HouseNode connectedNode))
        //                {
        //                    houseGraph.ConnectParts(currentNode, connectedNode);
        //                }
        //            }
        //        }
        //    }
        //}

        public bool PurchaseHousePart(HousePartInfo partInfo)
        {
            if (!budgetManager.SpendBudget(partInfo.price)) { return false; }
            // add to inventory
            Debug.Log($"Player {playerTag}: ");
            inventory.AddNewPartToInventory(partInfo);
            //HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(partInfo.housePartType);
            return true;
        }

        public void ToggleAllPurchaseIcons(bool state)
        {
            foreach (var icon in purchaseFloatingButtons)
            {
                icon.gameObject.SetActive(state);
            }
        }

        public bool PartIsInUse(HousePartInfo partInfo)
        {
            foreach (var node in houseGraph.nodes)
            {
                if(node.housePart.partInfo.partID == partInfo.partID)
                {
                    //Debug.Log($"{partInfo.partID} is in use by {playerTag}");
                    return true;
                }
            }
            return false;
        }

        public void ReplaceHousePartObject (/*BaseHousePartObject newPart*/ HousePartInfo housePartInfo)
        {
            bool shouldHideBubble = ResourceManager.Instance.allAvailableParts[housePartInfo.housePartType].Count == inventory.ownedParts[housePartInfo.housePartType].Count;
           
            var oldParts = GetAllHousePartObjectsOf(housePartInfo.housePartType);
            //var oldParts = GetAllHousePartObjects(newPart.HousePartType);
            //Debug.Log($"{oldParts.Count} pieces of {newPart.name} is in use");
            
            foreach (var oldPart in oldParts)
            {
                if (oldPart.shouldDisplayBubble)
                {
                    oldPart.bubble.gameObject.SetActive(!shouldHideBubble);
                    oldPart.shouldDisplayBubble = !shouldHideBubble;
                }
                //oldPart.partInfo = housePartInfo;
                oldPart.InitHousePartObject(this,housePartInfo);
            }
            //HH_GameManager.Instance.UIManager.inventoryUI.UpdateOwnedParts(newPart.HousePartType);
            HH_GameManager.Instance.uiManager.inventoryPanel.UpdateInventoryUI(housePartInfo.housePartType);
        }

        public BaseHousePartObject GetCurrentInUseHousePartObjectOf(HousePartType type)
        {
            foreach(var node in houseGraph.nodes)
            {
                if(node.housePart.HousePartType == type)
                {
                    return node.housePart;
                }
            }
            return null;
        }

        public List<BaseHousePartObject> GetAllHousePartObjectsOf (HousePartType type)
        {
            var res = new List<BaseHousePartObject>();
            Debug.Log($"Replace {type}");
            foreach(var node in houseGraph.nodes)
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
                if(!node.housePart.shouldDisplayBubble) continue;
                //HousePartType partType = node.housePart.HousePartType;

                //if (displayedPartTypes.Contains(partType))
                //{
                //    continue;
                //}

                //displayedPartTypes.Add(partType);
                //var icon = Instantiate(craftIcon, HH_GameManager.Instance.uiManager.floatingIcons).GetComponent<PurchaseFloatingButton>();
                var icon = HH_GameManager.Instance.uiManager.SpawnBubble();
                purchaseFloatingButtons.Add(icon);

                icon.InitBubbleForHousePart(node.housePart);
                node.housePart.bubble = icon;;

            }
        }

    }
}

