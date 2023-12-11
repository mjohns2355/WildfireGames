using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public static Dictionary<int, EW_StoryNode> nodeDict;
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
        nodeDict = new Dictionary<int, EW_StoryNode>();
        uiManager = GetComponent<EW_UIManager>();
        background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        EW_StoryFunctions.sceneManager = this;

        //List the names of the JSONs you want to read as options
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
            //If the uiManager is still typing a message, fill the rest of the msg instead of progressing
            if (uiManager.typingCoroutine != null)
            {
                uiManager.SkipTyping();
                return;
            }

            int nextNode = curNode.Advance();
            ChangeStoryNode(nextNode);
        }
    }

    //When the player runs out of time by doing enough actions, directs them to the appropriate node
    public void TimesUp()
    {
        EW_EventSystem.LeaveStoryNodeEvent -= TimesUp;
        ChangeStoryNode(2000);
    }

    //Determines what text goes in the Epilogue based on what tasks were done and whether they escaped early
    //Called by Epilogue node after the player either runs out of time or escapes early
    public void ShowEpilogue()
    {
        background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/Epilogue");

        EW_DialogueLine line1 = new EW_DialogueLine("This is the epilogue", "Epilogue", true);

        EW_DialogueNode epilogueNode = new EW_DialogueNode(new List<EW_DialogueLine> { line1 });
        epilogueNode.nextNode = -1;
        curNode = epilogueNode;
    }

    //Called when you leave the epilogue node
    public void EndAndShowTaskList()
    {
        uiManager.HideUI();
        if (actorParent != null)
        {
            Destroy(actorParent);
        }
        curNode = null;
        background.enabled = false;
        done = true;
        uiManager.ShowTaskList();
    }

    public void ChangeStoryNode(int nodeIndex)
    {
        if (nodeIndex == -1)
        {
            EndAndShowTaskList();
            return;
        }
        if (curNode != nodeDict[nodeIndex])
        {
            if (done) { return; }
            //Note the order of events here, as it may cause bugs if changed
            //Switch to the new node, leave the old node, Enter() the new node
            curNode = nodeDict[nodeIndex];
            EW_EventSystem.InvokeLeaveStoryNodeEvent();
            curNode.Enter(); //See EW_StoryNodes.cs for difference between Enter() and Play()
        }
        curNode.Play();
    }

    public void GoToArea(string areaName)
    {
        //Make sure the area name matches the name of the files in the Resources folder
        Debug.Log("Going to area " + areaName);

        //First load the background image
        string backgroundPath = "EarlyWarning/Art/" + areaName;
        Sprite backgroundSprite = Resources.Load<Sprite>(backgroundPath);
        if (backgroundSprite != null)
        {
            background.sprite = backgroundSprite;
        }

        //Then load/replace the actor prefab. This prefab contains each actor that will be present in the scene.
        string actorPath = "EarlyWarning/ActorPacks/" + areaName;
        GameObject actorPrefab = Resources.Load<GameObject>(actorPath);
        if (actorPrefab == null)
        {
            Debug.LogError("No actor prefab found at " + actorPath);
            return;
        }

        if (actorParent != null)
        {
            Destroy(actorParent);
        }
        actorParent = Instantiate(actorPrefab);
        actorParent.transform.SetParent(transform);
    }
}