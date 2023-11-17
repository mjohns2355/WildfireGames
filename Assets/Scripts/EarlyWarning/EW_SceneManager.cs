using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public List<EW_StoryNode> nodeList = new List<EW_StoryNode>();
    private bool done = false;
    public EW_UIManager uiManager;
    static EW_StoryNode curNode;

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;
        nodeList = new List<EW_StoryNode>();

        //SetUpNodeList();
        string storyPath = "Assets/Scripts/EarlyWarning/EW_SampleStory.json";
        curNode = EW_StoryParser.Parse(storyPath, this, nodeList);
        Debug.Log("Curnode: " + curNode);
    }

    public void HandleChoice1()
    {
        Debug.Log("Choice 1 selected");
        EW_DialogueNode dNode = new EW_DialogueNode(new List<string> {
                    "Choice 1 selected",
                    "And that's the demo!"
                });
        ChangeStoryNode(dNode);
    }

    public void HandleChoice2()
    {
        Debug.Log("Choice 2 selected");
        ChangeStoryNode(new EW_DialogueNode(new List<string> {
                    "Choice 2 selected",
                    "And that's the demo!"
                }));
    }

    public void HandleChoice3()
    {
        Debug.Log("Choice 3 selected");
        ChangeStoryNode(new EW_DialogueNode(new List<string> {
                    "Choice 3 selected",
                    "And that's the demo!"
                }));
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

            Debug.Log("Curnode: " + curNode);
        }
    }

    public void ChangeStoryNode(EW_StoryNode node)
    {
        Debug.Log("Changing");
        Debug.Log(curNode);
        Debug.Log(node);
        if (curNode != node)
        {
            EW_EventSystem.InvokeLeaveStoryNodeEvent();
            curNode = node;
            if (curNode == null)
            {
                done = true;
                return;
            }
            curNode.Play();
        }
    }
}