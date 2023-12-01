using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EW_UIManager : MonoBehaviour
{
    [SerializeField]
    private Image dialogueBox, nameplate;
    [SerializeField]
    private TextMeshProUGUI dialogueTextComponent, nameplateTextComponent;

    private float timeBetweenLetters = 0.04f;
    private EW_DialogueLine curLine;
    public Coroutine typingCoroutine = null;

    private List<Button> choiceButtons = new List<Button>();
    [SerializeField]
    private GameObject buttonPrefab;
    private List<GameObject> storyButtons;

    [SerializeField]
    private EW_SceneManager sceneManager;
    public TextMeshProUGUI timerText;
    private List<EW_Choice> selectedChoices;
    [SerializeField]
    private GameObject taskList;

    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float landscapeOrthographicSize = 5f;
    [SerializeField]
    private float portraitOrthographicSize = 9f;

    public void Start()
    {
        dialogueBox.enabled = false;
        dialogueTextComponent.text = "";
        selectedChoices = new List<EW_Choice>();
        taskList.SetActive(false);

        EW_StoryFunctions.uiManager = this;

        EW_EventSystem.TriggerDialogueEvent += BeginDialogue;
        EW_EventSystem.ChoiceSetupEvent += SetupChoices;
        EW_EventSystem.LeaveStoryNodeEvent += HideNameplate;
        HideNameplate();
    }

    public void Update()
    {
        if (Screen.width > Screen.height)
        {
            mainCamera.orthographicSize = landscapeOrthographicSize;
        }
        else
        {
            mainCamera.orthographicSize = portraitOrthographicSize;
        }
    }

    public void ShowTaskList()
    {
        taskList.SetActive(true);
        taskList.GetComponent<TextMeshProUGUI>().text = GetTaskListText();
    }

    private string GetTaskListText()
    {
        string text = "Tasks Completed:\n\n";
        text += "Cleaned the yard:" + (EW_SceneManager.cutLawn ? "YES" : "NO") + "\n";
        text += "Cut the tree:" + (EW_SceneManager.cutTree ? "YES" : "NO") + "\n";
        text += "Cleaned the gutters: " + (EW_SceneManager.cleanedGutters ? "YES" : "NO") + "\n";
        text += "Made breakfast: " + (EW_SceneManager.madeBreakfast ? "YES" : "NO") + "\n";
        return text;
    }

    public void CreateStoryButtons(string[] names)
    {
        storyButtons = new List<GameObject>();
        Debug.Log("Creating story buttons");

        for (int i = 0; i < names.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, GameObject.Find("StoryButtonParent").transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = names[i];
            int buttonIndex = i;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                sceneManager.storySelected = true;
                string storyPath = "Assets/Scripts/EarlyWarning/" + names[buttonIndex] + ".json";
                storyButtons.ForEach(b => Destroy(b));
                EW_SceneManager.curNode = EW_StoryParser.Parse(storyPath, sceneManager);
                EW_SceneManager.curNode.Enter();
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
        Debug.Log("Setting up choices");
        dialogueBox.enabled = false;
        dialogueTextComponent.text = "";

        for (int i = 0; i < choices.Count; i++)
        {
            if (selectedChoices.Contains(choices[i]))
            {
                choices.RemoveAt(i);
                i--;
            }
        }
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
        var boxRect = dialogueBox.GetComponent<RectTransform>().rect;
        float buttonHeight = boxRect.height / numChoices;
        float buttonWidth = boxRect.width;
        float yOffset = -buttonIndex * buttonHeight - (buttonHeight * 0.5f);

        RectTransform buttonRect = choiceButtonObject.GetComponent<RectTransform>();

        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        buttonRect.anchoredPosition = new Vector2(0f, yOffset);

        // Add the text and function
        TMP_Text buttonTextComponent = choiceButtonObject.GetComponentInChildren<TMP_Text>();
        if (buttonTextComponent != null)
        {
            Debug.Log("Setting button text to " + choice.text);
            buttonTextComponent.text = choice.text;
        }

        choiceButton.onClick.AddListener(() =>
        {
            RemoveChoiceButtons();
            if (!choice.repeatable)
            {
                selectedChoices.Add(choice);
            }
            EW_EventSystem.InvokeChangeStoryNodeEvent(choice.goesTo);
        });

        choiceButtons.Add(choiceButton);
    }

    private void RemoveChoiceButtons()
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
        ShowNameplate();
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    public void SkipTyping()
    {
        dialogueTextComponent.text = curLine.text;
        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private void ShowNameplate()
    {
        if (curLine.speaker == "")
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

    private IEnumerator TypeTextCoroutine()
    {
        dialogueTextComponent.text = ""; // Clear the text

        for (int i = 0; i < curLine.text.Length; i++)
        {
            dialogueTextComponent.text += curLine.text[i]; // Add one character at a time

            // Wait for a short duration to control the typing speed
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        typingCoroutine = null;
    }

    public void updateTimer(int minutesRemaining)
    {
        timerText.text = "Time left: " + minutesRemaining + " minutes";
    }
}
