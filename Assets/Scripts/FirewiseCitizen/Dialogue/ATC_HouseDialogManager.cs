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
    public GameObject messageBubblePrefab,topFade,bottomFade;
    public Transform messagebBubblesContainer;
    [Range(0.01f, 0.05f)]
    public float waitTimePerCharacter = 0.01f;
    [Range(0f, 2f)]
    public float baseWaitTime = 1;
    public ScrollRect scrollRect;
    //[SerializeField] private Button[] optionButtons; // Buttons for responses
    private List<FC_MessageBubble> optionMessageBubbles = new List<FC_MessageBubble>();
    private Dictionary<string, ATC_DialogTree> dialogTreeMap;
    [SerializeField] private ATC_DialogTree currentDialogTree;
    [SerializeField] private DialogNode currentNode;
    //[SerializeField] private int paragraphIndex;
    private string key;
    public Button skipButton;

    public Action OnDialogueComplete;
    public Action<DialogNode> OnDialogueNodeDisplayed;
    public Action<DialogOption> OnDialogueOptionSelected;
    //[SerializeField] GameObject nameTag;
    public bool isWaitingForPlayer =  true;
    public bool canShowSkipButton = false;
    private bool canClick = false;
    private CanvasGroup topEdgeFade;

    private void Start()
    {
        LoadDialogTrees("Assets/Resources/FirewiseCitizen/HouseDialogs.json");
        skipButton.onClick.AddListener(SkipDialogue);
        scrollRect.onValueChanged.AddListener(_ => UpdateEdgeFadeVisibility());
        topEdgeFade = topFade.GetComponent<CanvasGroup>();
    }

    private void UpdateEdgeFadeVisibility()
    {
        if (scrollRect == null || topFade == null || bottomFade == null) return;

        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        var scrollEnabled = contentHeight > viewportHeight;
        if (!scrollEnabled) return;
        float pos = scrollRect.verticalNormalizedPosition;

        float topTargetAlpha = (pos < 0.9f) ? 1f : 0f;

        topEdgeFade?.DOFade(topTargetAlpha, 0.2f);
    }

    public void LoadDialogTrees(string jsonFilePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("FirewiseCitizen/HouseDialogs");
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
    private void Update()
    {
        // click through dialogue
        if (isWaitingForPlayer && canClick && Input.GetMouseButtonDown(0))
        {
            canClick = false;
            DOVirtual.DelayedCall(baseWaitTime, () => canClick = true); //cooldown

            isWaitingForPlayer = false;
            ProceedToNextNode();
        }
    }

    private void ProceedToNextNode()
    {
        if (currentNode.options != null && currentNode.options.Length > 0 && currentNode.options[0].optionText != "Continue")
        {
           ShowOptions();
        }
        if (!currentNode.isEndNode && currentNode.options.Length == 1 && currentNode.options[0].optionText == "Continue")
        {
            //if (currentDialogTree.GetNodeById(currentNode.options[0].nextNodeId).id == currentNode.id) return;
            
            currentNode = currentDialogTree.GetNodeById(currentNode.options[0].nextNodeId);
            DisplayCurrentNode();
        }
        //else if(currentNode.isEndNode)
        //{
            
        //    DOVirtual.DelayedCall(0.1f,EndDialog).SetId(gameObject);
        //}
    }

    public void SkipDialogue()
    {
        EndDialog();
    }

    // inDialogue means start dialog it the middle of another dialogue
    public void StartDialogue(string key, bool inDialogue = false)
    {
        topEdgeFade.alpha = 0;
        this.key = key;
        //nextButton.onClick.RemoveAllListeners();
        if (dialogTreeMap.TryGetValue(key, out currentDialogTree))
        {
            currentNode = currentDialogTree.GetNodeById(currentDialogTree.rootNodeId);
            if (inDialogue) return;
            isWaitingForPlayer = false;
            canClick = false;
            DOVirtual.DelayedCall(1f, () =>
            {
                DisplayCurrentNode();
                canClick = true;
            });

        }
        else
        {
            Debug.LogError($"Dialog tree for '{key}' not found.");
        }

    }

    private void DisplayCurrentNode()
    {

        OnDialogueNodeDisplayed?.Invoke(currentNode);


        if (string.IsNullOrEmpty(currentNode.characterName))
        {
            SpawnAMessageBubble(currentNode.dialogText, null, false, false, true);
        }
        else if (currentNode.characterName == "Player")
        {
            SpawnAMessageBubble(currentNode.dialogText, null, true, false, false);
        }
        else
        {
            SpawnAMessageBubble(currentNode.dialogText, currentNode.characterName, false, false, false);
        }

        if (!string.IsNullOrEmpty(currentNode.portraitPath))
        {
            Sprite portrait = Resources.Load<Sprite>(currentNode.portraitPath);
            characterPortrait.gameObject.SetActive(true);
            characterPortrait.sprite = portrait;
        }
        canClick = false;

        if (currentNode.options == null || currentNode.options[0].optionText == "Continue" )
        {
            canClick = true;
            isWaitingForPlayer = true;
            if (currentNode.isEndNode)
            {
                DOVirtual.DelayedCall(2f, EndDialog).SetId(gameObject);
            }
        }
        else
        {
            //DOVirtual.DelayedCall(2f, () =>
            //{
            //    ShowOptions();
            //    isWaitingForPlayer = true; // Only allow input after the node is fully ready
            //});
            isWaitingForPlayer = true;
            canClick = true;
        }



        //StartCoroutine(DisplayNodeWithDelay());
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

        SpawnAMessageBubble(text, null, true, false,false);
        OnDialogueOptionSelected?.Invoke(selectedOption);

        //jump to end if it node is an end node
        if (currentNode.isEndNode)
        {
            DOVirtual.DelayedCall(0.2f, () =>
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
        
        DOVirtual.DelayedCall(1f, () =>
        {
            isWaitingForPlayer = true;
            canClick = true;
            DisplayCurrentNode();
        });
        
    }
    private void ShowOptions()
    {
       
        for (int i = 0; i < currentNode.options.Length; i++)
        {
            var index = i;
            var optionBubble = SpawnAMessageBubble(currentNode.options[i].optionText, null, false, true,false);

            // Add click listener
            optionBubble.messageBox.onClick.AddListener(() =>
            {
                OnOptionSelected(index);
            });

        }
    }

    public void EndDialog()
    {
        StopAllCoroutines();
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

    }
    public FC_MessageBubble SpawnAMessageBubble(string message, string name, bool isSentByUser, bool isOption,bool isDescription)
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

        if (isDescription)
        {
            var bubble = Instantiate(messageBubblePrefab, messagebBubblesContainer).GetComponent<FC_MessageBubble>();
            bubble.SetupDescription(message);
            AnimateMessageBubble(bubble.GetComponent<CanvasGroup>());
            return bubble;
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
