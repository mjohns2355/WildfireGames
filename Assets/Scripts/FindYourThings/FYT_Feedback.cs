using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class FYT_Feedback : MonoBehaviour
{
    private List<string> finalItemNames;
    private List<GameObject> queuedFeedback;
    private GameObject lastPanel;
    private GameObject currentPanel;
    private int index;
    public GameObject feedbackPanels;
    public GameObject endMenu;

    // Start is called before the first frame update
    void Start()
    {
        finalItemNames = new List<string>();
        queuedFeedback = new List<GameObject>();
        lastPanel = null;
        currentPanel = null;
        index = 0;
        storeFinalItems();
        printFinalItems();
        queueFeedback();
        printQueue();
    }

    public void storeFinalItems()
    {
        foreach (GameObject item in FYT_SettingsData.finalInventory)
        {
            TMPro.TextMeshProUGUI itemText = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            finalItemNames.Add(itemText.text.ToString());
        }   
    }

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

    public void nextFeedback()
    {
        if (currentPanel == null && lastPanel == null)
        {
            currentPanel = queuedFeedback[index];
            index++;
            currentPanel.SetActive(true);
        } else if (index == queuedFeedback.Count) 
        {
            currentPanel.SetActive(false);
            endMenu.SetActive(true);
        } else 
        {
            lastPanel = currentPanel;
            currentPanel = queuedFeedback[index];
            index++;

            lastPanel.SetActive(false);
            currentPanel.SetActive(true);
        }
    }

    private void printFinalItems() 
    {
        foreach (string item in finalItemNames)
        {
            Debug.Log(item);
        }
    }

    private void printQueue()
    {
        foreach (GameObject feedback in queuedFeedback)
        {
            Debug.Log(feedback.name);
        }
    }
}
