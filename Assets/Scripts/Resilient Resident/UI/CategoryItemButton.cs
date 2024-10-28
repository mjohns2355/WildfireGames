using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryItem : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public UnityEngine.UI.Outline outline;

    bool isInUse;
    HousePartInfo partInfo;
    public void InitCategoryItem(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        //gradeText.text = $"Grade {partInfo.grade}";
        icon.sprite = partInfo.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick();
        //var newHouseObject = HH_GameManager.Instance.CreateHousePartObject(partInfo, player);
        //player.ReplaceHousePartObject(newHouseObject);
        
        //Debug.Log($"new part object {newHouseObject.houseNode.housePart}");
        //HH_GameManager.Instance.UIManager.inventoryUI.onCategoryItemButtonClicked.Invoke(newHouseObject);
    }

    public void SetIsInUse(bool isInUse)
    {
        this.isInUse = isInUse;
        outline.enabled = isInUse;
        HH_GameManager.Instance.UIManager.inventoryUI.UpdateItemDetails(partInfo.partClass, partInfo.partID);
        //inUseText.gameObject.SetActive(isInUse);
    }

    public void OnButtonClick()
    {
        if (isInUse)
        {
            Debug.Log("Item is already in use");
            return;
        }
        if(partInfo == null)
        {
            Debug.Log("Item is not initialized");
            return ;
        }
        var player = HH_GameManager.Instance.currentPlayer;
        player.ReplaceHousePartObject(partInfo);
    }
}
