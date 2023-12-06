using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/*
Represents the items currently in the inventory. Should persist across the different panels/"rooms"
and clicking on the textual representation of the item should put the item back into the room.
*/
public class FYT_InventoryManager : MonoBehaviour
{
    [Header("Backpack Sprites")]
    public GameObject backpack;
    private Image backpackImage;
    public Sprite[] backpackImages;

    [Header("Inventory UI")]
    private List<GameObject> inventoryItems;
    public TMPro.TextMeshProUGUI inventoryText;
    private RectTransform inventoryRect;
    public GameObject itemPrefab;
    public GameObject petPrefab;
    private int offsetAmount = 50;
    private Vector2 offset = new Vector3(0, 50);
    public int bagLimit = 5;
    private int bagSize = 0;
    public bool hasPet = false;

    void Start()
    {
        inventoryRect = inventoryText.gameObject.GetComponent<RectTransform>();
        inventoryItems = new List<GameObject>();
        backpackImage = backpack.GetComponent<Image>();
    }

    public void addItem(string item)
    {
        if (bagSize <= 4)
        {
            GameObject newItem = Instantiate(itemPrefab, inventoryRect);
            RectTransform newRect = newItem.GetComponent<RectTransform>();
            newRect.anchoredPosition = newRect.anchoredPosition - offset;
            GameObject collectable = GameObject.Find(item);
            collectable.transform.parent.GetComponent<FYT_SceneSetup>().sceneItems.Remove(collectable);
            collectable.SetActive(false);

            newItem.GetComponent<Button>().onClick.AddListener(() => {
                collectable.SetActive(true);
                collectable.transform.parent.GetComponent<FYT_SceneSetup>().sceneItems.Add(collectable);
                Destroy(newItem);
                inventoryItems.Remove(newItem);
                bagSize -= 1;
                backpackImage.sprite = backpackImages[bagSize];
                updatePlacement();
            });

            TMPro.TextMeshProUGUI itemText = newItem.GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
            itemText.text = item;
            inventoryItems.Add(newItem);

            offset.y += offsetAmount;
            bagSize += 1;
            backpackImage.sprite = backpackImages[bagSize];
        } else
        {
            Debug.Log("Bag Limit Exceeded");
        }
    }

    public void addPet()
    {
        GameObject petItem = Instantiate(petPrefab, inventoryRect);
        RectTransform petRect = petItem.GetComponent<RectTransform>();
        Vector2 petOffset = new Vector3(50, 0);
        petRect.anchoredPosition = petRect.anchoredPosition - petOffset;
        GameObject pet = GameObject.Find("Pet");
        pet.SetActive(false);
        hasPet = true;
    }

    private void updatePlacement() 
    {
        Vector2 localOffset = new Vector3(0, 50);
        foreach (GameObject item in inventoryItems)
        {
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector2(0, -localOffset.y);
            localOffset.y += offsetAmount;
        }
        offset = localOffset;
    }
}
    

