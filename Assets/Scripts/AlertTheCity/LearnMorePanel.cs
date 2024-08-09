using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LearnMorePanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public GameObject iconButtonPrefab;
    public GameObject unlockedButtonPrefab;
    public GameObject homePage;
    public GameObject detailPage;
    public TextMeshProUGUI detailPageDescription;
    [SerializeField] Transform optionBtns;
    [SerializeField] Transform unlockedBtns;
    //HouseInfo selectedHouseInfo;
    [SerializeField]  HouseTypeInfo targetHouseInfo;
    [SerializeField]  StructureContextMenu targetMenu;
    HouseType houseType;
    HouseIcon currentSelectedIcon;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    private void SpawnIconButtons(int range)
    {
        for (int i = 1; i < range; i++)
        {
            var houseType = (HouseType)i;
            var obj = Instantiate(iconButtonPrefab, optionBtns);
            var icon = obj.GetComponent<HouseIcon>();
            icon.InitIcon(houseType);
            icon.AddOnClickActions(OnDetailedPageEnable);
            var iconIsLocked = GetHouseInfoFor(houseType).AllChoicesAreUnlocked();
            icon.ToggleIconState(!iconIsLocked);
            
        }
    }

    public void OnDetailedPageEnable()
    {
        currentSelectedIcon = EventSystem.current.currentSelectedGameObject.GetComponent<HouseIcon>();
        houseType = currentSelectedIcon.iconHouseType;
        //targetHouseInfo = GameManager.Instance.structureManager.ReturnHouseInfoFor(houseType);
        targetHouseInfo = GetHouseInfoFor(houseType);

        //selectedHouseInfo = new HouseInfo(houseType);
        detailPage.SetActive(true);
        homePage.SetActive(false);
        //title.text = "Learn More: " + selectedHouseInfo.longerTitle;
        title.text = "Learn More: " + targetHouseInfo.longerTitle;


        //detailPageDescription.text = selectedHouseInfo.description;
        detailPageDescription.text = targetHouseInfo.description;
        SpawnUnlockedButtons();

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        homePage.SetActive(true);
        detailPage.SetActive(false);
        for(int i = 0; i < optionBtns.childCount; i++)
        {
            Destroy(optionBtns.GetChild(i).gameObject);
        }
        for(int i = 0;i < unlockedBtns.childCount; i++)
        {
            Destroy(unlockedBtns.GetChild(i).gameObject);
        }

        targetMenu = null;
        targetHouseInfo = null;
    }

    private void OnEnable()
    {
        SpawnIconButtons(Enum.GetValues(typeof(HouseType)).Length);
    }

    void SpawnUnlockedButtons()
    {
        //Debug.Log("Target House Info: " + targetHouseInfo.name);
        foreach (var choice in targetHouseInfo.lockedChoices)
        {
            if (!choice.isLocked) continue;
            var button = Instantiate(unlockedButtonPrefab, unlockedBtns).GetComponent<UnlockedButton>();
            button.btnText.text = choice.choiceName;
            //Debug.Log(choice.choiceName);
            button.button.onClick.AddListener(OnUnlockedButtonClicked);
        }



    }

    void OnUnlockedButtonClicked()
    {
        if(targetMenu == null) return;
        var choiceName = EventSystem.current.currentSelectedGameObject.GetComponentInParent<UnlockedButton>().btnText.text;
        //Debug.Log(choiceName);
        //menu.optionsAreLocked = false;
        //var choice = house.houseInfo.ReturnChoiceByName(choiceName);
        var choice = targetHouseInfo.ReturnChoiceByName(choiceName);
        choice.isLocked = false;
        targetMenu.OnMenuEnable();
        var iconIsLocked = targetHouseInfo.AllChoicesAreUnlocked();
        targetMenu.icon.ToggleIconState(!iconIsLocked);
        //foreach (var menu in GameManager.Instance.uiController.contextMenus)
        //{
        //    var house = (HouseStructure)menu.owner;
        //    if (house.houseType == houseType )
        //    {
                
        //        break;
        //    }
        //}
        gameObject.SetActive(false);
        
    }

    HouseTypeInfo GetHouseInfoFor(HouseType type)
    {
        foreach (var menu in GameManager.Instance.uiController.contextMenus)
        {
            var house = (HouseStructure)menu.owner;
            if (house.HouseType == type)
            {
                targetMenu = menu;
                return house.houseInfo;
            }
        }

        return null;
    }

    //bool CheckIfAllChoicesAreUnlocked(HouseType houseType)
    //{
    //    var houseInfo = GetHouseInfoFor(houseType);
    //    if (houseInfo == null) return false;
    //    var lockedChoicesCount = houseInfo.lockedChoices.Where(x => x.isLocked == true).Count();
    //    //Debug.Log("Check " + houseType + " 's locked choices count "+  lockedChoicesCount);
    //    return lockedChoicesCount == 0;
    //}
}
