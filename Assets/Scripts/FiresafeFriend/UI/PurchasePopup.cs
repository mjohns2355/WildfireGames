using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HappyHouse.HouseSystem;
using UnityEngine.UI;
using System;
public class PurchasePopup : MonoBehaviour
{
    public TextMeshProUGUI priceText, classText, itemNameText, descriptionText, moneyWarningText;
    public Image icon;
    public Button cancelPurchase, purchase, cancelWarning, earnMoreMoney;
    public GameObject purchaseScreen, warningScreen;
    private HousePartInfo partInfo;

    private void Start()
    {
        cancelPurchase.onClick.AddListener(OnCancelClicked);
        cancelWarning.onClick.AddListener(OnCancelClicked);
        purchase.onClick.AddListener(OnPurchaseClicked);
        earnMoreMoney.onClick.AddListener(OnEarnMoreMoneyClicked);
    }

    private void OnEnable()
    {
        earnMoreMoney.gameObject.SetActive(HH_GameManager.Instance.uiManager.earnMoreMoney.IsActive());
    }
    private void OnEarnMoreMoneyClicked()
    {
        HH_GameManager.Instance.uiManager.HidePurchasePopup(partInfo);
        HH_GameManager.Instance.uiManager.ShowQuizPopup();
    }

    private void OnPurchaseClicked()
    {
        // insufficient money
        if (!HH_GameManager.Instance.currentPlayer.PurchaseHousePart(partInfo))
        {
            purchaseScreen.SetActive(false);
            warningScreen.SetActive(true);
            return;
        }
        HH_GameManager.Instance.uiManager.HidePurchasePopup(partInfo);
        var player = HH_GameManager.Instance.currentPlayer;
        player.ReplaceHousePartObject(partInfo);

        //test
        HH_GameManager.Instance.uiManager.HideStoreScreen();
    }

    private void OnCancelClicked()
    {
        Debug.Log("Cancel clicked");
        HH_GameManager.Instance.uiManager.HidePurchasePopup(partInfo);
        
    }

    public void InitPurchasePopup(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        priceText.text = $"Cost: ${partInfo.price:N0}";
        classText.text = $"Class {partInfo.partClass}";
        descriptionText.text = partInfo.description;
        icon.sprite = partInfo.icon;
        itemNameText.text = partInfo.partID;
    }

    private void OnDisable()
    {
        warningScreen.SetActive(false);
        purchaseScreen.SetActive(true );
    }

}
