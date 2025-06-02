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
    public Action onOptionConfirmed;
    public Button confirm, restart;
    public TextMeshProUGUI explaination;
    public GameObject menuUI;//ui
    public HouseIcon icon;//ui
    public TextMeshProUGUI title;
    [SerializeField] Transform options;
    public GameObject optionButtonPrefab;
    public Structure owner;
    [SerializeField] RectTransform menuTransform;
    [SerializeField] float menuOffset = 120f;
    Camera cam;
    public OptionButton CurrentOption { get; private set; }
    public bool allowMultipleChoices;
    public List<OptionButton> selectedOptions = new List<OptionButton>();
    public bool isSelected = false;
    public ATC_LearnMorePopup learnMorePopup;
    public Image choicePicture;
    private GameObject schoolText;

    private void Start()
    {
        schoolText = GameObject.FindGameObjectWithTag("marker");
        cam = Camera.main;
        HouseStructure house = (HouseStructure)owner;

        confirm.onClick.AddListener(() =>
        {
            onOptionConfirmed.Invoke();
            if (!isSelected)
            {
                isSelected = true;
                ATC_UIController.Instance.TalkedResidentsCount++;
                //ATC_UIController.Instance.UpdateObjectiveText();
            }
            
            Instantiate(Resources.Load("SelectedSFX") as GameObject);

            GameManager.Instance.cameraMovement.ResetCam();
            
            OnMenuDisable();
        });

        restart.onClick.AddListener(() =>
        {
            ClearChoice();
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
        owner.SetOutline(false);
        ATC_UIController.Instance.toolsBar.SetActive(false);
        if (GameManager.Instance.currentStage == LevelStage.Tutorial) return;
        //ATC_UIController.Instance.houseDialogManager.canShowSkipButton = isSelected;
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
        GameManager.Instance.currentStage = LevelStage.HouseDialog;
        ATC_UIController.Instance.houseDialogManager.StartDialogue(icon.iconHouseType.ToString());
    }
    public void OnMenuEnable()
    {
        if(owner == null) return;
        //menuUI.SetActive(true);
        HouseStructure house = (HouseStructure)owner;

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

        ClearOptionButtons();
        ClearChoice();
        ATC_UIController.Instance.ClearAllPanels();

        icon.ToggleIconState(!isSelected);
        //school text
        owner.SetOutline(true);
        schoolText.SetActive(true);

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
    void Update()
    {
        icon.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);
    }

    public void ClearChoice()
    {

        foreach (var option in selectedOptions)
        {
            option.ToggleOptionSelectState(false);
        }
        selectedOptions.Clear();

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
        
    }

    private void SpawnOptionButtons(string text/*, bool isLocked = false*/)
    {
        
        GameObject button = Instantiate(optionButtonPrefab,options);
        var optionButton = button.GetComponent<OptionButton>();

        optionButton.InitOptionButton(this, text);

        if(!isSelected) return;
        HouseStructure house = (HouseStructure)owner;
        var selectedChoice = GameManager.Instance.structureManager.GetPlayerChoicesDict()[house.houseType];

        foreach(var c in selectedChoice)
        {
            if (c.choiceName == text)
            {
                optionButton.ToggleOptionSelectState(true);
                    selectedOptions.Add(optionButton);
            }
        }
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
        confirm.interactable = selectedOptions.Count != 0;
    }
    public void ApplyBehavior()
    {
        HouseStructure house = (HouseStructure)owner;
        StartCoroutine(house.SpawnCarRoutine());
    }


}
