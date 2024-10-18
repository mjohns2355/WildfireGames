using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHouse.HouseSystem
{
    public class HouseManager : MonoBehaviour
    {
        public HouseBlueprint houseBlueprint;
        public HouseGraph houseGraph;
        public Dictionary<HousePartType, List<HousePart>> ownedParts = new Dictionary<HousePartType, List<HousePart>>();
        public Vector3 positionOffset;
        public float scaleMultiplier;
        public GameObject craftIcon;
        private void Start()
        {
            houseGraph = new HouseGraph();
            InitializeDefaultHouseLayout();

            HH_InputManager.Instance.OnHouseSelected += HouseClickedBehavior;
        }

        private void HouseClickedBehavior(HouseManager manager)
        {
            HH_InputManager.Instance.canClickHouse = false;
            if (manager != this) return;
            UpdateHouseUI();
        }

        void InitializeDefaultHouseLayout()
        {
            Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
            foreach (var part in houseBlueprint.partConnections)
            {
                //var obj = Instantiate(part.partPrefab, transform);
                var partInfo = part.partInfo;
                if (ownedParts.ContainsKey(partInfo.housePartType))
                {
                    var value = ownedParts[partInfo.housePartType];
                    if (!value.Contains(partInfo))
                    {
                        value.Add(partInfo);
                    }
                }
                else
                {
                    ownedParts.Add(partInfo.housePartType, new List<HousePart> { partInfo }); // Add new key-value pair
                }
                var obj = new GameObject(partInfo.name); 
                obj.transform.parent = transform;
                obj.transform.localPosition = part.localPosition + positionOffset;
                obj.transform.localRotation = Quaternion.Euler(part.localRotation);
                obj.transform.localScale = part.localScale;

                //var houseObj = obj.GetComponent<BaseHousePartObject>();
                var houseObj = obj.AddComponent<BaseHousePartObject>();
                
                houseObj.InitHousePartObject(partInfo);
                houseObj.houseManager = this;
                //var housePart = houseObj.housePart;
                var node = houseGraph.AddHousePart(houseObj);
                houseObj.houseNode = node;
                nodeDictionary[part.partID] = node;
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

        void UpdateHouseUI()
        {
            foreach(var node in houseGraph.nodes)
            {
                var icon = Instantiate(craftIcon, HH_GameManager.Instance.UIManager.transform).GetComponent<CraftIcon>();
                icon.owner = node.housePart;
            }

        }
    }
}

