using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using DG.Tweening;
public class ATC_HouseDialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject messageBubblePrefab;
    [SerializeField] private Transform messagebBubblesContainer;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button[] optionButtons; // Buttons for responses
    [SerializeField] private List<FC_MessageBubble> optionMessageBubbles = new List<FC_MessageBubble>();
    private Dictionary<string, ATC_DialogTree> dialogTreeMap;
    [SerializeField] private ATC_DialogTree currentDialogTree;
    private DialogNode currentNode;
    [SerializeField] private int paragraphIndex;
    [SerializeField] private string key;
    [SerializeField] private Button proceedButton;

    public Action OnDialogueComplete;
    //[SerializeField] GameObject nameTag;
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
            //Debug.Log($"Loaded JSON: {json}");
            DialogTreeCollection collection = JsonUtility.FromJson<DialogTreeCollection>(json);

            dialogTreeMap = new Dictionary<string, ATC_DialogTree>();
            foreach (var dialogTree in collection.dialogTrees)
            {
                dialogTreeMap[dialogTree.houseType] = dialogTree;
                //Debug.Log($"Loaded dialog tree for houseType: {dialogTree.houseType}");
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

        StartCoroutine(DisplayNodeWithDelay());

    }
    private IEnumerator DisplayNodeWithDelay()
    {
        // Initial delay for the first message
        if (currentNode.id == currentDialogTree.rootNodeId)
        {
            yield return new WaitForSeconds(3.0f);
            // Create a message bubble after the delay
        }
        float delayTime = 0.5f + (currentNode.dialogText.Length * 0.05f);
      

         // Wait before showing options or next message
                                                    
        FC_MessageBubble messageBubble = SpawnAMessageBubble(currentNode.dialogText, currentNode.characterName, false, false);

        if (!string.IsNullOrEmpty(currentNode.portraitPath))
        {
            Sprite portrait = Resources.Load<Sprite>(currentNode.portraitPath);
            characterPortrait.gameObject.SetActive(true);
            characterPortrait.sprite = portrait;
        }



        yield return new WaitForSeconds(delayTime);
        ShowOptions();
        // Click to end dialog

    }

    private void OnOptionSelected(int optionIndex)
    {
        // destroy all option message bubbles
        foreach (var option in optionMessageBubbles)
        {
            Destroy(option.gameObject);
        }
        optionMessageBubbles.Clear();


        //instantiate new message bubble (use message text if option text and message text is different)
        string text = string.Empty;
        var selectedOption = currentNode.options[optionIndex];
        if (!string.IsNullOrEmpty(selectedOption.messageText))
        {
            text = currentNode.options[optionIndex].messageText;
        }
        else
        {
            text = selectedOption.optionText;
        }

        SpawnAMessageBubble(text, "", true, false);

        if (currentNode.isEndNode)
        {
            DOVirtual.DelayedCall(2.0f, () =>
            {
                EndDialog();
            });

            return;
        }
        // show next node with delay
        string nextNodeId = currentNode.options[optionIndex].nextNodeId;
        // Find the next node
        currentNode = currentDialogTree.GetNodeById(nextNodeId);
        DOVirtual.DelayedCall(1f, () =>
        {
            DisplayCurrentNode();
        });


        //DisplayCurrentNode();
        //HideOptions();
    }

    private void ShowOptions()
    {
        //skip empty options
        if (currentNode.options[0].optionText == "Continue")
        {
            string nextNodeId = currentNode.options[0].nextNodeId;
            // Find the next node
            currentNode = currentDialogTree.GetNodeById(nextNodeId);
            DisplayCurrentNode();
            return;
        }

        for (int i = 0; i < currentNode.options.Length; i++)
        {
            var index = i;
            var optionBubble = SpawnAMessageBubble(currentNode.options[i].optionText, "", false, true);

            // Add click listener
            optionBubble.messageBox.onClick.AddListener(() =>
            {
                foreach (var option in optionMessageBubbles)
                {
                    if (option != optionBubble)
                    {
                        option.gameObject.SetActive(false);
                    }
                }

                optionBubble.sendButton.SetActive(false);

                Sequence sequence = DOTween.Sequence();
                sequence.Append(optionBubble.transform.DOScale(0.8f, 0.1f).SetEase(Ease.InOutQuad));
                sequence.Append(optionBubble.transform.DOScale(1f, 0.1f).SetEase(Ease.InOutQuad));
                sequence.AppendInterval(1f);
                sequence.OnComplete(() =>
                {
                    OnOptionSelected(index);
                });
            });

        }
    }
    private void HideOptions()
    {
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    IEnumerator EndDialogWithDelay()
    {
        yield return new WaitUntil(() => !isWaitingForPlayer);
        EndDialog();
    }
    private void EndDialog()
    {

        //Debug.Log("House dialog completed");
        //ATC_UIController.Instance.PopPanel();
        ATC_UIController.Instance.HideDialog();
        ClearMessages();
        //isWaitingForPlayer = true;
        characterPortrait.gameObject.SetActive(false);
        OnDialogueComplete.Invoke();



        if (Enum.TryParse(key, out HouseType houseType))
        {
            ATC_UIController.Instance.FindMenu(houseType).OnMenuEnable();
        }


        //proceedButton.onClick.RemoveAllListeners();
        //proceedButton.gameObject.SetActive(false);
    }
    public FC_MessageBubble SpawnAMessageBubble(string message, string name, bool isSentByUser, bool isOption)
    {
        if (isOption)
        {
            var optionBubble = Instantiate(messageBubblePrefab, messagebBubblesContainer).GetComponent<FC_MessageBubble>();
            optionBubble.SetupOptionButton(message);
            optionMessageBubbles.Add(optionBubble);
            AnimateMessageBubble(optionBubble.GetComponent<CanvasGroup>());
            // pop in effect
            optionBubble.transform.localScale = Vector3.zero;
            optionBubble.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            return optionBubble;
        }

        var messageBubble = Instantiate(messageBubblePrefab, messagebBubblesContainer).GetComponent<FC_MessageBubble>();
        messageBubble.SetupMessage(message, name, isSentByUser);
        AnimateMessageBubble(messageBubble.GetComponent<CanvasGroup>());

        return messageBubble;
    }

    private void AnimateMessageBubble(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutBack);
        ScrollToBottom();
        //bubbleTransform.localScale = Vector3.zero;
    }

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.DOVerticalNormalizedPos(0f, 0.3f);
    }

    public void ResetScrollPosition()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
    }

    public void ClearMessages()
    {
        for (int i = 0; i < messagebBubblesContainer.childCount; i++)
        {
            Destroy(messagebBubblesContainer.GetChild(i).gameObject);
        }
        optionMessageBubbles.Clear();
        characterPortrait.gameObject.SetActive(false);
        ResetScrollPosition();
    }

}
