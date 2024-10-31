using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogTree", menuName = "Dialog/DialogTree")]
public class ATC_DialogTree : ScriptableObject
{
    public DialogNode rootNode;
}
[System.Serializable]
public class DialogNode
{
    public string[] messages;
    public DialogOption[] options; // Possible player choices
}

[System.Serializable]
public class DialogOption
{
    public string optionText;
    public DialogNode nextNode; // The next node that this option leads to
    public bool isEndNode; // Marks if this option ends the dialog tree
}