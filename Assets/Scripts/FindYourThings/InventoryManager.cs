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
    public GameObject itemPrefab;

	private void Awake() {
        // if there is already a value assigned to the private variable and its not this, destroy this
    	if (_instance != null && _instance != this) {
        	Destroy(this.gameObject);
    	} else { 
        // if there is no value assigned to the private variable, assign this as the reference
        	_instance = this;
        }	
        // DontDestroyOnLoad(this.gameObject); // while working with one scene, commenting out for now
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void addItem(string item)
    {
        GameObject newItem = Instantiate(itemPrefab, inventoryText.gameObject.GetComponent<RectTransform>());
        GameObject collectable = GameObject.Find(item);

        collectable.SetActive(false);

        newItem.GetComponent<Button>().onClick.AddListener(() => {
            newItem.SetActive(false);
            collectable.SetActive(true);
            //items.Remove(newItem);
        });

        TMPro.TextMeshProUGUI itemText = newItem.GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
        itemText.text = item;
        //items.Add(newItem); // currently this doesn't work
    }

    private void updatePlacement() 
    {
        foreach (GameObject item in items)
        {

        }
    }
}
