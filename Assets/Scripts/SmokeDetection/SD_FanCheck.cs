using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_FanCheck : MonoBehaviour
{
    public GameObject fan;
    public GameObject fanCheckObject;
    private bool checkIt = false;
    
    void Update()
    {
        if(SD_Inventory.Instance.CheckItem(fan) == true && checkIt == false)
        {
            FanCheck();
            checkIt = true;
        }
    }

    public void FanCheck()
    {
        fanCheckObject.SetActive(true);
    }
}
