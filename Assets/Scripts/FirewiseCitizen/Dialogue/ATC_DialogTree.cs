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

        foreach (var node in variants)
        {
            if (node.conditions == null || node.conditions.IsEmpty()) return node;
           
            //Debug.Log("Flags: " + flags.hasSpoken + ", " + flags.hasIncentives + ", " + flags.gaveIncentives);

            if (!node.conditions.IsMet(flags)) continue;
            //Debug.Log("Node conditions met: " + node.dialogText);
            return node;
        }
        return null;
        //return nodes.Find(node => node.id == id);
    }


}
[System.Serializable]
public class DialogFlags
{
    public int hasSpoken =  -1;
    public int hasIncentives = -1;
    public int gaveIncentives = -1;
}
[System.Serializable]
public class DialogNode
{
    public string id;
    //public string variantGroup;
    public string dialogText;
    public string dialogTextES;
    public string characterName;
    public string portraitPath;
    public string audioPath;
    public string audioPathES;
    //public string[] messages;
    public DialogOption[] options;
    public bool isEndNode;
    public DialogCondition conditions;
    public string nextNodeId;
    public string GetNextNodeId()
    {
        string nextId;
        if (options == null)
        {
            nextId = nextNodeId?? (Convert.ToInt32(id) + 1).ToString();
        }
        else
        {
            nextId = options[0].nextNodeId ?? (Convert.ToInt32(id) + 1).ToString();
        }
        //Debug.Log("Next node id: " + nextNodeId);
        return nextId;
    }
}
[System.Serializable]
public class DialogCondition
{
    public int hasSpoken = -1;
    public int hasIncentives = -1;
    public int gaveIncentives = -1;

    public bool IsMet(DialogFlags flags)
    {
        return Matches(flags.hasSpoken, hasSpoken) &&
               Matches(flags.hasIncentives, hasIncentives) &&
               Matches(flags.gaveIncentives, gaveIncentives);
    }

    private bool Matches(int actual, int expected)
    {
        return expected == -1 || actual == expected;
    }

    public bool IsEmpty()
    {
        return hasSpoken == -1 && hasIncentives == -1 && gaveIncentives == -1;
    }
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