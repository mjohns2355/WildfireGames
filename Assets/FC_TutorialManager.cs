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

    RectTransform uiIcon;
    bool isTutorialStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        titleCard.onClick.AddListener(() =>
        {
            titleCard.gameObject.SetActive(false);
            StartTutorial();
        });

        dialogManager.OnDialogueComplete += OnIntroDialogueComplete;
        fireStationIcon.onClick.AddListener(OnFirestationIconClicked);
        uiIcon = fireStationIcon.GetComponent<RectTransform>();
    }

    private void OnTutroialDialogueComplete()
    {
        tutorialHouse.contextMenu.OnMenuEnable();
        tutorialHouse.contextMenu.confirm.onClick.AddListener(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                ATC_UIController.Instance.ShowDialog();
                dialogManager.StartDialog("outro");
                fireFighterDialogue.SetActive(false);
            });

        });
        fireFighterDialogue.SetActive(true);
    }

    private void OnIntroDialogueComplete()
    {
        var text1 = "Mary hasn't been spoken to yet, so her home is <b>marked</b> with an icon.\n";
        var controlText = string.Empty;
        if (GameManager.Instance.inputManager.isKeyboard)
        {
            controlText = "<b>Use W A S D to move, </b>";
        }
        else
        {
            controlText = "<b>Press and hold on the map to scroll, </b>";
        }
        var text2 = " and <b>select Mary's Home.</b>";
        string message = text1 + controlText + text2;
        UpdateBottomDialog(message);
        GameManager.Instance.cameraMovement.ResetCam();
        tutorialHouse.contextMenu.icon.gameObject.SetActive(true);
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
        tutorialHouse = structureManager.allMainHouses[HouseType.pet];
        var houseIcon = tutorialHouse.contextMenu.icon.GetComponent<Button>();
        houseIcon.onClick.RemoveAllListeners();
        houseIcon.onClick.AddListener(OnClickTutorialHouse);
    }

    private void OnClickTutorialHouse()
    {
        GameManager.Instance.cameraMovement.MoveToHouse(tutorialHouse);
        bottomDialogBox.SetActive(false);
        ATC_UIController.Instance.ShowDialog();
        dialogManager.StartDialog("tutorial");
    }

    public void OnFirestationIconClicked()
    {
        GameManager.Instance.cameraMovement.MoveToHouse(fireStation);
        bottomDialogBox.SetActive(false);
        ATC_UIController.Instance.ShowDialog();
        fireStationIcon.gameObject.SetActive(false);
        dialogManager.StartDialog("intro");
    }
}
