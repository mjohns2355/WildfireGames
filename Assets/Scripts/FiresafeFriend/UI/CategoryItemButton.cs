using HappyHouse.HouseSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image icon,BG;
    public Sprite inUseBG, emptyBG;
    [SerializeField] UnityEngine.UI.Outline outline;
    public HousePartType housePartType;
    bool isInUse;
    public HousePartInfo partInfo;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }
    public void InitCategoryItem(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        //gradeText.text = $"Grade {partInfo.grade}";
        icon.sprite = partInfo.icon;
        icon.gameObject.SetActive(true);
        BG.sprite = partInfo != null ? inUseBG : emptyBG;
    }

    public void ResetItem()
    {
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        BG.sprite = emptyBG;
        SetIsInUse(false);
        partInfo = null;
    }
    public void SetIsInUse(bool isInUse)
    {
        this.isInUse = isInUse;
        outline.enabled = isInUse;
        if (!isInUse ) return;
        HH_GameManager.Instance.uiManager.inventoryPanel.UpdateItemDetails(partInfo.materialClass, partInfo.partID);
        //inUseText.gameObject.SetActive(isInUse);
    }

    public void OnButtonClick()
    {
        if (isInUse)
        {
            Debug.Log($"Item {partInfo.partID} is already in use");
            return;
        }
        if(partInfo == null)
        {
            //Debug.Log($"Item {partInfo.partID} is not initialized");
            HH_GameManager.Instance.uiManager.ShowStoreScreen(housePartType);
            return ;
        }

        var player = HH_GameManager.Instance.currentPlayer;
        player.ReplaceHousePartObject(partInfo);
    }
}
