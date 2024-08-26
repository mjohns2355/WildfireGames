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
    public OptionButton selectButton;
    public TextMeshProUGUI explaination;
    public GameObject menu;//ui
    public HouseIcon icon;//ui
    public TextMeshProUGUI title;
    [SerializeField] Transform options;
    public GameObject optionButtonPrefab;
    public Button closeButton;
    public Button assignButton;
    public Structure owner;
    //bool optionsAreLocked = true;
    //[SerializeField] RectTransform canvasTransform;
    [SerializeField] RectTransform menuTransform;
    [SerializeField] float menuOffset = 120f;
    Camera cam;
    public OptionButton CurrentOption { get; private set; }
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
        menu.SetActive(true);
        icon.gameObject.SetActive(false);

        HouseStructure house = (HouseStructure)owner;
        house.OnStructureClick();
        if (house.isMainHouse)
        {
          
            UpdateMenuForHouse(house);

            foreach (var menu in GameManager.Instance.uiController.contextMenus)
            {
                if (menu == this) continue;
                if (!menu.gameObject.activeSelf) continue;
                menu.menu.SetActive(false);
                menu.icon.gameObject.SetActive(false);
            }
        }

    }

    public void OnMenuDisable()
    {
        ClearOptionButtons();
        //StartCoroutine(house.SpawnCarRoutine());
        foreach (var menu in GameManager.Instance.uiController.contextMenus)
        {
            menu.menu.SetActive(false);
            menu.icon.gameObject.SetActive(true);
        }
        owner.StopSturctureClick();
        ToggleChangeResponsePanel(false );
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
        menu.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);

        GameManager.Instance.uiController.ClampToWindow(menuTransform, menuOffset);
    }

    //public void ZoomMenuUI()
    //{
    //    float zoomSpeed = 2f;
    //    float scaler = 1;
    //    float axis = Input.GetAxis("Mouse ScrollWheel");
    //    scaler -= axis * zoomSpeed;
    //    Mathf.Clamp(scaler, 1f, 1.5f);
    //    menuUI.GetComponent<RectTransform>().localScale *= scaler;
    //}
    //public void UpdateText(Dictionary<string,int> structureInfo)
    //{
    //    StringBuilder builder = new StringBuilder();
    //    foreach (var item in structureInfo)
    //    {
    //        builder.AppendLine(item.Key + ":" + item.Value + "\n");
    //    }
    //    title.text = builder.ToString();
    //}

    public void UpdateMenuForHouse(HouseStructure house)
    {
        ClearOptionButtons();
        //var houseInfo = house.info;
        var houseInfo = house.houseInfo;
        title.text = houseInfo.menuTitle;
        foreach (var choice in houseInfo.normalChoices)
        {
            SpawnOptionButtons(choice.choiceName);
        }
        foreach(var choice in houseInfo.lockedChoices)
        {
            var isLocked = choice.isLocked;
            SpawnOptionButtons(choice.choiceName,isLocked);
        }
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


    }
    public void OnOptionButtonClicked(OptionButton option)
    {
        if (option.needConfirmation)
        {
            ToggleChangeResponsePanel(true,option);
        }
        else
        {
            CurrentOption = option;
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
            selectButton.button.onClick.AddListener(() =>
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
                selectButton.button.onClick.RemoveAllListeners();
            }

        }

    }
}
