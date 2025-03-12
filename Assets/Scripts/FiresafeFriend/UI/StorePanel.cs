using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HappyHouse.HouseSystem;
using UnityEngine.UI;
using DG.Tweening;
public class StorePanel : MonoBehaviour
{
    public TextMeshProUGUI typeCategoryText;
    public TextMeshProUGUI budgetText;
    public Transform available;
    public GameObject shopPartIcon;

    [SerializeField]private Button closeButton;
    private HousePartType targetCategory;
    private PurchaseFloatingButton currentButton;
    private HouseManager player;
    private bool isPublic;
    private float currentMoney;

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

    public void ShowStorePanel(HousePartType type, bool isPublic = false)
    {
        ClearIconsInStores();
        this.isPublic = isPublic;
        player = HH_GameManager.Instance.currentPlayer;
        targetCategory = type;
        typeCategoryText.text = type.ToString();
        UpdateBudgetText(player.budgetManager.currentBudget);

        PopulateIconsInStore();
    }

    public void UpdateStorePanel()
    {
        ClearIconsInStores();
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
            var info = Instantiate(p, available);
            //Debug.Log($"{p.name} is public: {isPublic}");
            info.isPublic = isPublic;

            if (player.inventory.PlayerOwnsPart(info))
            {
                //Debug.Log($"Skip {p.name}: player {player.playerTag} has already owned this part");
                Destroy(info);
                continue;
            }

            //tutorial store
            if (HH_GameManager.Instance.isTutorial)
            {
                if (info.partClass != MaterialClass.A)
                {
                    Destroy(info);
                    continue;
                }
            }
            var icon = Instantiate(shopPartIcon,available.transform).GetComponent<PartButton>();
            icon.InitPartIconButton(info);
        }
    }

    void ClearIconsInStores()
    {
        for (int i = 0; i < available.childCount; i++)
        {
            Destroy(available.GetChild(i).gameObject);
        }
    }

    public void UpdateBudgetText(float newMoney, float oldMoney = -1)
    {
        Debug.Log($"Update budget text: {newMoney},{oldMoney}");
        if (oldMoney == -1)
        {
            budgetText.text = $"$ {newMoney:N0}";
            currentMoney = newMoney;
            return;
        }

        // simple animation
        budgetText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 1);
        if(newMoney > oldMoney)
        {
            budgetText.DOColor(Color.green, 0.3f).OnComplete(() => budgetText.DOColor(Color.white, 0.3f));
        }
        else
        {
            budgetText.DOColor(Color.red, 0.3f).OnComplete(() => budgetText.DOColor(Color.white, 0.3f));
        }
        DOTween.To(() => currentMoney, x =>
        {
            currentMoney = x;
            budgetText.text = $"${(int)currentMoney:N0}";
        }, newMoney, 0.5f)
        .SetEase(Ease.OutQuad);
    }

    
}
