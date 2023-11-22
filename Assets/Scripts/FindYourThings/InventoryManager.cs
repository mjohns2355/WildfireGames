using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/*
Represents the items currently in the inventory. Should persist across the different panels/"rooms"
and clicking on the textual representation of the item should put the item back into the room.
*/
public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance; // static private variable of the component data type
	public static InventoryManager Instance { get { return _instance; } } // public way to access the private variable

    private List<GameObject> inventoryItems;
    public TMPro.TextMeshProUGUI inventoryText;
    private RectTransform inventoryRect;
    public GameObject itemPrefab;
    private Vector2 offset = new Vector3(0, 25);

	private void Awake() {
        // if there is already a value assigned to the private variable and its not this, destroy this
    	if (_instance != null && _instance != this) {
        	Destroy(this.gameObject);
    	} else { 
        // if there is no value assigned to the private variable, assign this as the reference
        	_instance = this;
        }	
    }

    void Start()
    {
        inventoryRect = inventoryText.gameObject.GetComponent<RectTransform>();
        inventoryItems = new List<GameObject>();
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
        collectable.transform.parent.GetComponent<SceneSetup>().sceneItems.Remove(collectable);
        collectable.SetActive(false);

        newItem.GetComponent<Button>().onClick.AddListener(() => {
            collectable.SetActive(true);
            collectable.transform.parent.GetComponent<SceneSetup>().sceneItems.Add(collectable);
            Destroy(newItem);
            inventoryItems.Remove(newItem);
            updatePlacement();
        });

        TMPro.TextMeshProUGUI itemText = newItem.GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
        itemText.text = item;
        inventoryItems.Add(newItem);

        offset.y += 25;
    }

    private void updatePlacement() 
    {
        Vector2 localOffset = new Vector3(0, 25);
        foreach (GameObject item in inventoryItems)
        {
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector2(0, -localOffset.y);
            localOffset.y += 25;
        }
        offset = localOffset;
    }
}
    

