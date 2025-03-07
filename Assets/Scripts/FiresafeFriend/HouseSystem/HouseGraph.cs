using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyHouse.HouseSystem;
using System.Linq;
public class HouseGraph
{
    public List<HouseNode> nodes = new List<HouseNode>();

    public List<HouseNode> GetNeighbors(HouseNode node)
    {
        return nodes.Contains(node) ? node.neighbourNodes : null;
    }

    public HouseNode AddHousePart(BaseHousePartObject newPart)
    {
        HouseNode newNode;
        //HouseNode newNode = new HouseNode(newPart);
        if (newPart.houseNode == null)
        {
            newNode = new HouseNode(newPart);
        }
        else
        {
            newNode = newPart.houseNode;
        }
        nodes.Add(newNode);
        //Debug.Log("Added new node: " + newNode.housePart.name);
        return newNode;
    }

    public void RemoveHousePart(HouseNode node)
    {
        if (!nodes.Contains(node))
        {
            //Debug.Log($"Try removing node: {node.housePart.name}");
            //Debug.Log("All neighbours: ");
            //foreach (HouseNode neighbour in nodes)
            //{
            //    Debug.Log($"Neighbour: {neighbour.housePart.name}");
            //}
           Debug.LogError("Remove Failed: Node is not existed");
            return;
        }
        nodes.Remove(node);
        //Debug.Log($"Removed node {node.housePart.name}");
        // Remove connections to the node being removed

        foreach (var neighbour in node.neighbourNodes.ToList())
        {
            neighbour.RemoveConnection(node);
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

    public void PrintGraph()
    {
        if (nodes.Count == 0)
        {
            Debug.Log("The graph is empty.");
            return;
        }

        foreach (var node in nodes)
        {
            string nodeInfo = $"{node.housePart.name} (Node)";
            if (node.neighbourNodes.Count > 0)
            {
                nodeInfo += " -> Neighbors: ";
                foreach (var neighbor in node.neighbourNodes)
                {
                    nodeInfo += $"{neighbor.housePart.name} (type:{neighbor.housePart.HousePartType} ), ";
                }
                nodeInfo = nodeInfo.TrimEnd(',', ' '); // Remove trailing comma
            }
            else
            {
                nodeInfo += " has no neighbors.";
            }

            Debug.Log(nodeInfo);
        }
    }
}
