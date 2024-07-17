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
    public Action<OptionButton> onOptionSelected;
    public GameObject menuUI;
    public OptionButton changeResponseButton;
    public TextMeshProUGUI explaination;
    public GameObject menu;//ui
    public GameObject icon;//ui
    public TextMeshProUGUI title;
    [SerializeField] Transform options;
    public GameObject optionButtonPrefab;
    public Button closeButton;
    public Button assignButton;
    public Structure owner;
    public bool optionsAreLocked = true;
    //public bool selectedBehavior = false;
    [SerializeField] Camera cam;
    // Start is called before the first frame update
    private void Awake()
    {
        assignButton.gameObject.SetActive(false);
        
    }
    private void Start()
    {
        cam = Camera.main;
        HouseStructure house = (HouseStructure)owner;


        foreach (var iconSprite in GameManager.Instance.uiController.iconList)
        {
            if (iconSprite.name == house.houseType.ToString())
            {
                icon.GetComponent<Image>().sprite = iconSprite;
            }
        }

 
    }

    public void OnMenuEnable()
    {
        if(owner == null) return;
        menu.SetActive(true);
        icon.SetActive(false);

        var panelRectTransform = menu.GetComponent<RectTransform>();
        var parentRectTransform = menu.GetComponentInParent<RectTransform>();
        GameManager.Instance.uiController.ClampToWindow(icon.transform.position, panelRectTransform, parentRectTransform);

        HouseStructure house = (HouseStructure)owner;
        if (house.isMainHouse)
        {
            changeResponseButton.InitOptionButton(this, "Change Response");
            UpdateMenuForHouse(house);
        }
    }

    public void OnMenuDisable()
    {
        ClearOptionButtons();
        //StartCoroutine(house.SpawnCarRoutine());
        menu.SetActive(false);
        icon.SetActive(true);
        owner.StopSturctureClick();
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
        menuUI.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);
        //icon.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position);

        //ZoomMenuUI();
    }

    public void ZoomMenuUI()
    {
        float zoomSpeed = 2f;
        float scaler = 1;
        float axis = Input.GetAxis("Mouse ScrollWheel");
        scaler -= axis * zoomSpeed;
        Mathf.Clamp(scaler, 1f, 1.5f);
        menuUI.GetComponent<RectTransform>().localScale *= scaler;
    }
    public void UpdateText(Dictionary<string,int> structureInfo)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var item in structureInfo)
        {
            builder.AppendLine(item.Key + ":" + item.Value + "\n");
        }
        title.text = builder.ToString();
    }

    public void UpdateMenuForHouse(HouseStructure house)
    {
        ClearOptionButtons();
       var houseInfo = house.info;
        title.text = houseInfo.menuTitle;
        foreach (var option in houseInfo.normalOptions)
        {
            SpawnOptionButtons(option);
        }
        foreach(var option in houseInfo.lockedOptions)
        {
            SpawnOptionButtons(option.Key,optionsAreLocked);
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

    public void OnClickGoodOptionButton(bool isGoodOption)
    {
        if (isGoodOption)
        {
            changeResponseButton.gameObject.SetActive(true);
            explaination.gameObject.SetActive(true);
            options.gameObject.SetActive(false);
        }
        else
        {
            OnMenuDisable();
        }


    }

    public void ApplyBehavior()
    {
        HouseStructure house = (HouseStructure)owner;
        StartCoroutine(house.SpawnCarRoutine());
    }


}
