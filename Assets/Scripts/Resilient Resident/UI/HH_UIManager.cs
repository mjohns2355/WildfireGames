using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HH_UIManager : MonoBehaviour
{
    public StorePanel storePanel;
    public GameObject bubbleIcon;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStoreScreen(BaseHousePartObject TargetObj)
    {
        storePanel.gameObject.SetActive(true);
        storePanel.ShowStorePanel(TargetObj);
    }

    public void HideStoreScreen()
    {
        //storePanel.SetActive(false);
    }
}
