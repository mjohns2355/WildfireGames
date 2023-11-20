using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    static List<EW_StoryNode> nodeList;
    private bool done = false;
    static EW_UIManager uiManager;
    static EW_StoryNode curNode;
    static int minutesRemaining = 120;

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;
        nodeList = new List<EW_StoryNode>();
        uiManager = GetComponent<EW_UIManager>();

        string storyPath = "Assets/Scripts/EarlyWarning/EW_PaulStory.json";
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
    public void useUpTime(int time)
    {
        minutesRemaining -= time;
        uiManager.updateTimer(minutesRemaining);
    }

    public void HandleBreakfast()
    {
        useUpTime(30);
        ChangeStoryNode(7);
    }

    public void HandleGoBag()
    {
        useUpTime(30);
        ChangeStoryNode(9);
    }

    public void HandleCheckYard()
    {
        Debug.Log("Paul checks the yard");
        ChangeStoryNode(11);
    }

    public void HandleCheckBackyard()
    {
        Debug.Log("Paul checks the backyard");
        ChangeStoryNode(19);
    }

    public void HandleCheckFrontyard()
    {
        Debug.Log("Paul checks the frontyard");
        ChangeStoryNode(13);
    }

    public void HandleCheckDownstairs()
    {
        Debug.Log("Paul checks downstairs");
        ChangeStoryNode(5);
    }

    public void HandleCutLawn()
    {
        useUpTime(30);
        Debug.Log("Paul cuts the lawn");
        ChangeStoryNode(15);
    }

    public void HandleAldoTalk()
    {
        useUpTime(30);
        Debug.Log("Paul talks to Aldo");
        ChangeStoryNode(17);
    }

    public void HandleCutTree()
    {
        useUpTime(30);
        Debug.Log("Paul cuts the tree");
        ChangeStoryNode(-1);
    }

    public void HandleCleanGutter()
    {
        useUpTime(30);
        Debug.Log("Paul cleans the gutter");
        ChangeStoryNode(-1);
    }
}