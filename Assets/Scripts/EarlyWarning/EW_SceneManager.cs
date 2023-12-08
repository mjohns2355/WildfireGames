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

            int nextNode = curNode.Advance();
            ChangeStoryNode(nextNode);
        }
    }

    public void TimesUp()
    {
        EW_EventSystem.LeaveStoryNodeEvent -= TimesUp;
        ChangeStoryNode(2000);
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
        uiManager.ShowTaskList();
    }

    public void ChangeStoryNode(int nodeIndex)
    {
        if (nodeIndex == -1)
        {
            EndPrefirePhase();
            return;
        }
        if (curNode != nodeDict[nodeIndex])
        {
            if (done) { return; }
            curNode = nodeDict[nodeIndex];
            EW_EventSystem.InvokeLeaveStoryNodeEvent();
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