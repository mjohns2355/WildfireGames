using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HH_UIManager : MonoBehaviour
{
    public GameObject storePanel;
    public GameObject bubbleIcon;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStoreScreen()
    {
        storePanel.SetActive(true);
    }

    public void HideStoreScreen()
    {
        storePanel.SetActive(false);
    }
}
