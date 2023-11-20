using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SD_ItemAQI : MonoBehaviour
{
    [SerializeField] private float AQIPower = .5f;
    [SerializeField] private float AQITickRate = .5f;    
    private float timerCount = 0.0f;
    private SD_GameState currentState;

    void Update()
    {
        currentState = SD_GameSateManager.Instance.getGameState();
        if(gameObject.activeSelf && currentState == SD_GameState.Ongoing)
        {
            timerCount += Time.deltaTime;
            if(timerCount >= AQITickRate)
            {
                SD_GameSateManager.Instance.AQIMeterIncrease(AQIPower);
                timerCount = 0.0f;
            }
        }
    }
}
