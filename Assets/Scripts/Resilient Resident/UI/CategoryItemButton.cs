using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryItem : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI gradeText, inUseText;
    public Image icon;

    bool isInUse;
    HousePartInfo partInfo;
    public void InitCategoryItem(HousePartInfo partInfo)
    {
        this.partInfo = partInfo;
        gradeText.text = $"Grade {partInfo.grade}";
        icon.sprite = partInfo.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isInUse)
        {
            Debug.Log("It is already in use");
            return;
        }
        var player = HH_GameManager.Instance.currentPlayer;
        var newHouseObject = HH_GameManager.Instance.CreateHousePartObject(partInfo, player);
        player.ReplaceHousePartObject(newHouseObject);
        //Debug.Log($"new part object {newHouseObject.houseNode.housePart}");
        //HH_GameManager.Instance.UIManager.inventoryUI.onCategoryItemButtonClicked.Invoke(newHouseObject);
    }

    public void SetIsInUse(bool isInUse)
    {
        this.isInUse = isInUse;
        inUseText.gameObject.SetActive(isInUse);
    }
}
