using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FC_TutorialManager : MonoBehaviour
{
    public Button titleCard,fireStationIcon;
    public GameObject bottomDialogBox, fireFighterDialogue, sideFirefighterPortrait;
    public TextMeshProUGUI bottomDialogText;
    //public ATC_HouseDialogManager dialogManager;
    //public StructureManager structureManager;
    public HouseStructure tutorialHouse;
    public Structure fireStation;
    public Sprite tutorialChoiceSprite;
    public int fireFighterStartNodeId, fireFighterEndNodeId, skipTutorialNodeId, reviewTutorialNodeId;
    RectTransform uiIcon;
    bool isTutorialStarted = false;
    [SerializeField] bool isFirstTimeTutorial = true;
    [SerializeField] StructureManager structureManager;
    [SerializeField] ATC_HouseDialogManager houseDialogManager;
    public GameObject voiceOverToggle;

    public void InitTutorialManager()
    {

        // reassign variables
        structureManager = GameManager.Instance.structureManager;
        houseDialogManager = ATC_UIController.Instance.houseDialogManager;
        //clean up old listeners
        titleCard.onClick.RemoveAllListeners();
        houseDialogManager.OnDialogueNodeDisplayed -= CheckDialogueNode;
        houseDialogManager.OnDialogueOptionSelected -= CheckDialogueOption;
        fireStationIcon.onClick.RemoveAllListeners();

        uiIcon = fireStationIcon.GetComponent<RectTransform>();
        // add listeners
        titleCard.onClick.AddListener(() =>
        {
            titleCard.gameObject.SetActive(false);
            voiceOverToggle.SetActive(false);
            StartTutorial();
        });

        
        houseDialogManager.OnDialogueNodeDisplayed += CheckDialogueNode;
        houseDialogManager.OnDialogueOptionSelected += CheckDialogueOption;

        fireStationIcon.onClick.AddListener(OnFirestationIconClicked);
        // TO-DO: should have a tutorial house type instead of using existing pet
        GameManager.Instance.availableHouseTypes.Clear();
        GameManager.Instance.availableHouseTypes.Add(HouseType.pet);
        houseDialogManager.skipButton.onClick.RemoveListener(() =>
        {
            if (isTutorialStarted)
            {
                OnOutroDialogueComplete();
            }
        });

        houseDialogManager.skipButton.onClick.AddListener(() =>
        {
            if (isTutorialStarted)
            {
                OnOutroDialogueComplete();
            }
            
        });
        // only hide the skip button when player first time playing the tutorial
        if (!isFirstTimeTutorial) return;
        houseDialogManager.SetSkipButton(false);
        houseDialogManager.canShowSkipButton = false;



    }
    private void CheckDialogueOption(DialogOption option)
    {
        int nextNodeId = ParseStringToInt(option.GetNextNodeId());
        //Debug.Log("Next node id: " + nextNodeId);
        if (nextNodeId == 0) return;
        //slide out firefighter portrait
        if (nextNodeId == fireFighterEndNodeId)
        {
            //Debug.Log("Slide out firefighter");
            MoveSidePortrait(350f);
        }
        //skip directly to outro
        else if (nextNodeId == skipTutorialNodeId)
        {
            GameManager.Instance.cameraMovement.ResetCam();
            houseDialogManager.StartDialogue("outro",true);
            houseDialogManager.OnDialogueComplete = null;
            houseDialogManager.OnDialogueComplete += OnOutroDialogueComplete;
        }
        //restart the tutorial but show the skip button
        else if (nextNodeId == reviewTutorialNodeId)
        {
            isFirstTimeTutorial = false;
            houseDialogManager.OnDialogueComplete = null;
            GameManager.Instance.cameraMovement.ResetCam();
            houseDialogManager.EndDialog();

            // hide all the house icons
            foreach (var menu in ATC_UIController.Instance.contextMenus)
            {
                menu.icon.gameObject.SetActive(false);
            }

            //clear choice
            GameManager.Instance.structureManager.GetPlayerChoicesDict().Clear();
            tutorialHouse.contextMenu.isSelected = false;
            tutorialHouse.contextMenu.icon.ToggleIconState(true);
            tutorialHouse.SetOutline(false);
            houseDialogManager.isWaitingForPlayer = true;
            ReloadTutorial();

        }
        //dialog will only display after player selected options
        houseDialogManager.isWaitingForPlayer = false;
    }

    private void CheckDialogueNode(DialogNode node)
    {
        int nodeId = ParseStringToInt(node.id);
        if (nodeId == 0) return;
        if (nodeId == fireFighterStartNodeId)
        {
            // slide in portrait
            MoveSidePortrait(-350f);
        }
    }

    private int ParseStringToInt(string str)
    {
        int nodeId;
        if(int.TryParse(str, out nodeId))
        {
            return nodeId;
        }
        //Debug.Log("Invalid string parse to int");
        return 0;
    }
    private void MoveSidePortrait(float moveDistance)
    {
        var rect = sideFirefighterPortrait.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        rect.DOAnchorPosX(startPos.x - moveDistance, 0.5f).SetEase(Ease.OutQuad);
    }
    private void OnTutroialDialogueComplete()
    {
        tutorialHouse.contextMenu.OnMenuEnable();

        fireFighterDialogue.SetActive(true);
    }

    private void OnConfirmedTutorialHouseMenu()
    {
        //Debug.Log("Invoke outro");
        fireFighterDialogue.SetActive(false);
        ATC_UIController.Instance.ShowDialog();
        houseDialogManager.StartDialogue("outro");
        //houseDialogManager.canShowSkipButton = false;
        houseDialogManager.OnDialogueComplete = null;
        houseDialogManager.OnDialogueComplete += OnOutroDialogueComplete;
    }

    //end tutorial
    private void OnOutroDialogueComplete()
    {
        isTutorialStarted = false;
        isFirstTimeTutorial = false;
        GameManager.Instance.cameraMovement.ResetCam();
        var houseIcon = tutorialHouse.contextMenu.icon;
        houseIcon.RemoveOnClickAction(OnClickTutorialHouse);
        houseDialogManager.canShowSkipButton = true;
        houseDialogManager.SetSkipButton(true);
        houseDialogManager.isWaitingForPlayer = false;
        //GameManager.Instance.SkipSimulationRec();
        Debug.Log("Remove Pet");
        GameManager.Instance.availableHouseTypes.Remove(HouseType.pet);
        ATC_UIController.Instance.ToggleHouseIcons(true);
        houseDialogManager.OnDialogueComplete = null;
        houseDialogManager.OnDialogueNodeDisplayed -= CheckDialogueNode;
        houseDialogManager.OnDialogueOptionSelected -= CheckDialogueOption;
        GameManager.Instance.currentStage = LevelStage.PhaseOne;
        GameManager.Instance.ResetGame();
        //gameObject.SetActive(false);
    }

    private void OnIntroDialogueComplete()
    {
        GameManager.Instance.canControlCam = true;
        var text1 = "Mary hasn't been spoken to yet, so her home is <b>marked</b> with a <sprite name=\"Pets\">. ";
        var controlText = string.Empty;
        var text2 = "and <b>select Mary's Home.</b>";
        //string message = text1 + "<b>Use W A S D to move</b> OR <b>Press and hold on the map to scroll </b>" + text2;
        string message = text1 + "<b>Touch and drag to move </b>" + text2;
        UpdateBottomDialog(message);
        GameManager.Instance.cameraMovement.ResetCam();
        tutorialHouse.contextMenu.icon.gameObject.SetActive(true);
        tutorialHouse.SetOutline(true);
        //houseDialogManager.canShowSkipButton = !isFirstTimeTutorial;
        houseDialogManager.OnDialogueComplete = null;
        houseDialogManager.OnDialogueComplete += OnTutroialDialogueComplete;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTutorialStarted || !fireStationIcon.gameObject.activeSelf || fireStation == null) return;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(fireStation.transform.position);
        uiIcon.position = screenPosition + new Vector3(0,100f,0f);
    }
    public void StartTutorial()
    {
        GameManager.Instance.currentStage = LevelStage.Tutorial;
        GameManager.Instance.canControlCam = false; 

        // hide all icons
        ATC_UIController.Instance.ToggleHouseIcons(false);
        //dialogManager.canShowSkipButton = false;
        houseDialogManager.OnDialogueComplete += OnIntroDialogueComplete;
        isTutorialStarted = true;
        if (StringManager.Instance != null)
        {
            string introText = StringManager.Instance.GetText("introText");
            UpdateBottomDialog(introText);
        }
        else
        {
            UpdateBottomDialog("Welcome to Firewise Residents! Tap on the Fire Station to Begin");
        }
        fireStationIcon.interactable = false;
        fireStationIcon.gameObject.SetActive(true);
        ScaleEffect(uiIcon).OnComplete(() => fireStationIcon.interactable = true);
        fireStation = structureManager.specialStructureDict[StructureType.FireStation].GetComponent<Structure>();
        SetUpTutorialHouse();
    }

    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 0.2f)
                         .SetLoops(2, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }
    void UpdateBottomDialog(string text)
    {
        bottomDialogBox.SetActive(true);
        bottomDialogText.text = text;
    }

    void SetUpTutorialHouse()
    {
        if (tutorialHouse != null) return;
        foreach (var house in structureManager.allMainHouses.Values)
        {
            house.SetOutline(false);
        }
        tutorialHouse = structureManager.allMainHouses[HouseType.pet];
        var houseIcon = tutorialHouse.contextMenu.icon;
        houseIcon.AddOnClickActions(OnClickTutorialHouse);
        tutorialHouse.contextMenu.restart.gameObject.SetActive(false);
        tutorialHouse.contextMenu.confirm.onClick.AddListener(() =>
        {
            OnConfirmedTutorialHouseMenu();

        });
    }

    private void OnClickTutorialHouse()
    {

        GameManager.Instance.cameraMovement.MoveToHouse(tutorialHouse);
        bottomDialogBox.SetActive(false);
        ATC_UIController.Instance.ShowDialog();
        houseDialogManager.StartDialogue("tutorial");
    }

    public void OnFirestationIconClicked()
    {
        houseDialogManager.ClearMessages();
        GameManager.Instance.cameraMovement.MoveToHouse(fireStation);
        bottomDialogBox.SetActive(false);
        ATC_UIController.Instance.ShowDialog();
        fireStationIcon.gameObject.SetActive(false);
        houseDialogManager.StartDialogue("intro");
    }

    public void ReloadTutorial()
    {

        titleCard.gameObject.SetActive(true);

    }
}
