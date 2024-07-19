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
    [SerializeField] RectTransform canvasTransform;
    [SerializeField] RectTransform menuTransform;
    [SerializeField] float menuOffset = 120f;
    //public bool selectedBehavior = false;
    Camera cam;
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

        HouseStructure house = (HouseStructure)owner;
        if (house.isMainHouse)
        {
          
            //menu.transform.position = cam.WorldToScreenPoint(owner.menuSpawnPos.position /*+ new Vector3(0,20f,0)*/);
            changeResponseButton.InitOptionButton(this, "Change Response");
            UpdateMenuForHouse(house);

            foreach (var menu in GameManager.Instance.uiController.contextMenus)
            {
                if (menu == this) continue;
                if (!menu.gameObject.activeSelf) continue;
                menu.menu.SetActive(false);
                menu.icon.SetActive(false);
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
            menu.icon.SetActive(true);
        }
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
