using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HappyHouse.HouseSystem;
using UnityEngine.UI;
public class StorePanel : MonoBehaviour
{
    public TextMeshProUGUI typeCategoryText;
    public TextMeshProUGUI budgetText;
    public Transform available;
    public GameObject shopPartIcon;

    [SerializeField]private Button closeButton;
    private HousePartType targetCategory;
    private PurchaseFloatingButton currentButton;
    private List<PartButton> allShopPartIcons;
    private HouseManager player;

    private void Start()
    {
        closeButton.onClick.AddListener(HideStorePanel);
    }
    public void SetCurrentPurchaseFloatingButton(PurchaseFloatingButton button)
    {
        if (currentButton != null && currentButton != button)
        {
            
            currentButton.ResetButton();
        }
        currentButton = button;

    }

    public void ShowStorePanel(HousePartInfo partInfo)
    {
        ClearIconsInStores();

        player = HH_GameManager.Instance.currentPlayer;
        targetCategory = partInfo.housePartType;
        typeCategoryText.text = partInfo.housePartType.ToString();
        UpdateBudgetText(player.budgetManager.currentBudget);

        PopulateIconsInStore();
    }

    public void HideStorePanel()
    {
        ClearIconsInStores();
        if (currentButton != null)
        {

            currentButton.ResetButton();
        }
        currentButton = null;
        gameObject.SetActive(false);

    }


    void PopulateIconsInStore()
    {
        foreach( var p in ResourceManager.Instance.allAvailableParts[targetCategory])
        {
            if (player.inventory.PlayerOwnsPart(p))
            {
                //Debug.Log($"Skip {p.name}: player {player.playerTag} has already owned this part");
                continue;
            }
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

    public void UpdateBudgetText(float amount)
    {
        budgetText.text = $"$ {amount:N0}";
    }

    
}
