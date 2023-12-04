using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SD_GameState
{
    Ongoing,
    Paused,
    Ended
}
public class SD_GameSateManager : MonoBehaviour
{
    [SerializeField] private SD_GameState gameState;
    [SerializeField] private float AQI = 0f;
    [SerializeField] private float AQIRate= 0f;
    [SerializeField] private float AQIMax = 100f;
    private static SD_GameSateManager instance;    
    private SD_AQIBar AQIBarHealth;
    [SerializeField] private float timerDuration = 5f;
    private float currentTimer = 0f;
    private int counterToWin = 5;
    private int currentCounter = 0;

    [SerializeField] private GameObject positiveAQI;
    [SerializeField] private GameObject negativeAQI;

    
    public static SD_GameSateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SD_GameSateManager>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        switchGameState(SD_GameState.Paused);
        SD_AQIBar.Instance.SetMaxAQI(AQIMax);
    }

    void Update()
    {
        SD_AQIBar.Instance.SetAQI(AQI);
        switch(gameState)
        {
            case SD_GameState.Paused:
                //Do not tick down on hp
                break;
            case SD_GameState.Ongoing:
                if(AQI >= AQIMax)
                {
                    switchGameState(SD_GameState.Ended);
                    SD_SceneManager.Instance.SetCurrentScene(5);
                    SD_SceneManager.Instance.HUDEnableDisable(false);
                }
                if(currentTimer >= timerDuration && counterToWin == currentCounter)
                {
                    switchGameState(SD_GameState.Ended);
                    SD_SceneManager.Instance.SetCurrentScene(6);
                    SD_SceneManager.Instance.HUDEnableDisable(false);
                }
                if(AQI < .9)
                {
                    TimerCheck();
                }
                else if( AQI > .91)
                {
                    currentTimer = 0f;
                }
                break;
            case SD_GameState.Ended:
                //Bringup Game, reset
                break;

        }
    }
    public void switchGameState(SD_GameState switchTo)
    {
        gameState = switchTo;
    }
    public SD_GameState getGameState()
    {
        return gameState;
    }
    public void AQIMeterIncrease(float increaseAmount)
    {
        
        AQI += increaseAmount;
        if(AQI < 0)
        {
            AQI = 0;
        }
    }
    

    private void TimerCheck()
    {
        currentTimer += Time.deltaTime;
    }
    
    public void PositiveAQINotification()
    {
        Debug.Log("TESTING");
        Animation positiveAnimation = positiveAQI.GetComponent<Animation>();
        if(positiveAnimation != null)
        {
            Debug.Log("TEGeeeee");
            positiveAnimation.Play();
        }
    }

    public void NegativeAQINotification()
    {
        Animation negativeAnimation = negativeAQI.GetComponent<Animation>();
        if(negativeAnimation != null)
        {
            negativeAnimation.Play();
        }
    }

    public void AddToCounter()
    {
        currentCounter++;
    }
    public void RemoveFromCounter()
    {
        currentCounter--;
    }


}   
