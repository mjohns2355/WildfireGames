using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HH_UIManager : MonoBehaviour
{
    public StorePanel storePanel;
    public PurchasePopup purchasePopup;
    public Button leftArrow, rightArrow,earnMoreMoney;
    public InventoryUI inventoryPanel;
    public Transform floatingIcons;
    public GameObject startText;
    public FF_QuizPopupUI quizPopup;
    public WarningPopupPanel warningPopup;
    public FF_PlantsMenu plantsMenu;
    public GameObject bubblePrefab;
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
    public void ShowWarningPopup()
    {
        warningPopup.gameObject.SetActive(true);
    }

    public void ShowQuizPopup()
    {
        quizPopup.gameObject.SetActive(true);
        quizPopup.InitQuizPopup();
    }
    public void ShowStoreScreen(HousePartType partType, bool isPublic = false/*, PurchaseFloatingButton clickedButton = null*/)
    {
        if (isPublic)
        {
            Debug.Log("Show store of public fences");
        }
        storePanel.gameObject.SetActive(true);
        //storePanel.SetCurrentPurchaseFloatingButton(clickedButton);
        storePanel.ShowStorePanel(partType,isPublic);
    }

    public void ShowPlantsMenu(FF_DirtMound owner)
    {
        //if (plantsMenu.gameObject.activeSelf) HidePlantsMenu();
        plantsMenu.gameObject.SetActive(true);
        plantsMenu.ShowPlantsMenu(owner);
    }

    public void HidePlantsMenu()
    {
        plantsMenu.ClosePlantsMenu();
    }
    public void HideStoreScreen()
    {
        storePanel.HideStorePanel();
    }

    public void ShowPurchasePopup(HousePartInfo partInfo)
    {
        purchasePopup.gameObject.SetActive(true);
        purchasePopup.InitPurchasePopup(partInfo);
    }

    public void HidePurchasePopup(HousePartInfo partInfo)
    {
        purchasePopup.gameObject.SetActive(false);
        //HH_GameManager.Instance.currentPlayer.ToggleAllPurchaseIcons(true);
        if (partInfo)
        {
            storePanel.gameObject.SetActive(true);
            storePanel.ShowStorePanel(partInfo.housePartType);
        }
            

    }

    public void ToggleInventory(bool state)
    {
        inventoryPanel.gameObject.SetActive(state);
       
        if (!inventoryPanel.inventoryUI.activeInHierarchy) return;
        // make sure the inventory grid is disabled when switching player
        inventoryPanel.inventoryUI.SetActive(false);
    }

    public void OnRoundEnd()
    {
        ToggleInventory(false);
        HideStoreScreen();
        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
    }

    public PurchaseFloatingButton SpawnBubble()
    {
        return Instantiate(bubblePrefab, floatingIcons).GetComponent<PurchaseFloatingButton>();
    }
}
