using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace HappyHouse.HouseSystem
{
    public class HouseManager : MonoBehaviour
    {
        public HouseBlueprint houseBlueprint;
        public HouseGraph houseGraph;
        public RR_Inventory inventory;
        public float budegt;
        public string playerTag;
        public Vector3 positionOffset;
        public float scaleMultiplier;
        public GameObject craftIcon;
        public GameObject arrowUI;

        private List<PurchaseFloatingButton> purchaseFloatingButtons = new List<PurchaseFloatingButton>();   
        private void Start()
        {
            houseGraph = new HouseGraph();
            InitializeDefaultHouseLayout();

            HH_GameManager.Instance.inputManager.OnHouseSelected += OnHouseSelected;
        }

        public void OnHouseSelected(HouseManager manager)
        {
            HH_GameManager.Instance.inputManager.canClickHouse = false;
            if (manager != this) return;
            HH_GameManager.Instance.currentPlayer = this;
            arrowUI.SetActive(true);
            UpdateHouseUI();
        }

        public void OnHouseDeselected()
        {
            HH_GameManager.Instance.inputManager.canClickHouse = true;
            foreach(var icon in purchaseFloatingButtons)
            {
                Destroy(icon.gameObject);
            }
            purchaseFloatingButtons.Clear();
            arrowUI.SetActive(false);
        }
        void InitializeDefaultHouseLayout()
        {
            Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
            foreach (var part in houseBlueprint.partConnections)
            {
                var newPartInfo = part.partInfo;
                var houseObj = HH_GameManager.Instance.CreateHousePartObject(newPartInfo,this);
                houseObj.transform.parent = transform;
                houseObj.transform.localPosition = part.localPosition + positionOffset;
                houseObj.transform.localRotation = Quaternion.Euler(part.localRotation);
                houseObj.transform.localScale = part.localScale * scaleMultiplier;
                var node = houseGraph.AddHousePart(houseObj);
                houseObj.houseNode = node;
                nodeDictionary[part.partID] = node;
                inventory.AddNewPartToInventory(newPartInfo);
            }

            foreach (var part in houseBlueprint.partConnections)
            {
                if (nodeDictionary.TryGetValue(part.partID, out HouseNode currentNode))
                {
                    foreach (var connectedPartId in part.connectedPartsId)
                    {
                        if (nodeDictionary.TryGetValue(connectedPartId, out HouseNode connectedNode))
                        {
                            houseGraph.ConnectParts(currentNode, connectedNode);
                        }
                    }
                }
            }
        }

        public bool PurchaseHousePart(HousePartInfo partInfo)
        {
            if(budegt - partInfo.price < 0)
            {
                Debug.Log("Not enough budget");
                return false;
            }
            budegt -= partInfo.price;
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
                if(node.housePart.PartInfo.partID == partInfo.partID)
                {
                    Debug.Log($"{partInfo.partID} is in use by {playerTag}");
                    return true;
                }
            }
            return false;
        }

        public BaseHousePartObject GetCurrentInUseHousePartObject (HousePartType type)
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
        void UpdateHouseUI()
        {
            foreach(var node in houseGraph.nodes)
            {
                var icon = Instantiate(craftIcon, HH_GameManager.Instance.UIManager.transform).GetComponent<PurchaseFloatingButton>();
                purchaseFloatingButtons.Add(icon);
                icon.owner = node.housePart;
            }

        }
    }
}

