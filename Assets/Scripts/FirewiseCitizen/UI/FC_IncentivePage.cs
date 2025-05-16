using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FC_IncentivePage : MonoBehaviour
{
    public Transform optionsGrid;
    public GameObject incentiveIconPrefab;
    public GameObject incentivePage, confirmationPage;
    public Button incentiveOne, incentiveTwo;
    List<FC_Incentiveicon> incentiveIcons = new();
    // Start is called before the first frame update
    void Start()
    {
        confirmationPage.SetActive(false);
        
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

    public void ShowConfirmationPage(HouseStructure targetHouse)
    {
        confirmationPage.SetActive(true);
    }
}
