using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public static List<EW_StoryNode> nodeList;
    private bool done = false;
    static EW_UIManager uiManager;
    public static EW_StoryNode curNode;
    public static int minutesRemaining = 120;
    public SpriteRenderer background, paulSprite;
    public Sprite livingRoomSprite;
    public bool storySelected = false;

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;
        nodeList = new List<EW_StoryNode>();
        uiManager = GetComponent<EW_UIManager>();
        background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        paulSprite = GameObject.Find("Paul").GetComponent<SpriteRenderer>();
        paulSprite.enabled = false;
        uiManager.timerText.text = "";
        EW_StoryFunctions.sceneManager = this;

        string[] storyNames = new string[] {
            "EW_SampleStory",
            "EW_PaulStory"
        };
        uiManager.CreateStoryButtons(storyNames);
    }

    // Update is called once per frame
    void Update()
    {
        bool newTouch = Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began);
        if ((newTouch || Input.anyKeyDown) && !done && storySelected)
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
        if (nodeIndex == 1)
        {
            background.sprite = livingRoomSprite;
            paulSprite.enabled = true;
        }

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
            curNode.Enter();
        }
        curNode.Play();
    }
}