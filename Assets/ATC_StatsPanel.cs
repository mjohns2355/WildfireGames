using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ATC_StatsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statsText;
    bool simEnds  = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatsText();
    }

    void UpdateStatsText()
    {
        if(simEnds) return;
        float timer = GameManager.Instance.SimTimer;
        if (GameManager.Instance.CurrentLevel == 0)
        {
            statsText.text = "Real-time Stats"+ "\n" 
                + "Timer:" + ConvertTimeToClockFormat(timer) + "\n" 
                +"Cars Evacuated: " + GameManager.Instance.carEvaucated;
        }
        else
        {
            statsText.text = "Real-time Stats" + "\n" 
                + "Timer: " + ConvertTimeToClockFormat(timer) + "\n" 
                +"Cars Evacuated: " + GameManager.Instance.carEvaucated + "\n" 
                +"Houses Destroyed: " + GameManager.Instance.houseDestroyed;
        }

    }

    public void ShowResultText()
    {
        simEnds = true;
        var rect = GetComponent<RectTransform>();
        ATC_UIController.Instance.ClampToWindow(rect, 100);
        var firstCar = Mathf.RoundToInt(GameManager.Instance.firstEvacCarTimeStamp);
        var lastCar = Mathf.RoundToInt(GameManager.Instance.lastEvacCarTimeStamp);
        statsText.text = $"Time of first car evacuated: {firstCar} seconds\r\nTime of final car evacuated:{lastCar}";
    }
    string ConvertTimeToClockFormat(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);  
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        
        return string.Format("{0:00}:{1:00}",minutes, seconds);
    }

    private void OnDisable()
    {
        simEnds  = false;
    }

}
