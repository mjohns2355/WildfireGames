using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
public class ATC_HouseDialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Button[] optionButtons; // Buttons for responses
    private Dictionary<string, ATC_DialogTree> dialogTreeMap;
    [SerializeField] private ATC_DialogTree currentDialogTree;
    [SerializeField] private DialogNode currentNode;
    [SerializeField] private int paragraphIndex;
    [SerializeField] private string key;
    [SerializeField] private Button nextButton;
    [SerializeField] GameObject nameTag;
    private bool isWaitingForPlayer = true;

    private void Start()
    {
        LoadDialogTrees("Assets/Resources/AlertTheCity/HouseDialogs.json");
        
    }
    public void LoadDialogTrees(string jsonFilePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("AlertTheCity/HouseDialogs");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found in Resources!");
        }
        else
        {
            string json = jsonFile.text;
            Debug.Log($"Loaded JSON: {json}");
            DialogTreeCollection collection = JsonUtility.FromJson<DialogTreeCollection>(json);

            dialogTreeMap = new Dictionary<string, ATC_DialogTree>();
            foreach (var dialogTree in collection.dialogTrees)
            {
                dialogTreeMap[dialogTree.houseType] = dialogTree;
                Debug.Log($"Loaded dialog tree for houseType: {dialogTree.houseType}");
            }
            Debug.Log($"Number of dialog trees loaded: {dialogTreeMap.Values.Count}");
        }

    }
    public void StartDialog(string key)
    {
        this.key = key;
        //nextButton.onClick.RemoveAllListeners();
        if (dialogTreeMap.TryGetValue(key, out currentDialogTree))
        {
            currentNode = currentDialogTree.GetNodeById(currentDialogTree.rootNodeId);
            DisplayCurrentNode();
        }
        else
        {
            Debug.LogError($"Dialog tree for '{key}' not found.");
        }
    }

    private void DisplayCurrentNode()
    {
        characterNameText.text = currentNode.characterName;
        dialogText.text = currentNode.dialogText;
        if(!string.IsNullOrEmpty(currentNode.portraitPath))
        {
            //Debug.Log($"Portrait Path: {currentNode.portraitPath}");
            Sprite portrait = Resources.Load<Sprite>(currentNode.portraitPath);
            characterPortrait.gameObject.SetActive(true);
            characterPortrait.sprite = portrait;
            
        }

        // Click to end dialog
        if (currentNode.isEndNode)
        {
            nextButton.onClick.AddListener(() =>
            {
                isWaitingForPlayer = false;
            });
            StartCoroutine(EndDialogWithDelay());
            return;
        }
        ShowOptions();
    }
    private void OnOptionSelected(int optionIndex)
    {
        string nextNodeId = currentNode.options[optionIndex].nextNodeId;
        // Find the next node
        currentNode = currentDialogTree.GetNodeById(nextNodeId);
        DisplayCurrentNode();
        HideOptions();
    }

    private void ShowOptions()
    {    
        StartCoroutine(ShowOptionsWithDelay(3.0f)); 
    }
    private void HideOptions()
    {
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
    private IEnumerator ShowOptionsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < currentNode.options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentNode.options[i].optionText;

                int optionIndex = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }
    IEnumerator EndDialogWithDelay()
    {
        yield return new WaitUntil(() => !isWaitingForPlayer);
        EndDialog();
    }
    private void EndDialog()
    {
        Debug.Log("House dialog completed");
        //ATC_UIController.Instance.PopPanel();
        ATC_UIController.Instance.HideDialog();
        characterPortrait.gameObject.SetActive(false);
        if (Enum.TryParse(key, out HouseType houseType))
        {
            ATC_UIController.Instance.FindMenu(houseType).OnMenuEnable();
        }
        nextButton.onClick.RemoveAllListeners();
    }
}
