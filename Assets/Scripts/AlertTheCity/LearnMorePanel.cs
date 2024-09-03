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
    private void SpawnIconButtons()
    {
        foreach (var houseType in GameManager.Instance.availableHouseTypes)
        {
            var obj = Instantiate(iconButtonPrefab, optionBtns);
            var icon = obj.GetComponent<HouseIcon>();
            icon.InitIcon(houseType);
            icon.AddOnClickActions(OnIconClicked);
            var iconIsLocked = GetHouseInfoFor(houseType).AllChoicesAreUnlocked();
            icon.ToggleIconState(!iconIsLocked);
        }
    }


    public void OnDetailedPageEnable(HouseType type)
    {
        targetHouseInfo = GetHouseInfoFor(type);
        targetMenu = ATC_UIController.Instance.FindMenu(type);
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
        for (int i = 0; i < unlockedBtns.childCount; i++)
        {
            Destroy(unlockedBtns.GetChild(i).gameObject);
        }
    }

    public void OnClickClose()
    {
        if (homePage.activeSelf)
        {
            ATC_UIController.Instance.PopPanel();
        }
        else
        {
            OnDetailedPageDisabled();
        }
    }
    private void OnDisable()
    {
        //homePage.SetActive(true);
        //detailPage.SetActive(false);
        for(int i = 0; i < optionBtns.childCount; i++)
        {
            Destroy(optionBtns.GetChild(i).gameObject);
        }

        OnDetailedPageDisabled();
        targetMenu = null;
        targetHouseInfo = null;
    }

    private void OnEnable()
    {
        SpawnIconButtons();
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
        var choseGoodOption = GameManager.Instance.choseGoodOption;
        if(!choseGoodOption)
        {
            GameManager.Instance.choseGoodOption = true;
        }
        var iconIsLocked = targetHouseInfo.AllChoicesAreUnlocked();
        targetMenu.icon.ToggleIconState(!iconIsLocked);
        targetMenu.OnMenuEnable();
        //gameObject.SetActive(false);
    }

    HouseTypeInfo GetHouseInfoFor(HouseType type)
    {
       return GameManager.Instance.structureManager.ReturnHouseInfoFor(type);
    }

    void OnIconClicked()
    {
        currentSelectedIcon = EventSystem.current.currentSelectedGameObject.GetComponent<HouseIcon>();
        houseType = currentSelectedIcon.iconHouseType;

        OnDetailedPageEnable(houseType);
    }
}
