using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ATC_HouseDialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button[] optionButtons; // Buttons for responses
    [SerializeField] private Button nextButton;
    [SerializeField] private ATC_DialogTree currentDialogTree;
    [SerializeField] private DialogNode currentNode;
    [SerializeField] private int paragraphIndex;
    [SerializeField] private HouseType houseType;

    public void StartHouseDialog(HouseType type, ATC_DialogTree dialogTree)
    {
        GameManager.Instance.currentStage = LevelStage.HouseDialog;
        currentDialogTree = dialogTree;
        currentNode = currentDialogTree.rootNode;
        paragraphIndex = 0;
        houseType = type;
        DisplayCurrentParagraph();
    }

    private void DisplayCurrentParagraph()
    {
        if (paragraphIndex < currentNode.messages.Length)
        {
            dialogText.text = currentNode.messages[paragraphIndex];
            paragraphIndex++;
            //nextButton.gameObject.SetActive(paragraphIndex < currentNode.messages.Length);
            nextButton.onClick.AddListener(DisplayCurrentParagraph); // Assign the next action
        }
        else
        {
            //nextButton.gameObject.SetActive(false);
            SetupOptions(currentNode.options);
        }
    }

    private void SetupOptions(DialogOption[] options)
    {
        Debug.Log("Show Options");
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i].optionText;
                int optionIndex = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(options[optionIndex]));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnOptionSelected(DialogOption selectedOption)
    {
        foreach (var button in optionButtons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
        nextButton.onClick.RemoveAllListeners();

        if (selectedOption.isEndNode)
        {
            EndDialog();
        }
        else
        {
            currentNode = selectedOption.nextNode;
            paragraphIndex = 0; // Reset for the next node
            DisplayCurrentParagraph();
        }
    }

    private void EndDialog()
    {
        Debug.Log("House dialog completed");
        ATC_UIController.Instance.PopPanel();
        ATC_UIController.Instance.FindMenu(houseType).OnMenuEnable();
        // Optionally signal to ATC_dialogManager that the house dialog is complete
    }
}
