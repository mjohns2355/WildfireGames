using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(fileName = "DialogTree", menuName = "Dialog/DialogTree")]
[System.Serializable]
public class ATC_DialogTree
{
    public string houseType;
    public string rootNodeId;
    public List<DialogNode> nodes;
    public DialogFlags flags;
    public DialogNode GetNodeById(string id)
    {
        var variants = nodes.Where(n => n.id == id).ToList();
        Debug.Log("Variants count: " + variants.Count);
        foreach (var node in variants)
        {
            if (node.conditions == null) return node;
            Debug.Log("Node conditions: " + node.conditions.hasSpoken + ", " + node.conditions.hasIncentives + ", " + node.conditions.gaveIncentives);
            Debug.Log("Flags: " + flags.hasSpoken + ", " + flags.hasIncentives + ", " + flags.gaveIncentives);

            if ((!node.conditions.hasSpoken || flags.hasSpoken) &&
                (!node.conditions.hasIncentives || flags.hasIncentives) &&
                (!node.conditions.gaveIncentives || flags.gaveIncentives))
            {
                return node;
            }
        }
        return null;
        //return nodes.Find(node => node.id == id);
    }


}
[System.Serializable]
public class DialogFlags
{
    public bool hasSpoken;
    public bool hasIncentives;
    public bool gaveIncentives;
}
[System.Serializable]
public class DialogNode
{
    public string id;
    public string variantGroup;
    public string dialogText;
    public string characterName;
    public string portraitPath;
    //public string[] messages;
    public DialogOption[] options;
    public bool isEndNode;
    public DialogCondition conditions;
    public string GetNextNodeId()
    {
        string nextNodeId;
        if (options == null)
        {
            nextNodeId = (Convert.ToInt32(id) + 1).ToString();
        }
        else
        {
            nextNodeId = options[0].nextNodeId ?? (Convert.ToInt32(id) + 1).ToString();
        }
        Debug.Log("Next node id: " + nextNodeId);
        return nextNodeId;
    }
}
[System.Serializable]
public class DialogCondition
{
    public bool hasSpoken;
    public bool hasIncentives;
    public bool gaveIncentives;
}
[System.Serializable]
public class DialogOption
{
    public string parentId;
    public string option;
    // text on option button
    public string optionText;
    // text actually sent in message
    public string messageText;
    public string nextNodeId;
    public DialogCondition conditions;
    public string GetNextNodeId()
    {
        return nextNodeId?? (Convert.ToInt32(parentId) + 1).ToString();
    }
}

[System.Serializable]
public class DialogTreeCollection
{
    public List<ATC_DialogTree> dialogTrees; // A list of all dialog trees in the JSON file
}