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
    public Image icon, arrowIndicator;
    public Button cancelPurchase, purchase, cancelWarning, earnMoreMoney, closeBGBtn,cancelRemove,confirmRemove;
    public GameObject purchaseScreen, warningScreen,removeTreeScreen;
    private HousePartInfo partInfo;
    private bool shouldShowStore = true;
    [SerializeField] Sprite upArrow, downArrow;
    private void Start()
    {
        cancelPurchase.onClick.AddListener(OnCancelClicked);
        cancelWarning.onClick.AddListener(OnCancelClicked);
        purchase.onClick.AddListener(OnPurchaseClicked);
        earnMoreMoney.onClick.AddListener(OnEarnMoreMoneyClicked);
        if (HH_GameManager.Instance.isTutorial) return;
        closeBGBtn.onClick.AddListener(OnCancelClicked);
        cancelRemove.onClick.AddListener(OnCancelClicked);
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

    private void CompareMaterialWithCurrentMaterial()
    {
        var currentMaterial = HH_GameManager.Instance.currentPlayer.upgradeClassDictionary[partInfo.housePartType];
        var thisMaterial = partInfo.materialClass;
        if (thisMaterial.Equals(currentMaterial)) return;
        arrowIndicator.sprite = currentMaterial.CompareTo(thisMaterial) < 0 ? downArrow : upArrow;
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

        // for tutorial 
        if (HH_GameManager.Instance.isTutorial)
        {
            HH_GameManager.Instance.uiManager.HideStoreScreen();
        }
    }

    private void OnCancelClicked()
    {
        //Debug.Log("Cancel clicked");
        HH_GameManager.Instance.uiManager.HidePurchasePopup(partInfo,shouldShowStore);

    }

    public void InitPurchasePopup(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        priceText.text = $"Purchase: ${partInfo.price:N0}";
        classText.text = $"Class {partInfo.materialClass}";
        descriptionText.text = partInfo.description;
        icon.sprite = partInfo.icon;
        itemNameText.text = partInfo.partID;
    }

    private void OnDisable()
    {
        shouldShowStore = true;
        warningScreen.SetActive(false);
        purchaseScreen.SetActive(false);
        if (HH_GameManager.Instance.isTutorial) return;
        removeTreeScreen.SetActive(false);
        
        
    }

    public void ShowWarningScreen()
    {
        warningScreen.SetActive(true);
        purchaseScreen.SetActive(false);
        shouldShowStore = false;
        if (removeTreeScreen) removeTreeScreen.SetActive(false);
    }

    public void ShowPurchaseScreen()
    {
        CompareMaterialWithCurrentMaterial();
        purchaseScreen.SetActive(true);
        warningScreen.SetActive(false);
        if (removeTreeScreen) removeTreeScreen.SetActive(false);
        earnMoreMoney.gameObject.SetActive(HH_GameManager.Instance.uiManager.earnMoreMoney.IsActive());
    }

    public void ShowRemoveScreen()
    {
        shouldShowStore = false;
        warningScreen.SetActive(false);
        purchaseScreen.SetActive(false);
        removeTreeScreen.SetActive(true);
    }
}
