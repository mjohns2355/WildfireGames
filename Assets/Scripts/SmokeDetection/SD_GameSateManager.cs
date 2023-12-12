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
    [SerializeField] private float AQIRateOfUse= 2f;
    [SerializeField] private float AQIMax = 100f;
    private static SD_GameSateManager instance;    
    private SD_AQIBar AQIBarHealth;
    private float currentTimer = 0f;
    // private int counterToWin = 5;
    private int currentCounter = 0;

    [SerializeField] private GameObject positiveAQI;
    [SerializeField] private GameObject negativeAQI;
    
    [SerializeField] private List<GameObject> listOfAQIObjects = new List<GameObject>();

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
        SD_ItemAQI[] itemScripts = Resources.FindObjectsOfTypeAll<SD_ItemAQI>();

        foreach (SD_ItemAQI itemScript in itemScripts)
        {
            if (itemScript.checkIsStarterItem())
            {
                listOfAQIObjects.Add(itemScript.gameObject);
                float aqiPower = itemScript.checkAQIPower();
                AQIRate += aqiPower;
            }
        }
        

        // foreach (GameObject starterItems in listOfAQIObjects) //goes through all of them and adds them up for AQIRate
        // {
        //     SD_ItemAQI itemScript = starterItems.GetComponent<SD_ItemAQI>();
        //     if (itemScript != null)
        //     {
        //         float aqiPower = itemScript.checkAQIPower();
        //         AQIRate += aqiPower;
        //     }
        // }

    }

    void Update()
    {
        switch(gameState)
        {
            case SD_GameState.Paused:
                //Do not tick down on hp
                break;
            case SD_GameState.Ongoing:
                AQIMeterIncrease(AQIRate);
                SD_AQIBar.Instance.SetAQI(AQI);
                if(AQI >= AQIMax)
                {
                    switchGameState(SD_GameState.Ended);
                    SD_SceneManager.Instance.SetCurrentScene(6);
                    SD_SceneManager.Instance.HUDEnableDisable(false);
                }
                if(AQIRate <= 0)
                {
                    switchGameState(SD_GameState.Ended);
                    SD_SceneManager.Instance.SetCurrentScene(7);
                    SD_SceneManager.Instance.HUDEnableDisable(false);
                }
                // if(AQI < .9)
                // {
                //     TimerCheck();
                // }
                // else if( AQI > .91)
                // {
                //     currentTimer = 0f;
                //     //currentTimer >= timerDuration
                // }
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
        currentTimer += Time.deltaTime;
        if(currentTimer >= AQIRateOfUse)
        {
            increaseAmount = increaseAmount * AQIRateOfUse; //Just so i can get a smoother increase in the bar. Its still the rate of number per second :)
            AQI += increaseAmount;
            currentTimer = 0f;
        }
        if(AQI < 0)
        {
            AQI = 0;
        }
    }

    public void AQIBurstIncrease(float increaseAmount)
    {
        AQI += increaseAmount;
    }

    public void addObjectToAQIList(GameObject newItem)
    {
        listOfAQIObjects.Add(newItem);
        SD_ItemAQI itemScript = newItem.GetComponent<SD_ItemAQI>();
        if (itemScript != null)
        {
            float aqiPower = itemScript.checkAQIPower();
            AQIRate += aqiPower;
        }
        currentCounter++;
    }
    public void removeObjectToAQIList(GameObject oldItem)
    {
        listOfAQIObjects.Remove(oldItem);
        SD_ItemAQI itemScript = oldItem.GetComponent<SD_ItemAQI>();
        if (itemScript != null)
        {
            float aqiPower = itemScript.checkAQIPower();
            AQIRate -= aqiPower;
        }
        currentCounter--;
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

    public float getAQIRate()
    {
        return AQIRateOfUse;
    }

}   
