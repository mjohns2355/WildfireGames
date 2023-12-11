using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SD_ItemAQI : MonoBehaviour
{
    [SerializeField] private float AQIPower = .5f;
    // [SerializeField] private float AQITickRate = .5f;    
    [SerializeField] private bool starterItem = false;
    private SD_GameState currentState;
    public bool checkIsStarterItem()
    {
        if(starterItem)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float checkAQIPower()
    {
        return AQIPower;
    }
}
