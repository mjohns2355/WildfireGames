using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGraph : MonoBehaviour
{
    public List<HouseNode> nodes = new List<HouseNode>();
    
    public HouseNode AddHousePart(HousePart newPart)
    {
        HouseNode newNode = new HouseNode(newPart);
        nodes.Add(newNode);
        return newNode;
    }

    public void RemoveHousePart(HouseNode node)
    {
        if (!nodes.Contains(node))
        {
            Debug.LogError("Remove Failed: Node is not existed");
        }
        nodes.Remove(node);
        // Remove connections to the node being removed
        foreach (var neighbour in node.neighbourNodes)
        {
            neighbour.RemoveConnection(node);
        }
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
