using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FYT_Feedback : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        printFinalItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void printFinalItems()
    {
        foreach (GameObject item in FYT_SettingsData.finalInventory)
        {
            TMPro.TextMeshProUGUI itemText = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            Debug.Log(itemText.text);
        }
        
    }
}
