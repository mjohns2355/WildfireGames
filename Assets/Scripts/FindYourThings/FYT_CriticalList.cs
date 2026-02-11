using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FYT_CriticalList : MonoBehaviour
{
    /* The "Inventory Manager" for the Critical List Mode */
    public TextMeshProUGUI timerText;
    public GameObject listOfItems;
    public GameObject medsText;
    public GameObject glassesText;
    public GameObject endMenu;
    private List<GameObject> collectedItems;

    // Start is called before the first frame update
    void Start()
    {
        // Checks if medicine or glasses are needed for the Critical List Game Mode
        if (FYT_SettingsData.medsNeeded == true)
        {
            medsText.SetActive(true);
        }
        if (FYT_SettingsData.glassesNeeded == true)
        {
            glassesText.SetActive(true);
        }
        // Instantiates the list meant to store all items collected by the player.
        collectedItems = new List<GameObject>();
    }

    // The function added to every collectable item in this game mode. 
    /* To add to an item:
        - Ensure collectable item has a Button component
        - Create a new OnClick() entry
        - Drag the game mode (Critical List Portrait or Critical List Landscape) into the GameObject slot
        - Select FYT_CriticalList then select crossOffList()
        - Write in the exact name of the collectable item (e.g. "First Aid") in the given arg field
    */
    public void crossOffList(string item)
    {
        // Adds a time penalty if a item not on the Critical List is collected
        if (FYT_ItemCatalog.IsPenaltyItem(item))
        {
            GetComponent<FYT_Timer>().TimePenalty();
            StartCoroutine(WrongItem());
        }
        // since parameter is used to find the collectable, the argument "item" must match
        // the name of the collectable exactly, and all collectables must be named exactly as they are
        // (e.g. "First Aid" instead of "FirstAid" or "firstAid")
        GameObject collectable = GameObject.Find(item); 

        // The text objects representing the Critical List are all children of the "listOfItems" gameObject
        // This iterates through all of those objects and crosses off the corresponding text object based 
        // on the item that was just collected. 
        foreach (Transform child in listOfItems.transform)
        {
            TextMeshProUGUI childName = child.gameObject.GetComponent<TextMeshProUGUI>();
            if (collectable.name == childName.text)
            {
                childName.fontStyle = FontStyles.Strikethrough;
                childName.color = Color.gray;
            }
        }

        // Removes the collectable from the sceneItems List from the FYT_SceneSetup script, which
        // determines which collectables are displayed in each room. Removing it ensures that the 
        // item won't respawn if the player returns to the room later
        collectable.transform.parent.GetComponent<FYT_SceneSetup>().sceneItems.Remove(collectable);
        collectedItems.Add(collectable); // item gets added to the player's cumultative inventory
        collectable.SetActive(false); // collectable gets hidden once collected
    }

    // Ends the game mode by passing the final inventory of items to the static settings class
    public void endList()
    {
        FYT_SettingsData.finalInventory = collectedItems;
        endMenu.SetActive(true);
    }

    // Makes the timer text briefly turn red when a time penalty is instituted
    IEnumerator WrongItem()
    {
        timerText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        timerText.color = Color.black;
    }
}
