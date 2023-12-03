using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/*
Represents the items currently in the inventory. Should persist across the different panels/"rooms"
and clicking on the textual representation of the item should put the item back into the room.
*/
public class FYT_InventoryManager : MonoBehaviour
{
    private List<GameObject> inventoryItems;
    public TMPro.TextMeshProUGUI inventoryText;
    private RectTransform inventoryRect;
    public GameObject itemPrefab;
    private int offsetAmount = 50;
    private Vector2 offset = new Vector3(0, 50);

    void Start()
    {
        inventoryRect = inventoryText.gameObject.GetComponent<RectTransform>();
        inventoryItems = new List<GameObject>();
    }

    void OnEnable()
    {
        if (inventoryItems != null)
        {
            resetInventory();
            foreach (Transform child in transform)
            {
                if (child.tag == "Room")
                {
                    child.GetComponent<FYT_SceneSetup>().startUp();
                    child.GetComponent<FYT_SceneSetup>().setup();
                }
            }
        }
    }

    void Update()
    {
        
    }

    public void addItem(string item)
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
            updatePlacement();
        });

        TMPro.TextMeshProUGUI itemText = newItem.GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
        itemText.text = item;
        inventoryItems.Add(newItem);

        offset.y += offsetAmount;
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

    private void resetInventory()
    {
        foreach (GameObject item in inventoryItems.ToArray())
        {
            item.GetComponent<Button>().onClick.Invoke();
            Destroy(item);
            inventoryItems.Remove(item);
        }
    }
}
    

