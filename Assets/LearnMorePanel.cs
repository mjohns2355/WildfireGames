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
    HouseInfo selectedHouseInfo;
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
        
        selectedHouseInfo = new HouseInfo(houseType);
        detailPage.SetActive(true);
        homePage.SetActive(false);
        title.text = "Learn More: " + selectedHouseInfo.longerTitle;


        detailPageDescription.text = selectedHouseInfo.description;
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
        foreach(var pair in selectedHouseInfo.lockedOptions)
        {
            var button = Instantiate(unlockedButtonPrefab, unlockedBtns).GetComponent<UnlockedButton>();
            button.btnText.text = pair.Key;
            button.button.onClick.AddListener(OnUnlockedButtonClicked);
        }
        //for(int i = 0; i < selectedHouseInfo.lockedOptions.Count; i++)
        //{
        //    //Debug.Log(info.lockedOptions[i]);
            
            
            
        //}
    }

    void OnUnlockedButtonClicked()
    {
        foreach (var menu in GameManager.Instance.uiController.contextMenus)
        {
            if (((HouseStructure)menu.owner).houseType == houseType )
            {
                menu.optionsAreLocked = false;
                menu.OnMenuEnable();
            }
        }
        gameObject.SetActive(false);
        
    }
}
