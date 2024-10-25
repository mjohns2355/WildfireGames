using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HH_UIManager : MonoBehaviour
{
    public StorePanel storePanel;
    public PurchasePopup purchasePopup;
    public Button leftArrow, rightArrow;
    public InventoryUI inventoryUI;
    public Button inventoryButton;
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

        inventoryButton.onClick.AddListener(() =>
        {
            var state = inventoryUI.gameObject.activeInHierarchy;
            inventoryUI.gameObject.SetActive(!state);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStoreScreen(HousePartInfo partInfo, PurchaseFloatingButton clickedButton)
    {
        storePanel.gameObject.SetActive(true);
        storePanel.SetCurrentPurchaseFloatingButton(clickedButton);
        storePanel.ShowStorePanel(partInfo);
    }

    public void HideStoreScreen()
    {
        storePanel.HideStorePanel();
    }

    public void ShowPurchasePopup(HousePartInfo partInfo)
    {
        purchasePopup.gameObject.SetActive(true);
        purchasePopup.InitPurchasePopup(partInfo);
        HH_GameManager.Instance.currentPlayer.ToggleAllPurchaseIcons(false);
        storePanel.gameObject.SetActive(false);
    }

    public void HidePurchasePopup(HousePartInfo partInfo)
    {
        purchasePopup.gameObject.SetActive(false);
        HH_GameManager.Instance.currentPlayer.ToggleAllPurchaseIcons(true);
        storePanel.gameObject.SetActive(true);
        storePanel.ShowStorePanel(partInfo);
    }
}
