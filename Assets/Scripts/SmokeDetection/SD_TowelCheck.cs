using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_TowelCheck : MonoBehaviour
{
    public GameObject towel;
    public GameObject towelCheckObject;
    private bool checkIt = false;
    
    void Update()
    {
        if(SD_Inventory.Instance.CheckItem(towel) == true && checkIt == false)
        {
            TowelCheck();
            checkIt = true;
        }
    }

    public void TowelCheck()
    {
        towelCheckObject.SetActive(true);
    }
}