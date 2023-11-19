using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    static List<EW_StoryNode> nodeList;
    private bool done = false;
    public EW_UIManager uiManager;
    static EW_StoryNode curNode;

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;
        nodeList = new List<EW_StoryNode>();

        string storyPath = "Assets/Scripts/EarlyWarning/EW_SampleStory.json";
        curNode = EW_StoryParser.Parse(storyPath, this, nodeList);

        string list = "";
        foreach (var node in nodeList)
        {
            list += node.ToString() + "\n";
        }
        Debug.Log("Node list: " + list);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && !done)
        {
            if (uiManager.typingCoroutine != null)
            {
                uiManager.SkipTyping();
                return;
            }

            ChangeStoryNode(curNode.Advance());
        }
    }

    public void ChangeStoryNode(int nodeIndex)
    {
        if (nodeIndex == -1)
        {
            Debug.Log("Story finished!");
            curNode = null;
            done = true;
            return;
        }
        // Debug.Log("Changing from " + curNode + " at index " + nodeList.IndexOf(curNode) + " to " + nodeList[nodeIndex] + " at index " + nodeIndex);
        if (curNode != nodeList[nodeIndex])
        {
            EW_EventSystem.InvokeLeaveStoryNodeEvent();
            curNode = nodeList[nodeIndex];
        }
        curNode.Play();
    }

    //CHOICE FUNCTIONS

    public void HandleChoice1()
    {
        Debug.Log("Choice 1 selected");
        ChangeStoryNode(3);
    }

    public void HandleChoice2()
    {
        Debug.Log("Choice 2 selected");
        ChangeStoryNode(4);
    }

    public void HandleChoice3()
    {
        Debug.Log("Choice 3 selected");
        ChangeStoryNode(5);
    }
}