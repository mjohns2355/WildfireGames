using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance; // static private variable of the component data type
	public static InventoryManager Instance { get { return _instance; } } // public way to access the private variable

    private List<GameObject> items;
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
        items = new List<GameObject>();
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

        collectable.SetActive(false);

        newItem.GetComponent<Button>().onClick.AddListener(() => {
            
            collectable.SetActive(true);
            Destroy(newItem);
            items.Remove(newItem);
            updatePlacement(); // for when the array works
        });

        TMPro.TextMeshProUGUI itemText = newItem.GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
        itemText.text = item;
        items.Add(newItem); // currently this doesn't work

        offset.y += 25;
    }

    private void updatePlacement() 
    {
        // hoping to use this to reformat the inventory text, but we need to be able to add each 
        // button to the array first
        Vector2 localOffset = new Vector3(0, 25);
        foreach (GameObject item in items)
        {
            Debug.Log(item);
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector2(0, inventoryRect.anchoredPosition.y - offset.y);
            offset.y += 25;
        }
        offset = localOffset;
    }

    //takes arg int length, for the number of random items chosen from items list
    List<GameObject> randomization(int length)
    {
        Random rand = new Random();
        List<GameObject> randItems;
        randItems = rand.GenerateRandomLoop(items)
        List<GameObject> finalItemList;

        for(int i = 0; i < length; i++)
        {
            finalItemList.add(randItems[i]);
        }

        return finalItemList;
    }

}
