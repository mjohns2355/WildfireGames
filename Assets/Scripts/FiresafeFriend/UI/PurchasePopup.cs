using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HappyHouse.HouseSystem;
using UnityEngine.UI;
using System;
public class PurchasePopup : MonoBehaviour
{
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public Image icon;
    public Button cancel, purchase;

    private HousePartInfo partInfo;

    private void Start()
    {
        cancel.onClick.AddListener(OnCancelClicked);
        purchase.onClick.AddListener(OnPurchaseClicked);
    }

    private void OnPurchaseClicked()
    {
        HH_GameManager.Instance.currentPlayer.PurchaseHousePart(partInfo);
        HH_GameManager.Instance.uiManager.HidePurchasePopup(partInfo);
        var player = HH_GameManager.Instance.currentPlayer;
        player.ReplaceHousePartObject(partInfo);
    }

    private void OnCancelClicked()
    {
        HH_GameManager.Instance.uiManager.HidePurchasePopup(partInfo);
        
    }

    public void InitPurchasePopup(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        priceText.text = $"Cost: ${partInfo.price}";
        classText.text = $"Class {partInfo.partClass}";
        descriptionText.text = partInfo.description;
        icon.sprite = partInfo.icon;
        itemNameText.text = partInfo.partID;
    }


}
