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
    HouseTypeInfo targetHouseInfo;
    HouseType houseType;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    private void SpawnIconButtons(int range)
    {
        for (int i = 1; i < range; i++)
        {
            foreach (var icon in GameManager.Instance.uiController.iconList)
            {
                if (icon.name == ((HouseType)i).ToString())
                {
                    iconButtonPrefab.GetComponent<Image>().sprite = icon;
                }
            }
            var obj = Instantiate(iconButtonPrefab, optionBtns);
            var button = obj.GetComponent<Button>();
            button.onClick.AddListener(OnDetailedPageEnable);

        }
    }

    public void OnDetailedPageEnable()
    {
        houseType = (HouseType)Enum.Parse(typeof(HouseType), EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite.name);
        targetHouseInfo = GameManager.Instance.structureManager.ReturnHouseInfoFor(houseType);

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
    }

    private void OnEnable()
    {
        SpawnIconButtons(Enum.GetValues(typeof(HouseType)).Length);
    }

    void SpawnUnlockedButtons()
    {
        //foreach(var pair in selectedHouseInfo.lockedOptions)
        //{
        //    var button = Instantiate(unlockedButtonPrefab, unlockedBtns).GetComponent<UnlockedButton>();
        //    button.btnText.text = pair.Key;
        //    button.button.onClick.AddListener(OnUnlockedButtonClicked);
        //}        
        
        foreach(var choice in targetHouseInfo.lockedChoices)
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
        foreach (var menu in GameManager.Instance.uiController.contextMenus)
        {
            var house = (HouseStructure)menu.owner;
            if (house.houseType == houseType )
            {
                var choiceName = EventSystem.current.currentSelectedGameObject.GetComponentInParent<UnlockedButton>().btnText.text;
                //Debug.Log(choiceName);
                //menu.optionsAreLocked = false;
                var choice = house.houseInfo.ReturnChoiceByName(choiceName);
                choice.isLocked = false;
                menu.OnMenuEnable();
                break;
            }
        }
        gameObject.SetActive(false);
        
    }
}
