using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EW_UIManager : MonoBehaviour
{
    [SerializeField]
    private Image dialogueBox;
    [SerializeField]
    private TextMeshProUGUI textComponent;

    private float timeBetweenLetters = 0.04f;
    private string curTargetText;
    public Coroutine typingCoroutine = null;

    private List<Button> choiceButtons = new List<Button>();
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]

    private EW_SceneManager sceneManager;
    public TextMeshProUGUI timerText;
    private List<EW_Choice> selectedChoices;

    public void Start()
    {
        dialogueBox.enabled = false;
        textComponent.text = "";
        selectedChoices = new List<EW_Choice>();

        EW_StoryFunctions.uiManager = this;

        EW_EventSystem.TriggerDialogueEvent += BeginDialogue;
        EW_EventSystem.ChoiceSetupEvent += SetupChoices;
    }

    void CreateStoryButton(string name, int x)
    {
        string storyPath = "Assets/Scripts/EarlyWarning/{name}";
        Debug.Log(storyPath);

        GameObject button = Instantiate(buttonPrefab, buttonPrefab.transform.parent);
        button.GetComponentInChildren<TextMeshProUGUI>().text = name;
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
            EW_StoryParser.Parse(storyPath, sceneManager);
        });
    }

    public void SetupChoices(List<EW_Choice> choices)
    {
        if (choiceButtons.Count != 0)
        {
            return;
        }
        Debug.Log("Setting up choices");
        dialogueBox.enabled = false;
        textComponent.text = "";

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
        curTargetText = line.text;
        dialogueBox.enabled = true;
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    public void SkipTyping()
    {
        textComponent.text = curTargetText;
        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private IEnumerator TypeTextCoroutine()
    {
        textComponent.text = ""; // Clear the text

        for (int i = 0; i < curTargetText.Length; i++)
        {
            textComponent.text += curTargetText[i]; // Add one character at a time

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
