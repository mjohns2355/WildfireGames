using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
The idea for this script is that it's attached to each "room" that we have (each
will be a panel with the associated background) and each will have a list with every possible
item available in the room. 
Depending on how our items are set up, the OnEnable will use the randomized list to populate
the room with the randomly selected items. 
Currently, this script assumes that all items will be put into the scene, and will be set
inactive to start.
*/
public class FYT_SceneSetup : MonoBehaviour
{
    [SerializeField] private int numberOfItems;
    [SerializeField] private List<GameObject> allItems;
    public GameObject livingRoom;
    public List<GameObject> sceneItems;
    public GameObject medicine;
    public GameObject glasses;

    // Start is called before the first frame update
    void Start()
    {
        startUp();
        setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnEnable() {
        // Debug.Log("enabled"); 
        setup();
    }

    //takes arg int length, for the number of random inventoryItems chosen from inventoryItems list
    List<GameObject> randomization(int length)
    {
        System.Random rand = new System.Random();
        List<GameObject> randItems = new List<GameObject>(allItems);
        List<GameObject> finalItemList = new List<GameObject>();

        for (int i = randItems.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            GameObject temp = randItems[i];
            randItems[i] = randItems[j];
            randItems[j] = temp;
        }

        for(int i = 0; i < length; i++)
        {
            finalItemList.Add(randItems[i]);
        }

        return finalItemList;
    }

    public void setup() 
    {
        foreach (GameObject item in sceneItems)
        {
            item.SetActive(true);
        }
    }

    public void printItems()
    {
        // for debug purposes
        foreach (GameObject item in sceneItems)
        {
            Debug.Log(item);
        }
    }

    public void startUp()
    {
        // Debug.Log(FYT_SettingsData.medsNeeded);
        // Debug.Log(FYT_SettingsData.glassesNeeded);
        foreach (GameObject item in allItems)
        {
            item.SetActive(false);
        }
        sceneItems = randomization(numberOfItems);
        // sceneItems = allItems; // just for debugging purposes
        if (FYT_SettingsData.medsNeeded && livingRoom.activeSelf) {
            sceneItems.Add(medicine);
        } else if (FYT_SettingsData.medsNeeded==false && livingRoom.activeSelf)
        {
            medicine.SetActive(false);
        }
        if (FYT_SettingsData.glassesNeeded && livingRoom.activeSelf) {
            sceneItems.Add(glasses);
        } else if (FYT_SettingsData.glassesNeeded==false && livingRoom.activeSelf)
        {
           glasses.SetActive(false);
        }
    }

}
