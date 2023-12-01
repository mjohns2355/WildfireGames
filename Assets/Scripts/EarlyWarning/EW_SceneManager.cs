using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public static List<EW_StoryNode> nodeList;
    private bool done = false;
    static EW_UIManager uiManager;
    public static EW_StoryNode curNode;
    public static int minutesRemaining = 120;
    public SpriteRenderer background;
    public GameObject actorParent;
    public bool storySelected = false;

    //Task list
    public static bool neighborTalk = false;
    public static bool goBag = false;
    public static bool cutLawn = false;
    public static bool cutTree = false;
    public static bool cleanedGutters = false;
    public static bool madeBreakfast = false;

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;
        nodeList = new List<EW_StoryNode>();
        uiManager = GetComponent<EW_UIManager>();
        background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
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

    public void EndPrefirePhase()
    {
        uiManager.HideUI();
        if (actorParent != null)
        {
            Destroy(actorParent);
        }
        curNode = null;
        background.enabled = false;
        done = true;
        uiManager.
        ShowTaskList();

        Invoke("ChangeStoryNodeWrapper", 3.0f);
    }
    
    private void EndOfPreFireWrapper()
    {
        ChangeStoryNode(25);
    }
    public void ChangeStoryNode(int nodeIndex)
    {
        if (nodeIndex == -1)
        {
            EndPrefirePhase();
            return;
        }
        // Debug.Log("Changing from " + curNode + " at index " + nodeList.IndexOf(curNode) + " to " + nodeList[nodeIndex] + " at index " + nodeIndex);
        if (curNode != nodeList[nodeIndex])
        {
            EW_EventSystem.InvokeLeaveStoryNodeEvent();
            if (done) { return; }
            curNode = nodeList[nodeIndex];
            curNode.Enter();
        }
        curNode.Play();
    }

    public void GoToArea(string areaName)
    {
        Debug.Log("Going to area " + areaName);

        string backgroundPath = "EarlyWarning/Art/" + areaName;
        Sprite backgroundSprite = Resources.Load<Sprite>(backgroundPath);
        if (backgroundSprite != null)
        {
            background.sprite = backgroundSprite;
        }

        string actorPath = "EarlyWarning/ActorPacks/" + areaName;
        GameObject actorPrefab = Resources.Load<GameObject>(actorPath);
        if (actorPrefab != null)
        {
            if (actorParent != null)
            {
                Destroy(actorParent);
            }
            actorParent = Instantiate(actorPrefab);
            actorParent.transform.SetParent(transform);
        }
        else
        {
            Debug.LogError("No actor prefab found at " + actorPath);
        }
    }
}