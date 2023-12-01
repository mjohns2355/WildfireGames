using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FYT_Timer : MonoBehaviour
{
    public GameObject EndMenu;
    public float timeLimit = 60;
    public TextMeshProUGUI timerText;
    private float timePassed;
    // Start is called before the first frame update
    void Start()
    {
        timePassed = timeLimit;
        UpdateTimeUI(timePassed);   
    }

    void OnEnable() 
    {
        timePassed = timeLimit;
        UpdateTimeUI(timePassed); 
    }

    // Update is called once per frame
    void Update()
    {
        timePassed -= Time.deltaTime;
        UpdateTimeUI(timePassed);

        if (timePassed <= 0)
        {
            TimerOver();
        }
        
    }

    void UpdateTimeUI(float time) 
    {
        timerText.text = "Time Remaining: " + Mathf.Max(0, Mathf.Ceil(time));
    } 

    void TimerOver()
    {
        EndMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
