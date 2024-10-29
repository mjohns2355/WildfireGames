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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInventoryUI(HousePartType partType)
    {
        // Hide icons
        foreach (var item in items)
        {
            item.icon.sprite = null;
            item.SetIsInUse(false);
        }
        var player = HH_GameManager.Instance.currentPlayer;

        var partDict = player.inventory.ownedParts[partType];
        for (int i = 0;i < partDict.Count; i++)
        {
            var categoryItem = items[i];
            var info = partDict[i];
            items[i].InitCategoryItem(info);
            bool isInUse = player.PartIsInUse(info);
            categoryItem.SetIsInUse(isInUse);
        }
        //foreach (var p in player.inventory.ownedParts[partType])
        //{
        //    var categoryItem = Instantiate(categoryItemPrefab,categoryItemButtons).GetComponent<CategoryItem>();
        //    categoryItem.InitCategoryItem(p);
        //    bool isInUse = player.PartIsInUse(p);
        //    categoryItem.SetIsInUse(isInUse);
        //}

        //LayoutRebuilder.ForceRebuildLayoutImmediate(categoryItemButtons.GetComponent<RectTransform>());
        
    }

    public void UpdateItemDetails(MaterialClass itemClass , string itemName)
    {
        classText.text = itemClass == MaterialClass.Unrated ? $"{itemClass}" : $"Class {itemClass}";
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
