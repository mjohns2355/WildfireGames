using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SD_OutsideAQI : MonoBehaviour
{
    [SerializeField] private float AQI = 3f;
    [SerializeField] private GameObject mask;
    private float currentTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        if(currentTimer >= SD_GameSateManager.Instance.getAQIRate())
        {
            if(!SD_Inventory.Instance.CheckItem(mask))
            {
                SD_GameSateManager.Instance.AQIMeterIncrease(AQI);
            }
            else
            {
                SD_GameSateManager.Instance.AQIMeterIncrease(AQI/3);
            }
        }
    }
}
