using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
public class HouseGraph
{
    public List<HouseNode> nodes = new List<HouseNode>();

    public List<HouseNode> GetNeighbors(HouseNode node)
    {
        return nodes.Contains(node) ? node.neighbourNodes : null;
    }

    public HouseNode AddHousePart(BaseHousePartObject newPart)
    {
        HouseNode newNode = new HouseNode(newPart);
        nodes.Add(newNode);
        Debug.Log("Added new node: " + newNode.housePart.name);
        return newNode;
    }

    public void RemoveHousePart(HouseNode node)
    {
        if (!nodes.Contains(node))
        {
            Debug.LogError($"Remove Failed: Node {node.housePart.name} is not existed");
        }
        nodes.Remove(node);
        Debug.Log($"Removed node {node.housePart.name}");
        // Remove connections to the node being removed

        for (int i = node.neighbourNodes.Count - 1; i >= 0; i--)
        {
            var neighbour = node.neighbourNodes[i];
            neighbour.RemoveConnection(node); // Modify the collection safely
        }
        //foreach (var neighbour in node.neighbourNodes)
        //{
        //    neighbour.RemoveConnection(node);
        //}
    }

    public void ConnectParts(HouseNode part1, HouseNode part2)
    {
        if(part1 == null || part2 == null)
        {
            Debug.LogWarning("Connect Failed");
            return;
        }
        part1.AddConnection(part2);
    }

    public void DisconnectParts(HouseNode part1, HouseNode part2)
    {
        if(part1 == null || part2 == null)
        {
            Debug.LogWarning("Disconnect Failed");
            return;
        }

        part1.RemoveConnection(part2);
    }
}
