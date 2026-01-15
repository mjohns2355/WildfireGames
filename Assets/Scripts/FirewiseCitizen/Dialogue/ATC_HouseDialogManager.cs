using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.Linq;
using Unity.VisualScripting;
public class ATC_HouseDialogManager : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI characterNameText;
    public Image characterPortrait;
    public GameObject messageBubblePrefab, topFade, bottomFade;
    public Transform messagebBubblesContainer;
    private AudioSource audioSource;
    [Range(0.01f, 0.05f)]
    public float waitTimePerCharacter = 0.01f;
    [Range(0f, 5f)]
    public float clickCooldown = 1f;
    public ScrollRect scrollRect;
    private List<FC_MessageBubble> optionMessageBubbles = new();
    private Dictionary<string, ATC_DialogTree> dialogTreeMap;
    // item2 is if the dialog is skipped
    public Dictionary<string, (DialogFlags,bool)> dialogFlagsMap;
    [SerializeField] private ATC_DialogTree currentDialogTree;
    [SerializeField] private DialogNode currentNode;
    private string key;
    public Button skipButton;

    public Action OnDialogueComplete;
    public Action<DialogNode> OnDialogueNodeDisplayed;
    public Action<DialogOption> OnDialogueOptionSelected;
    public bool isWaitingForPlayer = true;
    public bool canShowSkipButton = false;
    private bool canClick = false;
    private int optionsShown = 0;
    private CanvasGroup topEdgeFade;
    //[SerializeField]private List<string> currentPathIds = new();
    //private int currentPathIndex = 0;
    //private int finalPathLength = 1;
    private int choicesSelectedCount = 0;
    private const int totalChoices = 4;//fixed choice count for complete percent calculation
    public Dictionary<string, float> houseDialogCompletePercent = new();
    private void Start()
    {
        LoadDialogTrees("Assets/Resources/FirewiseCitizen/HouseDialogs.json");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        skipButton.onClick.AddListener(() => { 
            if(GameManager.Instance.currentStage != LevelStage.Tutorial)
            {
                EndDialog(true);
            }

            GameManager.Instance.cameraMovement.ResetCam();
        });
        scrollRect.onValueChanged.AddListener(_ => UpdateEdgeFadeVisibility());
        topEdgeFade = topFade.GetComponent<CanvasGroup>();
    }

    //private int CountNodesFrom(string nodeId)
    //{
    //    DialogNode node = currentDialogTree.GetNodeById(nodeId);
    //    if (node == null)
    //    {
    //        Debug.LogWarning($"CountNodesFrom: node '{nodeId}' not found!");
    //        return 0;
    //    }

    //    if (node.isEndNode || node.options == null || node.options.Length == 0 || node.options[0].optionText == null)
    //    {
    //        return 1;
    //    }

    //    string nextId = node.GetNextNodeId();

    //    return 1 + CountNodesFrom(nextId);
    //}
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
            foreach (var dialogTree in collection.dialogTrees)
            {
                foreach (var node in dialogTree.nodes)
                {
                    if (node.options != null)
                    {
                        foreach (var option in node.options)
                        {
                            option.parentId = node.id;
                        }
                    }
                }
            }
            dialogTreeMap = new Dictionary<string, ATC_DialogTree>();
            dialogFlagsMap = new Dictionary<string, (DialogFlags, bool)>();
            foreach (var dialogTree in collection.dialogTrees)
            {
                dialogTreeMap[dialogTree.houseType] = dialogTree;
                dialogFlagsMap[dialogTree.houseType] = (dialogTree.flags,false);
                //Debug.Log($"Loaded dialog tree for houseType: {dialogTree.houseType}");
            }
            //Debug.Log($"Number of dialog trees loaded: {dialogTreeMap.Values.Count}");
        }

    }
    private void Update()
    {
        // click through dialogue
        if (isWaitingForPlayer && canClick && Input.GetMouseButtonDown(0))
        {
            canClick = false;
            DOVirtual.DelayedCall(clickCooldown, () => canClick = true); //cooldown

            isWaitingForPlayer = false;
            ProceedToNextNode();
        }
    }
    private void ProceedToNextNode()
    {
        if (currentNode == null || currentDialogTree == null) return;
        if (currentNode.options != null && currentNode.options.Length > 0 && currentNode.options[0].optionText != null)
        {
            ShowOptions();
        }
        else if (!currentNode.isEndNode)
        {
            string nextId = currentNode.GetNextNodeId();
            DialogNode nextNode = currentDialogTree.GetNodeById(nextId);
   

            if (nextNode != null)
            {
                //currentPathIds.Add(nextId);
                //int remainingCount = CountNodesFrom(nextId);
                //finalPathLength = currentPathIds.Count + remainingCount;
                //currentPathIndex = currentPathIds.Count - 1;
                currentNode = nextNode;
                DisplayCurrentNode();
            }
        }
    }

    public void SkipDialogue()
    {
        // only choose to skip at the beginning of the dialog marked as skipped the whole dialog
        if(optionsShown == 1)
        {
            // is skipped = not sopken & choose to skip
            var t = dialogFlagsMap[key];
            t.Item2 = true;          // isSkipped = true
            dialogFlagsMap[key] = t;
        }

        EndDialog();
    }

    // inDialogue means start dialog it the middle of another dialogue
    public void StartDialogue(string key, bool inDialogue = false)
    {

        topEdgeFade.alpha = 0;
        optionsShown = 0;
        this.key = key;
        //nextButton.onClick.RemoveAllListeners();
        if (dialogTreeMap.TryGetValue(key, out currentDialogTree))
        {
            var t = dialogFlagsMap[key];
            t.Item2 = false;           // isSkipped = false
            dialogFlagsMap[key] = t;
            var val = GameManager.Instance.currentLevel > 0 ? 1 : 0;
            SetFlag("hasIncentives", val);
            choicesSelectedCount = 0;
            // reset current path
            //currentPathIds.Clear();
            //currentPathIndex = 0;
            //finalPathLength = 1;

            currentNode = currentDialogTree.GetNodeById(currentDialogTree.rootNodeId);
            //currentPathIds.Add(currentNode.id);

            if (inDialogue) return;
            ClearMessages();
            isWaitingForPlayer = false;
            canClick = false;
            DOVirtual.DelayedCall(1f, () =>
            {
                DisplayCurrentNode();
                canClick = true;
            })
                .SetId(gameObject)
                .OnComplete(() =>
            {
                SetSkipButton(true);
            });


        }
        else
        {
            Debug.LogError($"Dialog tree for '{key}' not found.");
        }

    }

    private void DisplayCurrentNode()
    {
        if (currentNode == null || !gameObject.activeInHierarchy) return;

        // cuts off audio playback if dialogue is skipped
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        OnDialogueNodeDisplayed?.Invoke(currentNode);

        // audio playback after node check - select path based on language
        string selectedAudioPath = currentNode.audioPath; // default to English

        if (LocalizationManager.CurrentLanguage == "es" && !string.IsNullOrEmpty(currentNode.audioPathES))
        {
            selectedAudioPath = currentNode.audioPathES;
        }

        if (!string.IsNullOrEmpty(selectedAudioPath) && TTSManager.IsEnabled)
        {
            AudioClip clip = Resources.Load<AudioClip>(selectedAudioPath);
            if (clip != null && audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Audio clip not found: {selectedAudioPath}");
            }
        }

        // Select dialogue text based on language
        string selectedDialogText = currentNode.dialogText; // default to English

        if (LocalizationManager.CurrentLanguage == "es" && !string.IsNullOrEmpty(currentNode.dialogTextES))
        {
            selectedDialogText = currentNode.dialogTextES;
        }

        if(string.IsNullOrEmpty(currentNode.characterName) && string.IsNullOrEmpty(selectedDialogText) && string.IsNullOrEmpty(currentNode.portraitPath))
        {
            isWaitingForPlayer = true;
            canClick = true;
            return;
        }
        if (string.IsNullOrEmpty(currentNode.characterName))
        {
            SpawnAMessageBubble(selectedDialogText, null, false, false, true);
        }
        else if (currentNode.characterName == "Player")
        {
            SpawnAMessageBubble(selectedDialogText, null, true, false, false);
        }
        else
        {
            SpawnAMessageBubble(selectedDialogText, currentNode.characterName, false, false, false);
        }

        if (!string.IsNullOrEmpty(currentNode.portraitPath))
        {
            Sprite portrait = Resources.Load<Sprite>(currentNode.portraitPath);
            characterPortrait.gameObject.SetActive(true);
            characterPortrait.sprite = portrait;
        }
        canClick = false;

        if (currentNode.options == null || currentNode.options[0].optionText == "")
        {
            canClick = true;
            isWaitingForPlayer = true;
            if (currentNode.isEndNode)
            {
                DOVirtual.DelayedCall(2f,()=> { EndDialog(); }).SetId(gameObject);
            }
        }
        else
        {
            isWaitingForPlayer = true;
            canClick = true;
        }
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
            text = selectedOption.messageText;
        }
        else
        {
            text = selectedOption.optionText;
        }

        SpawnAMessageBubble(text, null, true, false, false);
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
        float delayTime = clickCooldown + text.Length * waitTimePerCharacter;

        StartCoroutine(OptionSelectedRoutine(delayTime, selectedOption));
    }

    IEnumerator OptionSelectedRoutine(float delay, DialogOption selectedOption)
    {
        yield return new WaitUntil(() => isWaitingForPlayer == false);
        if (selectedOption.optionText == "Skip to final decision" || selectedOption.optionText == "Skip to Final Decisions")
        {
            SkipDialogue();
            yield return null;
        }

        if (!(selectedOption.optionText == "Skip to Incentives"))
        {
            choicesSelectedCount++;
        }
        // Find the next node

        string nextId = selectedOption.GetNextNodeId();
        //currentPathIds.Add(nextId);
        //currentPathIndex = currentPathIds.Count - 1;
        currentNode = currentDialogTree.GetNodeById(nextId);
        //int remainingCount = CountNodesFrom(nextId);
        //finalPathLength = currentPathIds.Count + remainingCount;

        DOVirtual.DelayedCall(1f, () =>
        {
            isWaitingForPlayer = true;
            canClick = true;
            DisplayCurrentNode();
        }).SetId(gameObject);


    }

    public void GetPathCompletionPercent()
    {
        float percent;
        percent = ((float)choicesSelectedCount / totalChoices) * 100f;
        //if (finalPathLength > 1)
        //{
        //    percent = (currentPathIndex / (float)(finalPathLength - 1)) * 100f;
        //}
        //else
        //{
        //    percent = 0f;
        //}
        if(optionsShown == 1)
        {
            percent = 0f;
        }

        percent = Mathf.Clamp(percent, 0, 100);
        if(houseDialogCompletePercent.TryGetValue(key,out var previousPercent))
        {
            houseDialogCompletePercent[key] = previousPercent >= percent ? previousPercent : percent;
            //Debug.Log($"{key}'s percent is {houseDialogCompletePercent[key]}");
        }
        else
        {
            houseDialogCompletePercent.Add(key, percent);
            //Debug.Log($"{key}'s percent is {houseDialogCompletePercent[key]}");
        }
        //Debug.Log($"Dialog is {percent:F1}% complete on this path.");
    }

    private void ShowOptions()
    {
        optionsShown++;
        var flags = dialogFlagsMap[currentDialogTree.houseType];
        foreach (var option in currentNode.options)
        {
            if (!option.conditions.IsMet(flags.Item1)) continue;

            var optionBubble = SpawnAMessageBubble(option.optionText, null, false, true, false);
            optionBubble.messageBox.onClick.AddListener(() =>
            {
                OnOptionSelected(currentNode.options.ToList().IndexOf(option));
            });
        }
    }

    public void EndDialog(bool closed = false)
    {
        StopAllCoroutines();

        // stop audio when dialogue conversation ends
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }


        if (!dialogFlagsMap[key].Item2 && dialogFlagsMap[key].Item1.hasSpoken != 1)   // if NOT skipped && has not spoken
        {
            //only count house type
            if (Enum.IsDefined(typeof(HouseType), key))
            {
                SetFlag("hasSpoken", 1); // set has spoken to 1
            }

        }
        GetPathCompletionPercent();

        ClearMessages();
        //isWaitingForPlayer = true;
        characterPortrait.gameObject.SetActive(false);
        OnDialogueComplete?.Invoke();
        DOTween.Kill(gameObject);
        var isHouseDialog = Enum.TryParse(key, out HouseType houseType) || key == "tutorial";
        ATC_UIController.Instance.HideDialog(isHouseDialog);
        if (isHouseDialog)
        {
            if (closed)
            {
                ATC_UIController.Instance.FindMenu(houseType).OnMenuDisable();
            }
            else
            {
                ATC_UIController.Instance.FindMenu(houseType).OnMenuEnable ();
            }
        }

    }
    public FC_MessageBubble SpawnAMessageBubble(string message, string name, bool isSentByUser, bool isOption, bool isDescription)
    {
        if (isOption)
        {
            var optionBubble = Instantiate(messageBubblePrefab, messagebBubblesContainer).GetComponent<FC_MessageBubble>();
            optionBubble.SetupOptionButton(message);
            optionMessageBubbles.Add(optionBubble);
            AnimateMessageBubble(optionBubble.GetComponent<CanvasGroup>());
            // pop in effect
            optionBubble.transform.localScale = Vector3.zero;
            optionBubble.transform.DOScale(Vector3.one, 0.3f)
                                  .SetEase(Ease.OutBack)
                                  .OnComplete(() => { optionBubble.messageBox.interactable = true; });

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
        canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutBack).SetLink(canvasGroup.gameObject);
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
        DOTween.KillAll();
    }

    public void SetSkipButton(bool isActive)
    {
        if (!canShowSkipButton) return;
        skipButton.gameObject.SetActive(isActive);
    }

    public void SetFlag(string flagName, int value)
    {
        if (!dialogFlagsMap.TryGetValue(key, out (DialogFlags,bool) flags)) return;
        if (flags.Item1 == null) return;
        switch (flagName)
        {
            case "hasSpoken":
                flags.Item1.hasSpoken = value;
                break;
            case "hasIncentives":
                flags.Item1.hasIncentives = value;
                break;
            case "gaveIncentives":
                flags.Item1.gaveIncentives = value;
                break;
            default:
                Debug.Log($"Flag '{flagName}' not found.");
                break;
        }
    }


    public void ResetFlags()
    {
        foreach (var flags in dialogFlagsMap.Values)
        {
            flags.Item1.hasIncentives = GameManager.Instance.HasIncentives == true? 1:0;
            flags.Item1.hasSpoken = 0;
            flags.Item1.gaveIncentives = 0;
        }
    }
}
