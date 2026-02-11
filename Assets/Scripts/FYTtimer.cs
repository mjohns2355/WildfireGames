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
    private FYT_dialogManager warningDialogManager;
    public GameObject startScreen;
    private bool paused = false;
    public GameObject loseScreen;

    private bool warningChecker = false;

    // Start is called before the first frame update
    void Start()
    {
        if (warning != null)
        {
            warningDialogManager = warning.GetComponent<FYT_dialogManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(timer);
        if (!startScreen.activeInHierarchy && !paused)
        {
            //Debug.Log("This is the time: " + timer);
            timer -= Time.deltaTime;
            mins = (int)timer / 60;
            sec = (int)(timer - mins * 60);

            if(sec < 10)
            {
                timerText.text = mins.ToString() + ":0" + sec.ToString();
            } 
            else
            {
                timerText.text = mins.ToString() + ":" + sec.ToString();
            }

            //Test condition to change the warning notif to be faster
            //if (timer < 355 && !warningChecker)
            //if (timer < 10 && !warningChecker)
            if (timer < 180 && !warningChecker)
            {
                paused = true;
                warningChecker = true;
                timerDisp.SetActive(true);
                warning.SetActive(true);
                string key = "alertText"; 
                string translatedWarning = (StringManager.Instance != null) ? StringManager.Instance.GetText(key) : "Warning!";

                if (warningDialogManager != null)
                {
                    warningDialogManager.TriggerWarningDialogue("alertText");
                }
                //warningChecker = true; 
                //startScreen.SetActive(false);
                
            }

            if (timer < 0)
            {
                timer = 0;
                if (StringManager.Instance != null)
                {
                    timerText.text = StringManager.Instance.GetText("gameOverText");
                }
                else
                {
                    timerText.text = "Game Over";
                }
                //timerText.text = "Game Over";
                loseScreen.SetActive(true);
            }
        }
        //Original Yiyang Code
        /*if (!warning.activeInHierarchy && !startScreen.activeInHierarchy)
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
        }*/

    }

    public void UnPause()
    {
        paused = false;
    }
}
