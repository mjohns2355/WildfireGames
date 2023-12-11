using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_OutsideAQI : MonoBehaviour
{
    [SerializeField] private float AQI = 3f;
    private float currentTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        if(currentTimer >= SD_GameSateManager.Instance.getAQIRate())
        {
            SD_GameSateManager.Instance.AQIMeterIncrease(AQI);
        }
    }
}
