using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform categoryButtons;
    public Transform categoryItemButtons;
    public GameObject categoryItemPrefab;
    public GameObject categoryButtonPrefab;
    public Action<BaseHousePartObject> onCategoryItemButtonClicked;
    List<CategoryButton> categories = new List<CategoryButton>();
    
    // Start is called before the first frame update
    void Start()
    {
        var resourceManager = ResourceManager.Instance;

        foreach (var type in resourceManager.allAvailableParts.Keys)
        {
            var categoryButton = Instantiate(categoryButtonPrefab,categoryButtons).GetComponent<CategoryButton>();
            categoryButton.InitCategoryButton(this,type);
            categories.Add(categoryButton);
        }

        UpdateOwnedParts(categories[0].category);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateOwnedParts(HousePartType partType)
    {
        // clear
        for(int i = 0;i < categoryItemButtons.childCount; i++)
        {
            Destroy(categoryItemButtons.GetChild(i).gameObject);
        }
        var player = HH_GameManager.Instance.currentPlayer;

        foreach (var p in player.inventory.ownedParts[partType])
        {
            var categoryItem = Instantiate(categoryItemPrefab,categoryItemButtons).GetComponent<CategoryItem>();
            categoryItem.InitCategoryItem(p);
            bool isInUse = player.PartIsInUse(p);
            categoryItem.SetIsInUse(isInUse);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(categoryItemButtons.GetComponent<RectTransform>());
        
    }

    
}
