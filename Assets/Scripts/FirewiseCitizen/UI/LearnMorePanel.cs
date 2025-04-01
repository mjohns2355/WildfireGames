using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    [SerializeField] Button nextButton, backButton;
    //HouseInfo selectedHouseInfo;
    [SerializeField]  HouseTypeInfo targetHouseInfo;
    [SerializeField]  StructureContextMenu targetMenu;
    HouseType houseType;
    HouseIcon currentSelectedIcon;
    int currentDescriptionIndex = 0;

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
    }
    private void SpawnIconButtons()
    {
        foreach (var houseType in GameManager.Instance.availableHouseTypes)
        {
            var obj = Instantiate(iconButtonPrefab, optionBtns);
            var icon = obj.GetComponent<HouseIcon>();
            icon.InitIcon(houseType);
            icon.AddOnClickActions(OnIconClicked);
            //var iconIsLocked = GetHouseInfoFor(houseType).AllChoicesAreUnlocked();
            //icon.ToggleIconState(!iconIsLocked);
        }
    }


    public void OnDetailedPageEnable(HouseType type, string choiceName = null)
    {
        targetHouseInfo = GetHouseInfoFor(type);
        targetMenu = ATC_UIController.Instance.FindMenu(type);

        detailPage.SetActive(true);
        homePage.SetActive(false); 
        //unlockedBtns.gameObject.SetActive(true);
        title.text = "Learn More: " + targetHouseInfo.longerTitle;

        //detailPageDescription.text = targetHouseInfo.description;
        bool isAllUnlocked = targetHouseInfo.AllChoicesAreUnlocked();
        if(choiceName != null)
        {
            currentDescriptionIndex = GetDescriptionIndex(choiceName);
        }
        DisplayCurrentDescription(isAllUnlocked);
        
    }

    int GetDescriptionIndex(string choiceName)
    {
        var choiceEntry = targetHouseInfo.ReturnChoiceByName(choiceName);
        return choiceEntry.index;
        
    }
    public void OnDetailedPageDisabled()
    {
        detailPage.SetActive(false);
        homePage.SetActive(true);
        //for (int i = 0; i < unlockedBtns.childCount; i++)
        //{
        //    Destroy(unlockedBtns.GetChild(i).gameObject);
        //}
        currentDescriptionIndex = 0;
        //unlockedBtns.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }

    public void OnClickClose()
    {
        OnDetailedPageDisabled();
        ATC_UIController.Instance.PopPanel();
        //if (homePage.activeSelf)
        //{
        //    ATC_UIController.Instance.PopPanel();
        //}
        //else
        //{
        //    OnDetailedPageDisabled();
        //}
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

    void SpawnUnlockedButtons(int index)
    {
        //for (int i = 0; i < unlockedBtns.childCount; i++)
        //{
        //    Destroy(unlockedBtns.GetChild(i).gameObject);
        //}

        //foreach (var entry in targetHouseInfo.houseChoicesDict)
        //{
        //    var choice = entry.Value.choice;
        //    var i = entry.Value.index;
        //    if (!choice.isLocked) continue;
        //    if(index == i)
        //    {
        //        var button = Instantiate(unlockedButtonPrefab, unlockedBtns).GetComponent<UnlockedButton>();
        //        button.btnText.text = choice.choiceName;
        //        button.button.onClick.AddListener(OnUnlockedButtonClicked);
        //        break;
        //    }
        //}
        //for (int i = 0; i< targetHouseInfo.lockedChoices.Count; i++)
        //{
        //    var choice = targetHouseInfo.lockedChoices[i];
        //    if (!choice.isLocked) continue;
        //    if (index == i)
        //    {
        //        var button = Instantiate(unlockedButtonPrefab, unlockedBtns).GetComponent<UnlockedButton>();
        //        button.btnText.text = choice.choiceName;
        //        button.button.onClick.AddListener(OnUnlockedButtonClicked);
        //        break;
        //    }
        //}

    }

    //void OnUnlockedButtonClicked()
    //{
    //    if(targetMenu == null) return;
    //    var unlockButton = EventSystem.current.currentSelectedGameObject.GetComponentInParent<UnlockedButton>();
    //    var choiceName = unlockButton.btnText.text;
    //    var choice = targetHouseInfo.ReturnChoiceByName(choiceName).choice;
    //    choice.isLocked = false;
    //    var choseGoodOption = GameManager.Instance.allGoodOptionsChose;
    //    if(!choseGoodOption)
    //    {
    //        GameManager.Instance.allGoodOptionsChose = true;
    //    }
    //    var iconIsLocked = targetHouseInfo.AllChoicesAreUnlocked();
    //    targetMenu.icon.ToggleIconState(!iconIsLocked);
    //    targetMenu.OnMenuEnable();
    //    //gameObject.SetActive(false);
    //}

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

    void DisplayCurrentDescription(bool shouldMergeText)
    {
        Debug.Log("Current Description Index: " + currentDescriptionIndex);
        var descriptions = targetHouseInfo.descriptions;
        if(shouldMergeText)
        {
            string descritption = string.Empty;
            foreach( var desc in descriptions )
            {
                descritption += desc;
            }

            detailPageDescription.text = descritption;
            backButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            unlockedBtns.gameObject.SetActive(false);
            return;
        }

        detailPageDescription.text = descriptions[currentDescriptionIndex];
        if(currentDescriptionIndex >= descriptions.Length - 1 )
        {
            nextButton.gameObject.SetActive(false);
            unlockedBtns.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
        if (currentDescriptionIndex > 0)
        {
            backButton.gameObject.SetActive(true);

        }
        else
        {
            backButton.gameObject.SetActive(false);
        }
        SpawnUnlockedButtons(currentDescriptionIndex);


    }

    void OnNextButtonClick()
    {
        currentDescriptionIndex++;
        DisplayCurrentDescription(false);
    }

    void OnBackButtonClick()
    {
        if(currentDescriptionIndex >  0)
        {
            currentDescriptionIndex--;
            DisplayCurrentDescription(false);
        }


    }
}
