using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FC_TutorialManager : MonoBehaviour
{
    public Button titleCard,fireStationIcon;
    public GameObject bottomDialogBox;
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

        fireStationIcon.onClick.AddListener(OnFirestationIconClicked);
        uiIcon = fireStationIcon.GetComponent<RectTransform>();
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
        bottomDialogBox.SetActive(true);
        UpdateBottomDialog("Welcome to Firewise Citizens! Tap on the Fire Station to Begin");
        fireStationIcon.gameObject.SetActive(true);
        fireStation = structureManager.specialStructureDict[StructureType.FireStation].GetComponent<Structure>();    
        
    }
    void UpdateBottomDialog(string text)
    {
        bottomDialogText.text = text;
    }

    public void InitTutorialHouse()
    {

        tutorialHouse = structureManager.allMainHouses[HouseType.pet];
        //tutorialHouse.contextMenu.icon.gameObject
    }

    public void OnFirestationIconClicked()
    {
        InitTutorialHouse();
        GameManager.Instance.cameraMovement.MoveToHouse(fireStation);
        bottomDialogBox.SetActive(false);
        //dialogManager.StartDialog("Tutorial");
    }
}
