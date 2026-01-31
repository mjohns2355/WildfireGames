using System.Collections.Generic;
using UnityEngine;

public class FYT_Feedback : MonoBehaviour
{
    /* Handles giving player feedback at the end of the game */
    public List<string> finalItemNames;
    private List<GameObject> queuedFeedback;
    private GameObject lastPanel;
    private GameObject currentPanel;
    private int index;
    public GameObject feedbackPanels;
    public GameObject endMenu;

    // Start is called before the first frame update
    void Start()
    {
        finalItemNames = new List<string>(); // used to store the string names of all collected items
        queuedFeedback = new List<GameObject>(); // used to store feedback for each of the items collected by the player
        lastPanel = null; // stores the last viewed panel in order to set it inactive
        currentPanel = null; // stores the current panel that is being viewed
        index = 0; // used to iterate through queuedFeedback

        // Based on the game mode, the type of object stored inside the inventory will be different,
        // and so filling finalItemNames will require two different methods
        if (FYT_SettingsData.isGoBag)
        {
            storeFinalItems();
        } else
        {
            queueList();
        }
        FYT_ScoreCalculator.Calculate(finalItemNames);
        queueFeedback();
    }

    // Used to store items collected by the player in Go Bag Mode
    public void storeFinalItems()
    {
        foreach (GameObject item in FYT_SettingsData.finalInventory)
        {
            // since the items collected by the player are buttons, the text is a child of the game object
            TMPro.TextMeshProUGUI itemText = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            finalItemNames.Add(itemText.text.ToString());
        }   
    }

    // Used to store items collected by the player in Critical List Mode
    public void queueList()
    {
        foreach (GameObject item in FYT_SettingsData.finalInventory)
        {
            // the items collected by the player are the item game objects themselves, so the 
            // string will be taken from the name of the game objects themselves
            finalItemNames.Add(item.name);
        }  
    }

    // Checks which feedback panels are necessary and "queues" them by adding them to the queuedFeedback List
    public void queueFeedback()
    {
        foreach (Transform child in feedbackPanels.transform)
        {
            foreach (string itemName in finalItemNames)
            {
                if (child.gameObject.name == itemName) 
                {
                    queuedFeedback.Add(child.gameObject);
                }
            }
        }
    }

    // The function attached to each "Next ->" button at the bottom of the end and feedback panels
    public void nextFeedback()
    {
        if (currentPanel == null && lastPanel == null && queuedFeedback.Count!=0) // checks if the current panel is the end menu, and thus not currently on a feedback panel
        {
            currentPanel = queuedFeedback[index];
            index++;
            currentPanel.SetActive(true);
        } else if (index == queuedFeedback.Count && index != 0) // checks if there are no more feedback panels
        {
            currentPanel.SetActive(false); // if no more feedback is left to give, set the final panel to inactive
            endMenu.SetActive(true); // and set the restart menu active
        } else if (queuedFeedback.Count==0)
        {
            endMenu.SetActive(true);

        } else // otherwise, move to the next feedback panel in queue
        {
            lastPanel = currentPanel;
            currentPanel = queuedFeedback[index];
            index++;

            lastPanel.SetActive(false);
            currentPanel.SetActive(true);
        }
    }

    // For debugging purposes
    private void printFinalItems() 
    {
        foreach (string item in finalItemNames)
        {
            Debug.Log(item);
        }
    }

    // For debugging purposes
    private void printQueue()
    {
        foreach (GameObject feedback in queuedFeedback)
        {
            Debug.Log(feedback.name);
        }
    }
}
