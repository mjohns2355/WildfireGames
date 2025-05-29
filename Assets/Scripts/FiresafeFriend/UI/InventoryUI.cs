using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform categoryButtons;
    public Transform categoryItemButtons;
    public GameObject categoryItemPrefab;
    public GameObject categoryButtonPrefab;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI itemNameText;
    public Action<BaseHousePartObject> onCategoryItemButtonClicked;
    public Button inventoryButton;
    public GameObject inventoryUI;
    public List<CategoryButton> categories = new List<CategoryButton>();
    List<InventoryItem> items = new List<InventoryItem>();
    CategoryButton defaultCategory;
    // Start is called before the first frame update
    void Start()
    {
        var resourceManager = ResourceManager.Instance;

        inventoryButton.onClick.AddListener(() =>
        {
            var state = inventoryUI.activeInHierarchy;
            ToggleInventory(!state);
        });

        // Spawn Inventory Items (max 4)
        for (int i = 0; i < categoryItemButtons.childCount; i++)
        {
            var item = categoryItemButtons.GetChild(i).GetComponent<InventoryItem>();
            items.Add(item);
        }

        // Spawn Category Buttons (roof,wall,etc)
        foreach (var type in resourceManager.allAvailableParts.Keys)
        {
            
            var categoryButton = Instantiate(categoryButtonPrefab, categoryButtons).GetComponent<CategoryButton>();
            categoryButton.InitCategoryButton(this, type);
            categories.Add(categoryButton);
        }

        defaultCategory = categories[0];
    }


    public void UpdateInventoryUI(HousePartType partType, bool isPublic = false)
    {
        // Hide icons
        foreach (var item in items)
        {
            item.icon.sprite = null;
            item.icon.gameObject.SetActive(false);
            item.SetIsInUse(false);
            item.housePartType = partType;
            item.partInfo = null;
        }
        var player = HH_GameManager.Instance.currentPlayer;
        var partDict = isPublic ? player.inventory.ownedPublicParts[partType] : player.inventory.ownedParts[partType];
        //var partDict = player.inventory.ownedParts[partType];
        for (int i = 0;i < partDict.Count; i++)
        {
            var inventoryItem = items[i];
            var info = partDict[i];
            //Debug.Log($"info {i} = {info.partID}");
            items[i].InitCategoryItem(info);
            bool isInUse = player.PartIsInUse(info);
            inventoryItem.SetIsInUse(isInUse);
        }
        
    }

    public void UpdateItemDetails(MaterialClass itemClass , string itemName)
    {
        classText.text = $"Class {itemClass}";
        itemNameText.text = itemName;
    }
    
    public void ToggleInventory(bool state)
    {
        if(state == true)
        {
            defaultCategory.bg.enabled = true;
            UpdateInventoryUI(defaultCategory.category);
        }
        inventoryUI.SetActive(state);
    }
}
