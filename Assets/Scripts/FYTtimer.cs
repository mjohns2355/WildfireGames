using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FYTtimer : MonoBehaviour
{
    //360 is original number
    private float timer = 360;
    private int mins;
    private int sec;
    public GameObject timerDisp;
    public TextMeshProUGUI timerText;

    public GameObject warning;
    public GameObject startScreen;
    private bool paused = false;
    public GameObject loseScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!warning.activeInHierarchy && !startScreen.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            mins = (int)timer / 60;
            sec = (int)(timer - mins * 60);
            if(sec < 10)
            {

                timerText.text = mins.ToString() + ":0" + sec.ToString();
            } else
            {

                timerText.text = mins.ToString() + ":" + sec.ToString();
            }
            if (timer < 180)
            {
                if (!timerDisp.activeInHierarchy)
                {
                    timerDisp.SetActive(true);
                    warning.SetActive(true);
                }
            }
            if (timer < 0)
            {
                timerText.text = "Game Over";
                loseScreen.SetActive(true);
            }
        }

    }

    public void UnPause()
    {
        paused = false;
    }
}
