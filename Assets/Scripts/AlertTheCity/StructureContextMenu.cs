using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StructureContextMenu : MonoBehaviour
{
    //public Action<OptionButton> onOptionSelected;
    public Action onOptionConfirmed;
    //public OptionButton changeResponseButton;
    //public OptionButton confirmButton;
    public Button confirm, restart;
    public TextMeshProUGUI explaination;
    public GameObject menuUI;//ui
    public HouseIcon icon;//ui
    public TextMeshProUGUI title;
    [SerializeField] Transform options;
    public GameObject optionButtonPrefab;
    //public Button closeButton;
    //public Button assignButton;
    public Structure owner;
    //bool optionsAreLocked = true;
    //[SerializeField] RectTransform canvasTransform;
    [SerializeField] RectTransform menuTransform;
    [SerializeField] float menuOffset = 120f;
    Camera cam;
    public OptionButton CurrentOption { get; private set; }
    OptionButton previousOption = null;
    public bool allowMultipleChoices;
    public List<OptionButton> selectedOptions = new List<OptionButton>();
    public bool isSelected = false;
    public ATC_LearnMorePopup learnMorePopup;
    public Image choicePicture;
    private GameObject schoolText;
    // Start is called before the first frame update
    private void Awake()
    {
        //assignButton.gameObject.SetActive(false);

    }
    private void Start()
    {
        schoolText = GameObject.FindGameObjectWithTag("marker");
        cam = Camera.main;
        HouseStructure house = (HouseStructure)owner;

        //changeResponseButton.button.onClick.AddListener(() =>
        //{
        //    ToggleChangeResponsePanel(false);
        //});
        confirm.onClick.AddListener(() =>
        {
            //if(CurrentOption != null)
            //{

            //}
            onOptionConfirmed.Invoke();
            isSelected = true;

            GameManager.Instance.cameraMovement.ResetCam();
            OnMenuDisable();
        });

        restart.onClick.AddListener(() =>
        {
            ClearChoice();
            //GameManager.Instance.cameraMovement.ResetCam();
            OnMenuDisable();
            ShowDialog();
        });
        icon.InitIcon(house.houseType);
        icon.AddOnClickActions(OnMainHouseClicked);

    }

    public void OnMainHouseClicked()
    {
        // school text
        schoolText.SetActive(false);
        if (GameManager.Instance.currentStage == LevelStage.Tutorial) return;
        ATC_UIController.Instance.houseDialogManager.canShowSkipButton = isSelected;
        if (isSelected)
        {
            GameManager.Instance.cameraMovement.MoveToHouse(owner, false);

            OnMenuEnable();
            return;
        }
        GameManager.Instance.cameraMovement.MoveToHouse(owner/*.camFocusPos*/);
        ShowDialog();
    }

    void ShowDialog()
    {
        
            ATC_UIController.Instance.ShowDialog();

            //ATC_UIController.Instance.houseDialogManager.StartHouseDialog(icon.iconHouseType,icon.houseDialog);
            GameManager.Instance.currentStage = LevelStage.HouseDialog;
            ATC_UIController.Instance.houseDialogManager.StartDialogue(icon.iconHouseType.ToString());
        


    }
    public void OnMenuEnable()
    {
        if(owner == null) return;
        //menuUI.SetActive(true);
        HouseStructure house = (HouseStructure)owner;
        //Debug.Log($"{house.HouseType} is selected: {isSelected}");
        //if (CurrentOption != null)
        //{
        //    Debug.Log($"Current option is {CurrentOption.GetOptionContent()}. ");
        //}
        choicePicture.sprite = house.houseInfo.choicePicture;
        confirm.interactable = isSelected;
        if (!house.isMainHouse) return;
        allowMultipleChoices = house.houseInfo.allowMultipleChoices;
        house.OnStructureClick();
        ATC_UIController.Instance.PushPanel(menuUI);
        icon.gameObject.SetActive(false);
        //ATC_UIController.Instance.ToggleHouseIcons(false);
        UpdateMenuForHouse(house, GameManager.Instance.currentStage == LevelStage.Tutorial);
        foreach (var menu in ATC_UIController.Instance.contextMenus)
        {
            if (menu == this) continue;
            if (!menu.gameObject.activeSelf) continue;
            //menu.menu.SetActive(false);
            menu.icon.gameObject.SetActive(false);
        }


    }

    public void OnMenuDisable()
    {
        ATC_UIController.Instance.toolsBar.SetActive(true);
        foreach (var menu in ATC_UIController.Instance.contextMenus)
        {
            //menu.menuUI.SetActive(false);
            menu.icon.gameObject.SetActive(true);
        }
        owner.StopSturctureClick();
        //ToggleChangeResponsePanel(false);
        //confirm.interactable = false;
        ClearOptionButtons();
        ClearChoice();
        ATC_UIController.Instance.ClearAllPanels();
        //ATC_UIController.Instance.ToggleHouseIcons(true);
        icon.ToggleIconState(!isSelected);
        //school text

        schoolText.SetActive(true);
        //GameManager.Instance.cameraMovement.ResetCam();
        //GameManager.Instance.canControlCam = true;
        //StartCoroutine(house.SpawnCarRoutine());

        //changeResponseButton.onClick.RemoveAllListeners();
        //selectedBehavior = true;

    }

    void ClearOptionButtons()
    {
        if(options.childCount == 0) return; 
        for (int i = 0; i < options.childCount; i++)
        {

            Destroy(options.GetChild(i).gameObject);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        icon.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);
        //menuUI.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);

        //ATC_UIController.Instance.ClampToWindow(menuTransform, menuOffset);
    }

    public void ClearChoice()
    {
        //previousOption = null;
        //CurrentOption = null;
        //if (allowMultipleChoices)
        //{
        foreach (var option in selectedOptions)
        {
            option.ToggleOptionSelectState(false);
        }
        selectedOptions.Clear();
        //}
    }

    public void UpdateMenuForHouse(HouseStructure house, bool isTutorial = false)
    {
        ClearOptionButtons();
        //var houseInfo = house.info;
        var houseInfo = house.houseInfo;
        title.text = houseInfo.menuTitle;


        foreach (var entry in houseInfo.houseChoicesDict)
        {
            var choice = entry.Value.choice;
            if (isTutorial)
            {
                if(choice.choiceName == "Plan Ahead")
                {
                    SpawnOptionButtons(choice.choiceName/*,choice.isLocked*/);
                    break;
                }
            }
            else {
                SpawnOptionButtons(choice.choiceName/*,choice.isLocked*/);
            }
        }
        
        //foreach(var choice in houseInfo.lockedChoices)
        //{
        //    var isLocked = choice.isLocked;
        //    SpawnOptionButtons(choice.choiceName,isLocked);
        //}
    }

    private void SpawnOptionButtons(string text/*, bool isLocked = false*/)
    {
        
        GameObject button = Instantiate(optionButtonPrefab,options);
        var optionButton = button.GetComponent<OptionButton>();

        optionButton.InitOptionButton(this, text);
        //if (isLocked)
        //{
        //    //Debug.Log("Locked Option: " + text);
        //    optionButton.isLocked = true;

        //}
        if(!isSelected) return;
        HouseStructure house = (HouseStructure)owner;
        var selectedChoice = GameManager.Instance.structureManager.GetPlayerChoicesDict()[house.houseType];

        foreach(var c in selectedChoice)
        {
            if (c.choiceName == text)
            {
                optionButton.ToggleOptionSelectState(true);
                //if (allowMultipleChoices)
                //{
                    selectedOptions.Add(optionButton);
                //}
                //else
                //{
                //    CurrentOption = optionButton;
                //}
            }
        }
        //if(selectedChoice.choiceName == text)
        //{
        //    optionButton.ToggleOptionSelectState(true);
        //}
        //if (!allowMultipleChoices && CurrentOption != null && text == CurrentOption.GetOptionContent())
        //{
        //    Debug.Log($"Selected Option: {CurrentOption.GetOptionContent()}");
        //    optionButton.ToggleOptionSelectState(true);
        //}
        //else if (allowMultipleChoices && selectedOptions.Contains(optionButton))
        //{
        //    optionButton.ToggleOptionSelectState(true);
        //}
        //if (CurrentOption == null) return;
        //if (previousOption.GetOptionContent() == "Home Hardening" && text == "Home Hardening")
        //{
        //    optionButton.ToggleOptionSelectState(true);
        //}
        //else { 
        //    optionButton.ToggleOptionSelectState(text == CurrentOption.GetOptionContent());
        //}
    }
    public void OnOptionButtonClicked(OptionButton option)
    {
        HouseStructure house = (HouseStructure)owner;
        string waitForNotice = "Wait for Notice";
        string evacuateEarly = "Evacuate Early";
        if (selectedOptions.Contains(option))
        {
            option.ToggleOptionSelectState(false);
            selectedOptions.Remove(option);
        }
        else
        {
            // Check for conflicting options
            OptionButton conflictingOption = selectedOptions.FirstOrDefault(o =>
                        (o.GetOptionContent() == waitForNotice && option.GetOptionContent() == evacuateEarly) ||
                        (o.GetOptionContent() == evacuateEarly && option.GetOptionContent() == waitForNotice));

            if (conflictingOption != null)
            {
                Debug.Log("Conflicting options: " + conflictingOption.GetOptionContent());
                conflictingOption.ToggleOptionSelectState(false);
                selectedOptions.Remove(conflictingOption);
            }

            // handle any multi-choice logic
            if (selectedOptions.Count >= house.houseInfo.requiredChoicesCount)
            {
                var oldestOption = selectedOptions[0];
                oldestOption.ToggleOptionSelectState(false);
                selectedOptions.RemoveAt(0);
            }

            // Add the new selection
            option.ToggleOptionSelectState(true);
            selectedOptions.Add(option);
        }
        //Debug.Log($"Selected {selectedOptions.Count} choices");
        confirm.interactable = true;
        //confirm.interactable = selectedOptions.Count == house.houseInfo.requiredChoicesCount;
        //onOptionSelected.Invoke();
        // }
    }
    public void ApplyBehavior()
    {
        HouseStructure house = (HouseStructure)owner;
        StartCoroutine(house.SpawnCarRoutine());
    }


}
