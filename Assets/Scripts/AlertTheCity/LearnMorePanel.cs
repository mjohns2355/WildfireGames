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
        houseType = currentSelectedIcon.iconHouseType;;
        targetHouseInfo = GetHouseInfoFor(houseType);

        detailPage.SetActive(true);
        homePage.SetActive(false);

        title.text = "Learn More: " + targetHouseInfo.longerTitle;
        detailPageDescription.text = targetHouseInfo.description;
        SpawnUnlockedButtons();

        
    }

    public void OnDetailedPageDisabled()
    {
        detailPage.SetActive(false);
        homePage.SetActive(true);
    }

    public void OnClickClose()
    {
        if (homePage.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            OnDetailedPageDisabled();
        }
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
        foreach (var choice in targetHouseInfo.lockedChoices)
        {
            if (!choice.isLocked) continue;
            var button = Instantiate(unlockedButtonPrefab, unlockedBtns).GetComponent<UnlockedButton>();
            button.btnText.text = choice.choiceName;
            button.button.onClick.AddListener(OnUnlockedButtonClicked);
        }



    }

    void OnUnlockedButtonClicked()
    {
        if(targetMenu == null) return;

        var choiceName = EventSystem.current.currentSelectedGameObject.GetComponentInParent<UnlockedButton>().btnText.text;
        var choice = targetHouseInfo.ReturnChoiceByName(choiceName);
        choice.isLocked = false;
        targetMenu.OnMenuEnable();
        var iconIsLocked = targetHouseInfo.AllChoicesAreUnlocked();
        targetMenu.icon.ToggleIconState(!iconIsLocked);
        gameObject.SetActive(false);
        if (GameManager.Instance.hasChoseGoodOption) return;
        GameManager.Instance.hasChoseGoodOption = true;
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


}
