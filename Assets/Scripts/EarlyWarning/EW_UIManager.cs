using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void Start()
    {
        dialogueBox.enabled = false;
        textComponent.text = "";

        EW_EventSystem.TriggerDialogueEvent += BeginDialogue;
        EW_EventSystem.ChoiceSetupEvent += SetupChoices;
    }

    public void SetupChoices(List<EW_Choice> choices)
    {
        dialogueBox.enabled = false;
        textComponent.text = "";

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
            buttonTextComponent.text = choice.text;
        }

        choiceButton.onClick.AddListener(() =>
        {
            RemoveChoiceButtons();
            choice.onSelect.Invoke();
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

    public void BeginDialogue(string textToType)
    {
        curTargetText = textToType;
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
}
