using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ATC_StatsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statsText;

    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatsText();
    }

    void UpdateStatsText()
    {
        if(GameManager.Instance.SimIsEnd) return;
        //rect.transform.localPosition
        float timer = GameManager.Instance.SimTimer;
        if (GameManager.Instance.CurrentLevel == 0)
        {
            statsText.text = "Real-time Stats" + "\n"
                + "Timer:" + ConvertTimeToClockFormat(timer) + "\n"
                + "Cars Evacuated: " + GameManager.Instance.carsEvacuated + "\n";
        }
        else
        {
            statsText.text = "Real-time Stats" + "\n" 
                + "Timer: " + ConvertTimeToClockFormat(timer) + "\n" 
                +"Cars Evacuated: " + GameManager.Instance.carsEvacuated + "\n"
                + "Houses Destroyed: " + GameManager.Instance.housesDestroyed;
        }

    }

    public void ShowResultText()
    {
        
        ATC_UIController.Instance.ClampToWindow(rect, 100);
        var firstCar = Mathf.RoundToInt(GameManager.Instance.firstEvacCarTimeStamp);
        var lastCar = Mathf.RoundToInt(GameManager.Instance.lastEvacCarTimeStamp);
        statsText.text = $"Time of first car evacuated: {firstCar} seconds \r\nTime of final car evacuated:{lastCar} seconds \r\nCars Not Evacuated:{GameManager.Instance.carsNotEvacuated}";
    }
    string ConvertTimeToClockFormat(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);  
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        
        return string.Format("{0:00}:{1:00}",minutes, seconds);
    }

    private void OnDisable()
    {
       
    }

}
