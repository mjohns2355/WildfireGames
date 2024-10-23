using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HH_UIManager : MonoBehaviour
{
    public StorePanel storePanel;
    public GameObject bubbleIcon;
    public Button leftArrow, rightArrow;
    // Start is called before the first frame update
    void Start()
    {
        // TODO: add a confirmation pop up
        leftArrow.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.SwitchPlayer("p1");
            
        });
        rightArrow.onClick.AddListener(() =>
        {
            HH_GameManager.Instance.SwitchPlayer("p2");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStoreScreen(BaseHousePartObject TargetObj, PurchaseFloatingButton clickedButton)
    {
        storePanel.gameObject.SetActive(true);
        storePanel.SetCurrentPurchaseFloatingButton(clickedButton);
        storePanel.ShowStorePanel(TargetObj);
    }

    public void HideStoreScreen()
    {
        storePanel.HideStorePanel();
    }

    public void ShowPurchasePopup(HousePartInfo partInfo)
    {
        storePanel.ShowPurchasePopup(partInfo);
    }

    public void HidePurchasePopup()
    {
        storePanel.HidePurchasePopup();
    }
}
