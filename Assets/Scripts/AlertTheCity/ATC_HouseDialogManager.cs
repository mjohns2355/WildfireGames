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
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI characterNameText;
    public Image characterPortrait;
    public GameObject messageBubblePrefab;
    public Transform messagebBubblesContainer;
    [Range(0.01f, 0.05f)]
    public float waitTimePerCharacter = 0.01f;
    [Range(0f, 0.5f)]
    public float baseWaitTime = 0.2f;
    public ScrollRect scrollRect;
    //[SerializeField] private Button[] optionButtons; // Buttons for responses
    private List<FC_MessageBubble> optionMessageBubbles = new List<FC_MessageBubble>();
    private Dictionary<string, ATC_DialogTree> dialogTreeMap;
    private ATC_DialogTree currentDialogTree;
    private DialogNode currentNode;
    //[SerializeField] private int paragraphIndex;
    private string key;
    public Button skipButton;

    public Action OnDialogueComplete;
    public Action<DialogNode> OnDialogueNodeDisplayed;
    public Action<DialogOption> OnDialogueOptionSelected;
    //[SerializeField] GameObject nameTag;
    public bool isWaitingForPlayer =  true;
    public bool canShowSkipButton = false;

    private void Start()
    {
        LoadDialogTrees("Assets/Resources/AlertTheCity/HouseDialogs.json");
        skipButton.onClick.AddListener(SkipDialogue);
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

    public void SkipDialogue()
    {
        EndDialog();
    }

    // inDialogue means start dialog it the middle of another dialogue
    public void StartDialogue(string key, bool inDialogue = false)
    {
        this.key = key;
        //nextButton.onClick.RemoveAllListeners();
        if (dialogTreeMap.TryGetValue(key, out currentDialogTree))
        {
            currentNode = currentDialogTree.GetNodeById(currentDialogTree.rootNodeId);
            if (inDialogue) return;
            DisplayCurrentNode();
        }
        else
        {
            Debug.LogError($"Dialog tree for '{key}' not found.");
        }

    }

    private void DisplayCurrentNode()
    {
        OnDialogueNodeDisplayed(currentNode);
        StartCoroutine(DisplayNodeWithDelay());
    }
    private IEnumerator DisplayNodeWithDelay()
    {

        // Initial delay for the first message
        if (currentNode.id == currentDialogTree.rootNodeId)
        {
            yield return new WaitForSeconds(3.0f);
            skipButton.gameObject.SetActive(canShowSkipButton);
            // Create a message bubble after the delay
        }
        float delayTime = baseWaitTime + currentNode.dialogText.Length * waitTimePerCharacter;
                                                    
        SpawnAMessageBubble(currentNode.dialogText, currentNode.characterName, false, false);

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
        OnDialogueOptionSelected?.Invoke(selectedOption);

        // jump to end if it node is an end node
        if (currentNode.isEndNode)
        {
            DOVirtual.DelayedCall(2.0f, () =>
            {
                EndDialog();
            }).SetId(gameObject);

            return;
        }
        float delayTime = baseWaitTime + text.Length * waitTimePerCharacter;

        StartCoroutine(OptionSelectedRoutine(delayTime,selectedOption));

        //DisplayCurrentNode();
        //HideOptions();
    }

    IEnumerator OptionSelectedRoutine(float delay, DialogOption selectedOption)
    {
        yield return new WaitUntil(() => isWaitingForPlayer == false);

        // show next node with delay
        string nextNodeId = selectedOption.nextNodeId;
        // Find the next node
        currentNode = currentDialogTree.GetNodeById(nextNodeId);
        isWaitingForPlayer = true;
        DOVirtual.DelayedCall(delay, () =>
        {
            DisplayCurrentNode();
        });
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
                //foreach (var option in optionMessageBubbles)
                //{
                //    if (option != optionBubble)
                //    {
                //        option.gameObject.SetActive(false);
                //    }
                //}

                //optionBubble.sendButton.SetActive(false);
                OnOptionSelected(index);
                //Sequence sequence = DOTween.Sequence();
                //sequence.Append(optionBubble.transform.DOScale(0.8f, 0.1f).SetEase(Ease.InOutQuad));
                //sequence.Append(optionBubble.transform.DOScale(1f, 0.1f).SetEase(Ease.InOutQuad));
                //sequence.AppendInterval(1f);
                //sequence.OnComplete(() =>
                //{
                //    OnOptionSelected(index);
                //});
            });

        }
    }
    //private void HideOptions()
    //{
    //    foreach (var button in optionButtons)
    //    {
    //        button.gameObject.SetActive(false);
    //    }
    //}

    //IEnumerator EndDialogWithDelay()
    //{
    //    yield return new WaitUntil(() => !isWaitingForPlayer);
    //    EndDialog();
    //}
    public void EndDialog()
    {
        StopAllCoroutines();
        //Debug.Log("House dialog completed");
        //ATC_UIController.Instance.PopPanel();
        ATC_UIController.Instance.HideDialog();
        ClearMessages();
        //isWaitingForPlayer = true;
        characterPortrait.gameObject.SetActive(false);
        OnDialogueComplete?.Invoke();
        DOTween.Kill(gameObject);


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
