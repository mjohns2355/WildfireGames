using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public static Dictionary<int, EW_StoryNode> nodeDict;
    private bool done = false;
    static EW_UIManager uiManager;
    public static EW_StoryNode curNode;
    public SpriteRenderer background;
    public GameObject actorParent;
    public bool storySelected = false;

    //Task tracking variables
    //currentPhase is changed in UseUpTime()
    public string currentPhase = "Fire Season Starting";
    //Each task will be set to currentPhase when it is completed

    public Dictionary<string, string> tasks = new Dictionary<string, string>
    {
        {"neighborTalk", "notDone"},
        {"goBag", "notDone"},
        {"cutLawn", "notDone"},
        {"cutTree", "notDone"},
        {"cleanGutters", "notDone"},
        {"makeBreakfast", "notDone"},
        {"moveCar", "notDone"}
    };

    public void DoTask(string taskName)
    {
        tasks[taskName] = "done";
        UseUpTime();
    }

    public bool taskDone(string taskName)
    {
        return tasks[taskName] != "notDone";
    }

    public void UseUpTime()
    {
        switch (currentPhase)
        {
            case "Fire Season Starting":
                currentPhase = "Red Flag Day 1";
                break;
            case "Red Flag Day 1":
                currentPhase = "Red Flag Day 2";
                break;
            case "Red Flag Day 2":
                currentPhase = "Evacuation Warning!";
                break;
            case "Evacuation Warning!":
                EW_EventSystem.LeaveStoryNodeEvent += TimesUp;
                break;
            default:
                break;
        }

        uiManager.UpdatePhaseText(currentPhase);
    }

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

        //if node 2000 visited(ran out of time/didn't hit escape in time)
        //  EW_DialogueLine line1 = new EW_DialogueLine("Paul used his Pool as a last ditch escape method. Later, he was taken by medical crews to a local rendezvous point at the highschool where he was treated for injuries");
        //else (means they did some taks and hit escape at decent time which is GOOD)
        //  if backed car into driveway
        //      EW_DialogueLine line1 = new EW_DialogueLine("With his car ready to go, Paul sped off to a local church where he was examined by local EMTS");
        //  else if talked to aldo(didn't back car in + did some tasks)
        //      EW_DialogueLine line1 = new EW_DialogueLine("Paul runs over to Aldo's task asking if he can join them in escaping. They all quickly exit Marin county to stay at Aldo's parents vacation home in Napa, far from the fire.");
        //  else
        //      if(mow lawn)
        //          EW_DialogueLine line1 = new EW_DialogueLine("Having cut the lawn earlier, Paul is able to use an escape trail down the road from his property. He is met by firefighters who assist him evacuating.");
        //      else if(cut tree and gutter)
        //          EW_DialogueLine line1 = new EW_DialogueLine("Having cut the tree and gutter before a red flag day, Paul is able to escape through the back of his property. He is picked up by a firetruck while walking down the auxilary trail.");               
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