using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "DialogTree", menuName = "Dialog/DialogTree")]
[System.Serializable]
public class ATC_DialogTree
{
    public string houseType;
    public string rootNodeId;
    public List<DialogNode> nodes;
    public DialogNode GetNodeById(string id)
    {
        return nodes.Find(node => node.id == id);
    }
}
[System.Serializable]
public class DialogNode
{
    public string id;
    public string dialogText;
    public string characterName;
    public string portraitPath;
    //public string[] messages;
    public DialogOption[] options;
    public bool isEndNode;
}

[System.Serializable]
public class DialogOption
{
    public string option;
    public string optionText;
    public string nextNodeId;

}

[System.Serializable]
public class DialogTreeCollection
{
    public List<ATC_DialogTree> dialogTrees; // A list of all dialog trees in the JSON file
}