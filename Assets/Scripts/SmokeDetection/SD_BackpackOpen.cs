using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_BackpackOpen : MonoBehaviour
{
    public GameObject inventoryUI;

    public void OpenInventoryUI()
    {
        if(inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            inventoryUI.SetActive(true);
        }
    }
}
