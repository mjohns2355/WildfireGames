using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHouse.HouseSystem
{
    public class HouseNode
    {
        public HousePart housePart;
        public List<HouseNode> neighbourNodes;

        public HouseNode(HousePart part)
        {
            housePart = part;
            neighbourNodes = new List<HouseNode>();
        }

        // add connection to another node

        public void AddConnection(HouseNode node)
        {
            if (!neighbourNodes.Contains(node))
            {
                neighbourNodes.Add(node);
                node.neighbourNodes.Add(this);  // Bidirectional connection
            }
        }

        // Remove a connection to another node
        public void RemoveConnection(HouseNode node)
        {
            if (neighbourNodes.Contains(node))
            {
                neighbourNodes.Remove(node);
                node.neighbourNodes.Remove(this);
            }
        }
    }
}

