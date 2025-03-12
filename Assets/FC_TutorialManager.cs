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
    public ATC_HouseDialogManager dialogManager;
    public StructureManager structureManager;
    public HouseStructure tutorialHouse;
    public Structure fireStation;

    public int fireFighterStartNodeId, fireFighterEndNodeId, skipTutorialNodeId, reviewTutorialNodeId;
    RectTransform uiIcon;
    bool isTutorialStarted = false;
    bool isFirstTimeTutorial = true;


    public void InitTutorialManager()
    {
        titleCard.onClick.AddListener(() =>
        {
            titleCard.gameObject.SetActive(false);
            StartTutorial();
        });

        dialogManager.OnDialogueNodeDisplayed += CheckDialogueNode;
        dialogManager.OnDialogueOptionSelected += CheckDialogueOption;

        fireStationIcon.onClick.AddListener(OnFirestationIconClicked);
        uiIcon = fireStationIcon.GetComponent<RectTransform>();
    }
    private void CheckDialogueOption(DialogOption option)
    {
        int nextNodeId = ParseStringToInt(option.nextNodeId);
        if (nextNodeId == 0) return;
        //slide out firefighter portrait
        if (nextNodeId == fireFighterEndNodeId)
        {
            MoveSidePortrait(300f);
        }
        //skip directly to outro
        else if (nextNodeId == skipTutorialNodeId)
        {
            GameManager.Instance.cameraMovement.ResetCam();
            dialogManager.StartDialogue("outro",true);
            dialogManager.OnDialogueComplete = null;
            dialogManager.OnDialogueComplete += OnOutroDialogueComplete;
        }
        //restart the tutorial but show the skip button
        else if (nextNodeId == reviewTutorialNodeId)
        {
            isFirstTimeTutorial = false;
            dialogManager.OnDialogueComplete = null;

            DOVirtual.DelayedCall(1f, () =>
            {
                dialogManager.EndDialog();

                // hide all the house icons
                foreach (var menu in ATC_UIController.Instance.contextMenus)
                {
                    menu.icon.gameObject.SetActive(false);
                }

                //clear choice
                structureManager.GetPlayerChoicesDict().Clear();
                tutorialHouse.contextMenu.isSelected = false;
                tutorialHouse.contextMenu.icon.ToggleIconState(true);
                dialogManager.isWaitingForPlayer = true;
                StartTutorial();
            });

        }
        //dialog will only display after player selected options
        dialogManager.isWaitingForPlayer = false;
    }

    private void CheckDialogueNode(DialogNode node)
    {
        int nodeId = ParseStringToInt(node.id);
        if (nodeId == 0) return;
        if (nodeId == fireFighterStartNodeId)
        {
            // slide in portrait
            MoveSidePortrait(-300f);
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
        dialogManager.StartDialogue("outro");
        dialogManager.canShowSkipButton = false;
        dialogManager.OnDialogueComplete = null;
        dialogManager.OnDialogueComplete += OnOutroDialogueComplete;
    }

    //end tutorial
    private void OnOutroDialogueComplete()
    {
        //Debug.Log("Outro is completed");
        isTutorialStarted = false;
        GameManager.Instance.cameraMovement.ResetCam();
        var houseIcon = tutorialHouse.contextMenu.icon;
        houseIcon.RemoveOnClickAction(OnClickTutorialHouse);
        dialogManager.canShowSkipButton = true;
        dialogManager.isWaitingForPlayer = false;
        GameManager.Instance.SkipSimulationRec();
        dialogManager.OnDialogueComplete = null;
    }

    private void OnIntroDialogueComplete()
    {
        var text1 = "Mary hasn't been spoken to yet, so her home is <b>marked</b> with a <sprite name=\"pet\">.\n";
        var controlText = string.Empty;
        //if (GameManager.Instance.inputManager.isKeyboard)
        //{
        //    controlText = "<b>Use W A S D to move, </b>";
        //}
        //else
        //{
        //    controlText = "<b>Press and hold on the map to scroll, </b>";
        //}
        var text2 = " and <b>select Mary's Home.</b>";
        string message = text1 + "<b>Use W A S D to move OR Press and hold on the map to scroll </b>" + text2;
        UpdateBottomDialog(message);
        GameManager.Instance.cameraMovement.ResetCam();
        tutorialHouse.contextMenu.icon.gameObject.SetActive(true);
        dialogManager.canShowSkipButton = !isFirstTimeTutorial;
        dialogManager.OnDialogueComplete = null;
        dialogManager.OnDialogueComplete += OnTutroialDialogueComplete;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTutorialStarted || !fireStationIcon.gameObject.activeSelf) return;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(fireStation.transform.position);
        uiIcon.position = screenPosition + new Vector3(0,100f,0f);
    }
    public void StartTutorial()
    {
        GameManager.Instance.currentStage = LevelStage.Tutorial;
        //dialogManager.canShowSkipButton = false;
        dialogManager.OnDialogueComplete += OnIntroDialogueComplete;
        isTutorialStarted = true;
        UpdateBottomDialog("Welcome to Firewise Citizens! Tap on the Fire Station to Begin");
        fireStationIcon.gameObject.SetActive(true);
        fireStation = structureManager.specialStructureDict[StructureType.FireStation].GetComponent<Structure>();
        SetUpTutorialHouse();
    }
    void UpdateBottomDialog(string text)
    {
        bottomDialogBox.SetActive(true);
        bottomDialogText.text = text;
    }

    void SetUpTutorialHouse()
    {
        if (tutorialHouse != null || !isFirstTimeTutorial) return;
        foreach (var house in structureManager.allMainHouses.Values)
        {
            house.outline.enabled = false;
        }
        tutorialHouse = structureManager.allMainHouses[HouseType.pet];
        tutorialHouse.outline.enabled = true;
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
        dialogManager.StartDialogue("tutorial");
    }

    public void OnFirestationIconClicked()
    {
        GameManager.Instance.cameraMovement.MoveToHouse(fireStation);
        bottomDialogBox.SetActive(false);
        ATC_UIController.Instance.ShowDialog();
        fireStationIcon.gameObject.SetActive(false);
        dialogManager.StartDialogue("intro");
    }

}
