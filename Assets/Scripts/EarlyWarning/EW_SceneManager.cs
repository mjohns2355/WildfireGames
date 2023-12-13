using System;
using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public static Dictionary<int, EW_StoryNode> nodeDict;
    private bool done = false;
    static EW_UIManager uiManager;
    public EW_StoryNode curNode;
    public SpriteRenderer background;
    private GameObject actorParent;
    [NonSerialized]
    public bool storySelected = false;
    private bool ranOutOfTime = false;

    //List the names of stories that should be read from JSON
    public static string[] storyNames = new string[] {
        // "Sample Story",
        "Paul's Story"
    };

    //Task tracking variables
    //currentPhase is changed in UseUpTime()
    //Each task will be set to currentPhase when it is completed
    [NonSerialized]
    public int curPhase = 0;
    [NonSerialized]
    public string[] phaseStrings = new string[]
    {    
    //Note: If you change the text of these you may have to change a couple of places where they are referenced
        "Fire Season Starting",
        "Fire Season",
        "Fire Season",
        "Red Flag Day",
        "Red Flag Day",
        "Evacuation Warning!"
    };
    [NonSerialized]
    public Dictionary<string, string> tasksDone = new Dictionary<string, string>
    {
        {"neighborTalk", "notDone"},
        {"goBag", "notDone"},
        {"cutLawn", "notDone"},
        {"cutTree", "notDone"},
        {"cleanGutters", "notDone"},
        {"makeBreakfast", "notDone"},
        {"moveCar", "notDone"}
    };

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.Clear();
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;
        nodeDict = new Dictionary<int, EW_StoryNode>();
        uiManager = GetComponent<EW_UIManager>();
        background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        EW_StoryFunctions.sceneManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (curNode == null) { return; }
        bool newTouch = Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began);
        if ((newTouch || Input.anyKeyDown) && !done && storySelected)
        {
            //If the uiManager is still typing a message, fill the rest of the msg instead of progressing
            if (uiManager.typingCoroutine != null)
            {
                uiManager.SkipTyping();
                return;
            }

            Advance();
        }
    }

    private void Advance()
    {
        int nextNode = curNode.Advance();
        if (nextNode == curNode.id || nextNode == -2)
        {
            curNode.Play();
        }
        else if (nextNode == -1)
        {
            EndAndShowTaskList();
        }
        else
        {
            ChangeStoryNode(nextNode);
            curNode.Play();
        }
    }

    public void ChangeStoryNode(int targetID)
    {
        //Debug.Log("ChangeStoryNode from node " + curNode.id + " to node " + targetID);

        //Note the order of events here, as it may cause bugs if changed
        //Switch to the new node, leave the old node, Enter() the new node
        curNode = nodeDict[targetID];
        EW_EventSystem.InvokeLeaveStoryNodeEvent();
        curNode.Enter(); //See EW_StoryNodes.cs for difference between Enter() and Play()
    }

    public void Reset()
    {
        ranOutOfTime = false;
        foreach (string task in new List<string>(tasksDone.Keys))
        {
            tasksDone[task] = "notDone";
        }
        curPhase = 0;
        storySelected = false;
        done = false;
        nodeDict = new Dictionary<int, EW_StoryNode>();
        curNode = null;
    }

    public void DoTask(string taskName)
    {
        tasksDone[taskName] = phaseStrings[curPhase];

        if (taskName == "cutLawn" && phaseStrings[curPhase] == "Red Flag Day")
        {
            ChangeStoryNode(1301);
        }

        UseUpTime();
    }

    public bool TaskDone(string taskName)
    {
        return tasksDone[taskName] != "notDone";
    }

    //Advances to the next phase of the story
    public void UseUpTime()
    {
        curPhase++;
        if (curPhase >= phaseStrings.Length)
        {
            EW_EventSystem.LeaveStoryNodeEvent += TimesUp;
            return;
        }
        uiManager.UpdatePhaseText(phaseStrings[curPhase]);
    }

    //When the player runs out of time by doing enough actions, directs them to the appropriate node
    public void TimesUp()
    {
        ranOutOfTime = true;
        EW_EventSystem.LeaveStoryNodeEvent -= TimesUp;
        ChangeStoryNode(2000);
    }

    //Determines what text goes in the Epilogue based on what tasks were done and whether they escaped early
    //Called by Epilogue node after the player either runs out of time or escapes early
    //This is a mess, and will need to be changed and modularized when you add more stories or change Paul's story
    public void ShowEpilogue()
    {
        if (actorParent != null)
        {
            Destroy(actorParent);
        }
        background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/PaulEpilogue");

        List<EW_DialogueLine> lines = new List<EW_DialogueLine>();

        // didn't evacuate in time ending
        if (ranOutOfTime)
        {
            lines.Add(new EW_DialogueLine("Deciding to evacuate too late, Paul used his Pool as a last ditch protection method. Later, he was taken by medical crews to a local rendezvous point at the high school where he was treated for injuries"));
        }
        else
        {
            bool cutLawn = tasksDone["cutLawn"] != "notDone";
            bool cutTree = tasksDone["cutTree"] != "notDone";

            // incorrectly cleared lawn ending
            if ((cutTree || cutLawn) && (tasksDone["cutLawn"] == "Red Flag Day" || tasksDone["cutTree"] == "Red Flag Day"))
            {
                lines.Add(new EW_DialogueLine("Paul trimmed his lawn on a red flag day and subsequently started a large fire. Firefighters helped rescue him but his property was lost."));
                lines.Add(new EW_DialogueLine("Clearing ignitable material on a red flag day should never be done as it can quickly ignite.", "Cal Fire", true));
            }
            else if (tasksDone["moveCar"] != "notDone")
            {
                lines.Add(new EW_DialogueLine("With his car ready to go from previously backing it in, Paul sped off to the local rendezvous point downtown."));
            }
            else if (tasksDone["neighborTalk"] != "notDone")
            {
                lines.Add(new EW_DialogueLine("Paul runs over to Aldo's asking if he can join them in evacuating. They all quickly exit Marin county to stay at Aldo's parents vacation home in Napa, far from the fire."));
            }
            else
            {
                if (tasksDone["goBag"] != "notDone")
                {
                    lines.Add(new EW_DialogueLine("Paul grabs his go bag and hurries out the door"));
                }
                else
                {
                    lines.Add(new EW_DialogueLine("Paul hurries out the door, but without a go bag, he can only grab his wallet and keys"));
                }
                if ((cutTree || cutLawn) && (tasksDone["cutLawn"] != "Red Flag Day" || tasksDone["cutTree"] != "Red Flag Day"))
                {
                    lines.Add(new EW_DialogueLine("Having cleaned up his yard before the red flag day, Paul is able to use an escape trail down the road from his property. He is met by firefighters who assist him evacuating."));
                }
                else
                {
                    lines.Add(new EW_DialogueLine("Paul runs down the road from his property, but is stopped by a fallen tree. He is forced to turn back and wait for firefighters to assist him evacuating."));
                }
            }
        }

        EW_EpilogueNode epilogueNode = new EW_EpilogueNode(lines)
        {
            id = 9000,
            nextNode = -1
        };
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
        background.sprite = null;
        done = true;
        uiManager.ShowTaskList();
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