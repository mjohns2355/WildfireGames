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
        public Dictionary<HousePartType, List<HousePartInfo>> inventory = new Dictionary<HousePartType, List<HousePartInfo>>();
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
                //var obj = Instantiate(part.partPrefab, transform);
                var newPartInfo = part.partInfo;
                var obj = new GameObject(newPartInfo.name); 
                obj.transform.parent = transform;
                obj.transform.localPosition = part.localPosition + positionOffset;
                obj.transform.localRotation = Quaternion.Euler(part.localRotation);
                obj.transform.localScale = part.localScale;

                //var houseObj = obj.GetComponent<BaseHousePartObject>();
                var houseObj = obj.AddComponent<BaseHousePartObject>();
                
                houseObj.InitHousePartObject(newPartInfo,this);
                //houseObj.owner = this;
                //var housePart = houseObj.housePart;
                var node = houseGraph.AddHousePart(houseObj);
                houseObj.houseNode = node;
                nodeDictionary[part.partID] = node;
                AddNewPartToInventory(newPartInfo);
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
            // update price text ui
            // add to inventory
            AddNewPartToInventory(partInfo);
            return true;
        }

        bool AddNewPartToInventory(HousePartInfo newPartInfo)
        {
            if (inventory.ContainsKey(newPartInfo.housePartType))
            {
                var value = inventory[newPartInfo.housePartType];

                if (value.Exists(part => part.name == newPartInfo.name)) return false;
                value.Add(Instantiate(newPartInfo));
            }
            else
            {
                inventory.Add(newPartInfo.housePartType, new List<HousePartInfo> { newPartInfo }); // Add new key-value pair
            }
            return true;
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

