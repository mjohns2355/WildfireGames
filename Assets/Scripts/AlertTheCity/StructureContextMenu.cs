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
    //public OptionButton changeResponseButton;
    //public OptionButton confirmButton;
    public Button confirm, cancel;
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
    public OptionButton CurrentOption { get; private set; }
    OptionButton previousOption = null;
    public bool allowMultipleChoices;
    public List<OptionButton> selectedOptions = new List<OptionButton>();
    // Start is called before the first frame update
    private void Awake()
    {
        //assignButton.gameObject.SetActive(false);
        
    }
    private void Start()
    {
        cam = Camera.main;
        HouseStructure house = (HouseStructure)owner;
        //changeResponseButton.button.onClick.AddListener(() =>
        //{
        //    ToggleChangeResponsePanel(false);
        //});
        confirm.onClick.AddListener(() =>
        {
            onOptionSelected.Invoke();
            OnMenuDisable();
        });

        cancel.onClick.AddListener(() =>
        {
            ClearChoice();
            OnMenuDisable();

        });
        icon.InitIcon(house.HouseType);
        icon.AddOnClickActions(() =>
        {
            GameManager.Instance.cameraMovement.MoveToHouse(owner.camFocusPos);
            ATC_UIController.Instance.ShowDialog();
            ATC_UIController.Instance.houseDialogManager.StartHouseDialog(icon.iconHouseType,icon.houseDialog);
        });
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
        //ToggleChangeResponsePanel(false);

        ClearOptionButtons();
        ATC_UIController.Instance.ClearAllPanels();

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
        previousOption = null;
        CurrentOption = null;
        if (allowMultipleChoices)
        {
            foreach (var option in selectedOptions)
            {
                option.ToggleOptionSelectState(false);
            }
            selectedOptions.Clear();
        }
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
            SpawnOptionButtons(choice.choiceName/*,choice.isLocked*/);
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

        if (!allowMultipleChoices && CurrentOption != null && text == CurrentOption.GetOptionContent())
        {
            optionButton.ToggleOptionSelectState(true);
        }
        else if (allowMultipleChoices && selectedOptions.Contains(optionButton))
        {
            optionButton.ToggleOptionSelectState(true);
        }
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
        //if (option.needConfirmation)
        //{
        //    //ToggleChangeResponsePanel(true,option);
        //}
        //else
        //{
        if (!allowMultipleChoices)
        {
            if (CurrentOption != null)
            {

                previousOption = CurrentOption;
                previousOption.ToggleOptionSelectState(false);
            }
            CurrentOption = option;
            CurrentOption.ToggleOptionSelectState(true);
            //OnMenuDisable();
            //}

            if (CurrentOption == null) return;
            //onOptionSelected.Invoke();
        }
        else // Handle multiple choices
        {
            // Toggle selection
            if (selectedOptions.Contains(option))
            {
                option.ToggleOptionSelectState(false);
                selectedOptions.Remove(option);
            }
            else
            {
                if (selectedOptions.Count >= 2)
                {
                    var oldestOption = selectedOptions[0];
                    oldestOption.ToggleOptionSelectState(false);
                    selectedOptions.RemoveAt(0);
                }

                // Add the new selection
                option.ToggleOptionSelectState(true);
                selectedOptions.Add(option);
            }

            //onOptionSelected.Invoke();
        }
    }
    public void ApplyBehavior()
    {
        HouseStructure house = (HouseStructure)owner;
        StartCoroutine(house.SpawnCarRoutine());
    }

    //void ToggleChangeResponsePanel(bool state, OptionButton currentOption = null)
    //{
    //    explaination.transform.parent.gameObject.SetActive(state);
    //    options.gameObject.SetActive(!state);

    //    if(state == true)
    //    {
    //        confirmButton.button.onClick.AddListener(() =>
    //        {
    //            currentOption.needConfirmation = false;
    //            OnOptionButtonClicked(currentOption);
    //        });
    //    }
    //    else
    //    {
    //        if(currentOption != null)
    //        {
    //            currentOption.needConfirmation = true;
    //            confirmButton.button.onClick.RemoveAllListeners();
    //        }

    //    }

    //}
}
