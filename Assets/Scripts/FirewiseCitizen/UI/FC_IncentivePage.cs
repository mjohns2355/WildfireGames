using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FC_IncentivePage : MonoBehaviour
{
    public Transform optionsGrid;
    public GameObject incentiveIconPrefab;
    public GameObject incentivePage, confirmationPage;
    public OptionButton incentiveOne, incentiveTwo;
    public Button confirm, cancel,skip;
    List<FC_Incentiveicon> incentiveIcons = new();
    OptionButton currentSelected;
    HouseStructure owner;
    // Start is called before the first frame update
    void Start()
    {
        confirmationPage.SetActive(false);
        confirm.onClick.AddListener(OnIncentiveConfirmed);
        cancel.onClick.AddListener(OnIncentiveCancel);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowIncentivesPage()
    {
        incentivePage.SetActive(true);
        foreach (var houseType in GameManager.Instance.availableHouseTypes)
        {
            var icon = GameManager.Instance.structureManager.houseInfoDict[houseType].houseIcon;
            var owner = GameManager.Instance.structureManager.allMainHouses[houseType];
            var obj = Instantiate(incentiveIconPrefab, optionsGrid);
            var incentiveIcon = obj.GetComponent<FC_Incentiveicon>();
            incentiveIcon.SetUpIcon(owner, icon);
            incentiveIcon.offerButton.onClick.AddListener(()=>
            {
                ShowConfirmationPage(owner);
            });
            incentiveIcons.Add(incentiveIcon);

        }
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
        GameManager.Instance.StartSimulation();
        HideConfirmationPage();
        HideIncentivesPage();
        owner.OnReceivedIncentives();
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
