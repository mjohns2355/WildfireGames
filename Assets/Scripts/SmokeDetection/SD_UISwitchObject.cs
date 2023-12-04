using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_UISwitchObject : MonoBehaviour
{
    public GameObject thisObject;
    public GameObject objectSwitch;
    public GameObject itemNeeded;
    public bool trueForNegative = false;
    public void ObjectSwitch()
    {  
        thisObject.SetActive(false);
        objectSwitch.SetActive(true);
        if(trueForNegative)
        {
            SD_GameSateManager.Instance.NegativeAQINotification();
            SD_GameSateManager.Instance.AddToCounter();
        }
        else
        {
            SD_GameSateManager.Instance.PositiveAQINotification();
            SD_GameSateManager.Instance.RemoveFromCounter();
        }
    }
    public void UseItemToSwitch()
    {
        if(itemNeeded != null) //Checks for Item
        {
            if(SD_Inventory.Instance.CheckItem(itemNeeded) == true)
            {
                SD_Inventory.Instance.RemoveItem(itemNeeded);
                ObjectSwitch();
            }
        }
        else
        {
            ObjectSwitch();
        }
    }
}
