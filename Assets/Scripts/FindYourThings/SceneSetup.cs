using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
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
public class SceneSetup : MonoBehaviour
{
    [SerializeField] private int numberOfItems;
    [SerializeField] private List<GameObject> allItems;
    public List<GameObject> sceneItems;
    public GameObject medicine;
    public GameObject glasses;

    // Start is called before the first frame update
    void Start()
    {
        //sceneItems = randomization(numberOfItems);
        sceneItems = allItems; // just for debugging purposes
        if (SettingsData.medsNeeded) {
            sceneItems.Add(medicine);
        }
        if (SettingsData.glassesNeeded) {
            sceneItems.Add(glasses);
        }
        
        setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnEnable() {
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

    void setup() 
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

}
