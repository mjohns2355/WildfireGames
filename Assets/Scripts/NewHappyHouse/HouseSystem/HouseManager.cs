using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHouse.HouseSystem
{
    public class HouseManager : MonoBehaviour
    {
        public HouseBlueprint houseBlueprint;
        public HouseGraph houseGraph;
        private void Start()
        {
            houseGraph = new HouseGraph();
            InitializeDefaultHouseLayout();
        }

        void InitializeDefaultHouseLayout()
        {
            Dictionary<string, HouseNode> nodeDictionary = new Dictionary<string, HouseNode>();
            foreach (var part in houseBlueprint.partConnections)
            {
                var obj = Instantiate(part.partPrefab, transform);
                obj.transform.localPosition = part.localPosition;
                obj.transform.localRotation = Quaternion.Euler(part.localRotation);
                obj.transform.localScale = part.localScale;

                var houseObj = obj.GetComponent<BaseHousePartObject>();
                houseObj.houseManager = this;
                var housePart = houseObj.housePart;
                var node = houseGraph.AddHousePart(housePart);
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
    }
}

