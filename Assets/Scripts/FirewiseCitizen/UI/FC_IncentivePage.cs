using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FC_IncentivePage : MonoBehaviour
{
    public Transform optionsGrid;
    public GameObject incentiveIconPrefab;
    public GameObject incentivePage, confirmationPage,note;
    public OptionButton incentiveOne, incentiveTwo;
    public Button confirm, cancel,skip,back;
    List<FC_Incentiveicon> incentiveIcons = new();
    OptionButton currentSelected;
    HouseStructure owner;
    // Start is called before the first frame update
    void Start()
    {
        confirmationPage.SetActive(false);
        confirm.onClick.AddListener(OnIncentiveConfirmed);
        cancel.onClick.AddListener(OnIncentiveCancel);
        skip.onClick.AddListener(OnSkipped);
        back.onClick.AddListener(OnCancelled);
    }


    public void ShowIncentivesPage()
    {
        incentivePage.SetActive(true);
        foreach (var houseType in GameManager.Instance.availableHouseTypes)
        {
            if (houseType == HouseType.pet || houseType == HouseType.kids) continue;
            if (!ATC_UIController.Instance.FindMenu(houseType).isSelected) continue;
            var icon = GameManager.Instance.structureManager.houseInfoDict[houseType].houseIcon;
            var owner = GameManager.Instance.structureManager.allMainHouses[houseType];
            var obj = Instantiate(incentiveIconPrefab, optionsGrid);
            var incentiveIcon = obj.GetComponent<FC_Incentiveicon>();
            incentiveIcon.SetUpIcon(owner, icon);
            incentiveIcon.offerButton.onClick.AddListener(()=>
            {
                ShowConfirmationPage(owner);
            });
            incentiveIcon.iconButton.onClick.AddListener(() =>
            {
                ShowConfirmationPage(owner);
            });
            incentiveIcons.Add(incentiveIcon);

        }
        // show note when no icons are available
        note.SetActive(incentiveIcons.Count == 0);
    }


    public void HideIncentivesPage()
    {
        incentivePage.SetActive(false);
        foreach (var icon in incentiveIcons)
        {
            Destroy(icon.gameObject);
        }
        incentiveIcons.Clear();
    }

    public void ShowConfirmationPage(HouseStructure owner)
    {
        this.owner = owner;
        confirm.interactable = currentSelected != null;
        confirmationPage.SetActive(true);
        incentiveOne.InitIncentiveOptions(owner.houseInfo.incentiveOptions[0], this);
        incentiveTwo.InitIncentiveOptions(owner.houseInfo.incentiveOptions[1], this);
    }

    public void HideConfirmationPage()
    {
        confirmationPage.SetActive(false);
        currentSelected?.ToggleOptionSelectState(false);
        currentSelected = null;
    }
    public void OnIncentiveOptionClicked(OptionButton clicked)
    {
        // Deselect previous
        if (currentSelected != null && currentSelected != clicked)
            currentSelected.ToggleOptionSelectState(false);

        // Select new
        currentSelected = clicked;
        currentSelected.ToggleOptionSelectState(true);
        confirm.interactable = currentSelected != null;
    }

    public void OnIncentiveConfirmed()
    {
        GameManager.Instance.ToggleSimStatus(true);
        GameManager.Instance.StartSimulation();
        HideConfirmationPage();
        HideIncentivesPage();
        owner.OnReceivedIncentives();
    }

    public void OnCancelled()
    {
        ATC_UIController.Instance.start.interactable = true;
        HideIncentivesPage();
    }
    public void OnSkipped()
    {
        ATC_UIController.Instance.popUp.SetActive(true);
        GameManager.Instance.StartSimulation();
        HideConfirmationPage();
        HideIncentivesPage();
    }
    public void OnIncentiveCancel()
    {
        HideConfirmationPage();

    }
}
