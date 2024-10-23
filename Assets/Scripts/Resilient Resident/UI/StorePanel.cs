using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HappyHouse.HouseSystem;
public class StorePanel : MonoBehaviour
{
    public TextMeshProUGUI typeCategoryText;
    public TextMeshProUGUI playerInfoText;
    public Transform available;
    public GameObject shopPartIcon;
    public PurchasePopup purchasePopup;
    private HousePartType targetCategory;
    private PurchaseFloatingButton currentButton;
    private List<PartButton> allShopPartIcons;
    public void SetCurrentPurchaseFloatingButton(PurchaseFloatingButton button)
    {
        if (currentButton != null && currentButton != button)
        {
            
            currentButton.ResetButton();
        }
        currentButton = button;

    }

    public void ShowPurchasePopup(HousePartInfo partInfo)
    {
        purchasePopup.gameObject.SetActive(true);
        purchasePopup.InitPurchasePopup(partInfo);
    }
    public void HidePurchasePopup()
    {
        purchasePopup.gameObject.SetActive(false);
    }
    public void ShowStorePanel(BaseHousePartObject targetHouseObj)
    {
        ClearIconsInStores();

        var partInfo = targetHouseObj.PartInfo;
        var player = HH_GameManager.Instance.currentPlayer;
        targetCategory = partInfo.housePartType;
        typeCategoryText.text = "Store: " + partInfo.housePartType.ToString().ToUpper();
        playerInfoText.text = $"{player.playerTag}: ${player.budegt.ToString()}";

        PopulateIconsInStore();
    }

    public void HideStorePanel()
    {
        ClearIconsInStores();
        currentButton = null;
        gameObject.SetActive(false);

    }


    void PopulateIconsInStore()
    {
        foreach( var p in ResourceManager.Instance.allAvailableParts[targetCategory])
        {
            var icon = Instantiate(shopPartIcon,available.transform).GetComponent<PartButton>();
            icon.InitPartIconButton(p);
        }
    }

    void ClearIconsInStores()
    {
        for (int i = 0; i < available.childCount; i++)
        {
            Destroy(available.GetChild(i).gameObject);
        }
    }

    
}
