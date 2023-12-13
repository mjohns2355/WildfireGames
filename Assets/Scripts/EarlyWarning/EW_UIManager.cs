using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EW_UIManager : MonoBehaviour
{
    private Image dialogueBox, nameplate;
    [SerializeField]
    private TextMeshProUGUI dialogueTextComponent, nameplateTextComponent;

    private float timeBetweenLetters = 0.04f;
    private EW_DialogueLine curLine;
    [HideInInspector]
    public Coroutine typingCoroutine = null;

    [HideInInspector]
    public List<Button> choiceButtons = new List<Button>();
    [SerializeField]
    private GameObject buttonPrefab;
    private List<GameObject> storyButtons;

    [SerializeField]
    private EW_SceneManager sceneManager;
    public TextMeshProUGUI timerText;
    private Image warningIcon;
    private List<EW_Choice> selectedChoices;
    [SerializeField]
    private GameObject taskList, resetButton, mainMenuButton;

    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float landscapeOrthographicSize = 5f, portraitOrthographicSize = 9f;

    public void Start()
    {
        dialogueBox = GameObject.Find("DialogueBox").GetComponent<Image>();
        nameplate = GameObject.Find("Nameplate").GetComponent<Image>();
        warningIcon = GameObject.Find("WarningIcon").GetComponent<Image>();
        warningIcon.sprite = Resources.Load<Sprite>("EarlyWarning/Art/FireSZN");
        warningIcon.enabled = false;
        dialogueBox.enabled = false;
        dialogueTextComponent.text = "";
        timerText.text = "";
        selectedChoices = new List<EW_Choice>();
        taskList.SetActive(false);

        resetButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            sceneManager.Reset();
            warningIcon.sprite = Resources.Load<Sprite>("EarlyWarning/Art/FireSZN");
            HideUI();
            CreateStoryButtons(EW_SceneManager.storyNames);
        });

        mainMenuButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            // MJ: Go to main menu
        });

        EW_StoryFunctions.uiManager = this;

        EW_EventSystem.TriggerDialogueEvent += BeginDialogue;
        EW_EventSystem.ChoiceSetupEvent += SetupChoices;
        EW_EventSystem.LeaveStoryNodeEvent += HideNameplate;
        HideNameplate();
        CreateStoryButtons(EW_SceneManager.storyNames);
    }

    public void Update()
    {
        // Each frame, checks the current screen orientation and scales the background image accordingly
        mainCamera.orthographicSize =
            (Screen.width > Screen.height) ? landscapeOrthographicSize : portraitOrthographicSize;
    }

    public void HideUI()
    {
        taskList.SetActive(false);
        warningIcon.enabled = false;
        dialogueBox.enabled = false;
        dialogueTextComponent.text = "";
        HideNameplate();
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        timerText.text = "";
    }

    public void ShowTaskList()
    {
        taskList.SetActive(true);
        string text = "Tasks Completed:\n\n";
        text += "Mow the lawn:" + (sceneManager.TaskDone("cutLawn") ? "YES" : "NO") + "\n";
        text += "Cut the tree:" + (sceneManager.TaskDone("cutTree") ? "YES" : "NO") + "\n";
        text += "Cleaned the gutters: " + (sceneManager.TaskDone("cleanGutters") ? "YES" : "NO") + "\n";
        text += "Prepared Go Bag: " + (sceneManager.TaskDone("goBag") ? "YES" : "NO") + "\n";
        taskList.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void CreateStoryButtons(string[] names)
    {
        storyButtons = new List<GameObject>();
        Transform parentTransform = GameObject.Find("StoryButtonParent").transform;
        foreach (var name in names)
        {
            GameObject button = Instantiate(buttonPrefab, parentTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = name;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                sceneManager.storySelected = true;
                string storyPath = "EarlyWarning/StoryJSONs/" + name;
                storyButtons.ForEach(b => Destroy(b));
                sceneManager.curNode = EW_StoryParser.Parse(storyPath, sceneManager);
                sceneManager.curNode.Enter();
                sceneManager.curNode.Play();
            });
            storyButtons.Add(button);
        }
    }

    public void SetupChoices(List<EW_Choice> choices)
    {
        if (choiceButtons.Count != 0)
        {
            return;
        }
        dialogueBox.enabled = false;
        dialogueTextComponent.text = "";

        choices.RemoveAll(choice => selectedChoices.Contains(choice));

        for (int i = 0; i < choices.Count; i++)
        {
            CreateChoiceButton(choices[i], i, choices.Count);
        }
    }

    private void CreateChoiceButton(EW_Choice choice, int buttonIndex, int numChoices)
    {
        GameObject choiceButtonObject = Instantiate(buttonPrefab, dialogueBox.transform);
        Button choiceButton = choiceButtonObject.GetComponent<Button>();

        // Calculate and set position and size of the button
        // This makes sure the buttons are evenly spaced vertically and fill the dialogue box
        var boxRect = dialogueBox.GetComponent<RectTransform>().rect;
        float buttonHeight = boxRect.height / numChoices;
        float buttonWidth = boxRect.width;
        float yOffset = -buttonIndex * buttonHeight - (buttonHeight * 0.5f);

        RectTransform buttonRect = choiceButtonObject.GetComponent<RectTransform>();

        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        buttonRect.anchoredPosition = new Vector2(0f, yOffset);

        // Add the text
        TMP_Text buttonTextComponent = choiceButtonObject.GetComponentInChildren<TMP_Text>();
        buttonTextComponent.text = choice.text;

        // Set up the button to move to the appropriate node
        choiceButton.onClick.AddListener(() =>
        {
            RemoveChoiceButtons();
            if (!choice.repeatable)
            {
                // If the choice is not repeatable, note that we have selected it
                selectedChoices.Add(choice);
            }
            EW_EventSystem.InvokeChangeStoryNodeEvent(choice.goesTo);
            sceneManager.curNode.Play();
        });

        choiceButtons.Add(choiceButton);
    }

    // Clears the choice buttons from the screen
    public void RemoveChoiceButtons()
    {
        foreach (Button button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    public void BeginDialogue(EW_DialogueLine line)
    {
        curLine = line;
        dialogueBox.enabled = true;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    public void SkipTyping()
    {
        if (!curLine.important)
        {
            dialogueTextComponent.text = curLine.text;
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    private void ShowNameplate()
    {
        if (curLine.speaker == "" || curLine.speaker == null)
        {
            HideNameplate();
        }
        else
        {
            nameplate.enabled = true;
            nameplateTextComponent.text = curLine.speaker;
        }
    }

    private void HideNameplate()
    {
        nameplate.enabled = false;
        nameplateTextComponent.text = "";
    }

    //Writes out the text one letter at a time
    private IEnumerator TypeTextCoroutine()
    {
        dialogueTextComponent.text = "";
        dialogueTextComponent.color = Color.black;
        ShowNameplate();
        float messageLetterDelay = timeBetweenLetters;
        if (curLine.important)
        {
            dialogueTextComponent.color = new Color(0.7f, 0.0f, 0.0f);
            messageLetterDelay *= 1.1f;
        }

        for (int i = 0; i < curLine.text.Length; i++)
        {
            dialogueTextComponent.text += curLine.text[i];

            // Wait for a short duration to control the typing speed
            yield return new WaitForSeconds(messageLetterDelay);
        }

        typingCoroutine = null;
    }

    public void UpdatePhaseText(string text)
    {
        warningIcon.enabled = true;
        if (text == "Red Flag Day" || text == "Evacuation Warning!")
        {
            if (text == "Red Flag Day")
            {
                warningIcon.sprite = Resources.Load<Sprite>("EarlyWarning/Art/RedFlagIcon");
            }
            else
            {
                warningIcon.sprite = Resources.Load<Sprite>("EarlyWarning/Art/EvacuationIcon");
            }
            timerText.color = new Color(0.7f, 0.0f, 0.0f);
        }
        else
        {
            timerText.color = Color.black;
        }
        timerText.text = "Phase: " + text;
    }
}
