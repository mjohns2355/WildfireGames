using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Represents the items currently in the inventory. Should persist across the different panels/"rooms"
and clicking on the textual representation of the item should put the item back into the room.
*/

public class FYT_InventoryManager : MonoBehaviour
{
    [Header("Backpack Sprites")] // handles changing the backpack sprite in the UI based on the number of items in the bag
    public GameObject backpack; // the backpack Game Object itself
    private Image backpackImage; // initial empty image
    public Sprite[] backpackImages; // contains all possible backpack images, in order of empty to full

    [Header("Inventory UI")] // handles the inventory UI 
    private List<GameObject> inventoryItems; // stores all the buttons created 
    public TMPro.TextMeshProUGUI inventoryText; // for the label; orients the items collected by the player
    private RectTransform inventoryRect;
    public GameObject itemPrefab; // a button prefab that is instantiated to represent an item collected by the player
    private int offsetAmount = 50; // sets the amount of space between inventory items in the UI
    private Vector2 offset = new Vector3(0, 50);
    private int bagLimit = 7; // max number of objects collectable by the player
    private int bagSize = 0;
    public GameObject endMenu; // menu to navigate to once the player is done collecting

    void Start()
    {
        inventoryRect = inventoryText.gameObject.GetComponent<RectTransform>();
        inventoryItems = new List<GameObject>();
        backpackImage = backpack.GetComponent<Image>();
        if (Screen.width > Screen.height) // offset needs to be increased in landscape mode
        {
            offsetAmount = 100;
            offset = new Vector3(0, 100);
        }
    }

    // The function used by all collectables in the Go Bag Game Mode
    /* To add to an item:
        - Ensure collectable item has a Button component
        - Create a new OnClick() entry
        - Drag the game mode (Go Bag Portrait or Go Bag Landscape) into the GameObject slot
        - Select FYT_InventoryManager then select addItem()
        - Write in the exact name of the collectable item (e.g. "First Aid") in the given arg field
    */
    public void addItem(string item)
    {
        if (bagSize <= bagLimit-1) // ensures that an item can only be collected if there is still space in the bag
        {
            GameObject newItem = Instantiate(itemPrefab, inventoryRect); // create the button that represents the collected item, and parent it to the inventory text
            RectTransform newRect = newItem.GetComponent<RectTransform>(); // get the RectTransform of the newly created button in order to position it
            newRect.anchoredPosition = newRect.anchoredPosition - offset; // moves the new button down by the offset amount; this is what makes sure the button appears below any other button that was created before
            GameObject collectable = GameObject.Find(item); // find the item that was collected/clicked on

            // Removes the collectable from the sceneItems List from the FYT_SceneSetup script, which
            // determines which collectables are displayed in each room. Removing it ensures that the 
            // item won't respawn if the player returns to the room later
            collectable.transform.parent.GetComponent<FYT_SceneSetup>().sceneItems.Remove(collectable);
            collectable.SetActive(false); // make the collectable disappear from the scene

            // adds an onClick() event that returns the collectable to the scene when the corresponding button in the inventory UI
            // is clicked
            newItem.GetComponent<Button>().onClick.AddListener(() => {
                collectable.SetActive(true);
                collectable.transform.parent.GetComponent<FYT_SceneSetup>().sceneItems.Add(collectable);
                Destroy(newItem);
                inventoryItems.Remove(newItem);
                bagSize -= 1;
                backpackImage.sprite = backpackImages[bagSize];
                updatePlacement();
            });

            // sets the text of the inventory button to be the name of the item; because of this, the collectable's name
            // must be written like "First Aid" rather than "FirstAid" or "firstAid"
            TMPro.TextMeshProUGUI itemText = newItem.GetComponent<Button>().GetComponentInChildren<TMPro.TextMeshProUGUI>();
            itemText.text = item;
            inventoryItems.Add(newItem); // stores the button in the inventory

            offset.y += offsetAmount; // adds to the offset so that the next added button will appear below the previously added button
            bagSize += 1; // increases the amount of items in the bag
            backpackImage.sprite = backpackImages[bagSize]; // sets the image of the backpack in the UI to represent the amount of items in the bag
        }
    }

    // Helper function called to make sure the inventory buttons in the UI are neatly organized, especially
    // after a button is removed
    private void updatePlacement() 
    {
        Vector2 localOffset = new Vector3(0, 50); // starts from the initial offset
        if (Screen.width > Screen.height) // changes the offset if in landscape mode
        {
            localOffset = new Vector3(0, 100);
        }

        // iterates through each remaining button and manually sets its y value, then updates the 
        // global offset to make sure the next button follows the layout
        foreach (GameObject item in inventoryItems) 
        {
            RectTransform itemRect = item.GetComponent<RectTransform>();
            itemRect.anchoredPosition = new Vector2(0, -localOffset.y);
            if (Screen.width > Screen.height)
            {
                itemRect.anchoredPosition = new Vector2(-20, -localOffset.y);
            }
            localOffset.y += offsetAmount;
        }
        offset = localOffset;
    }

    // Ends the game mode by passing the final inventory of items to the static settings class
    public void endGame()
    {
        FYT_SettingsData.finalInventory = inventoryItems;
        endMenu.SetActive(true);
    }
}
    

