using UnityEngine;
using TMPro;

public class FYT_Timer : MonoBehaviour
{
    /* Serves as the timer for the Critical List Game Modes */
    public GameObject EndMenu;
    public float timeLimit = 60; // time limit for the game mode; set to 60 by default
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
        timePassed -= Time.deltaTime; // decreases time by 1/60th of a second per frame
        UpdateTimeUI(timePassed);

        // Ends the game once time is out
        if (timePassed <= 0)
        {
            TimerOver();
        }
        
    }

    // Helper function that formats the time remaining in seconds and displays it in the UI
    void UpdateTimeUI(float time) 
    {
        timerText.text = "Time Left: " + Mathf.Max(0, Mathf.Ceil(time));
    } 

    // Function for when the player runs out of time
    void TimerOver()
    {
        GetComponent<FYT_CriticalList>().endList();
        gameObject.SetActive(false);
    }

    // Institutes a penalty
    public void TimePenalty()
    {
        timePassed -= 20;
    }
}
