using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StructureContextMenu : MonoBehaviour
{
    //public Action<OptionButton> onOptionSelected;
    public Action onOptionSelected;
    public OptionButton changeResponseButton;
    public OptionButton confirmButton;
    public TextMeshProUGUI explaination;
    public GameObject menuUI;//ui
    public HouseIcon icon;//ui
    public TextMeshProUGUI title;
    [SerializeField] Transform options;
    public GameObject optionButtonPrefab;
    public Button closeButton;
    //public Button assignButton;
    public Structure owner;
    //bool optionsAreLocked = true;
    //[SerializeField] RectTransform canvasTransform;
    [SerializeField] RectTransform menuTransform;
    [SerializeField] float menuOffset = 120f;
    Camera cam;
    public string CurrentOption { get; private set; }
    string previousOption = null;
    // Start is called before the first frame update
    private void Awake()
    {
        //assignButton.gameObject.SetActive(false);
        
    }
    private void Start()
    {
        cam = Camera.main;
        HouseStructure house = (HouseStructure)owner;
        changeResponseButton.button.onClick.AddListener(() =>
        {
            ToggleChangeResponsePanel(false);
        });
        icon.InitIcon(house.HouseType);
        
    }

    public void OnMenuEnable()
    {
        if(owner == null) return;
        //menuUI.SetActive(true);
        HouseStructure house = (HouseStructure)owner;

        if (!house.isMainHouse) return;
        house.OnStructureClick();
        ATC_UIController.Instance.PushPanel(menuUI);
        icon.gameObject.SetActive(false);
        UpdateMenuForHouse(house);
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
        foreach (var menu in ATC_UIController.Instance.contextMenus)
        {
            //menu.menuUI.SetActive(false);
            menu.icon.gameObject.SetActive(true);
        }
        owner.StopSturctureClick();
        ToggleChangeResponsePanel(false);

        ClearOptionButtons();
        ATC_UIController.Instance.ClearAllPanels();
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
        menuUI.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);

        ATC_UIController.Instance.ClampToWindow(menuTransform, menuOffset);
    }

    public void ClearChoice()
    {
        previousOption = null;
        CurrentOption = null;
    }

    public void UpdateMenuForHouse(HouseStructure house)
    {
        ClearOptionButtons();
        //var houseInfo = house.info;
        var houseInfo = house.houseInfo;
        title.text = houseInfo.menuTitle;
        foreach (var entry in houseInfo.houseChoicesDict)
        {
            var choice = entry.Value.choice;
            SpawnOptionButtons(choice.choiceName,choice.isLocked);
        }
        //foreach(var choice in houseInfo.lockedChoices)
        //{
        //    var isLocked = choice.isLocked;
        //    SpawnOptionButtons(choice.choiceName,isLocked);
        //}
    }

    private void SpawnOptionButtons(string text, bool isLocked = false)
    {
        GameObject button = Instantiate(optionButtonPrefab,options);
        var optionButton = button.GetComponent<OptionButton>();

        optionButton.InitOptionButton(this, text);
        if (isLocked)
        {
            //Debug.Log("Locked Option: " + text);
            optionButton.isLocked = true;

        }
        if (previousOption == "Home Hardening" && text == "Home Hardening")
        {
            optionButton.ToggleOptionSelectState(true);
        }
        else { 
            optionButton.ToggleOptionSelectState(text == CurrentOption);
        }
    }
    public void OnOptionButtonClicked(OptionButton option)
    {
        if (option.needConfirmation)
        {
            ToggleChangeResponsePanel(true,option);
        }
        else
        {
            if(CurrentOption != null)
            {
                previousOption = CurrentOption;
            }
            CurrentOption = option.GetOptionContent();
            OnMenuDisable();
        }

        if (CurrentOption == null) return;
        onOptionSelected.Invoke();
       
    }
    public void ApplyBehavior()
    {
        HouseStructure house = (HouseStructure)owner;
        StartCoroutine(house.SpawnCarRoutine());
    }

    void ToggleChangeResponsePanel(bool state, OptionButton currentOption = null)
    {
        explaination.transform.parent.gameObject.SetActive(state);
        options.gameObject.SetActive(!state);

        if(state == true)
        {
            confirmButton.button.onClick.AddListener(() =>
            {
                currentOption.needConfirmation = false;
                OnOptionButtonClicked(currentOption);
            });
        }
        else
        {
            if(currentOption != null)
            {
                currentOption.needConfirmation = true;
                confirmButton.button.onClick.RemoveAllListeners();
            }

        }

    }
}
