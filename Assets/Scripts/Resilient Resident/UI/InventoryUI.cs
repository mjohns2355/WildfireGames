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
    // Start is called before the first frame update
    void Start()
    {
        var resourceManager = ResourceManager.Instance;

        inventoryButton.onClick.AddListener(() =>
        {
            var state = inventoryUI.activeInHierarchy;
            inventoryUI.SetActive(!state);
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

        var defaultMaterial = categories[0];
        defaultMaterial.bg.enabled = true;
        UpdateInventoryUI(defaultMaterial.category);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInventoryUI(HousePartType partType)
    {
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

    public void UpdateItemDetails(string itemClass , string itemName)
    {
        classText.text = $"Class {itemClass}";
        itemNameText.text = itemName;
    }
    

}
