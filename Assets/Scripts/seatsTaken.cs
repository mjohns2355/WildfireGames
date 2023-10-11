using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seatsTaken : MonoBehaviour
{
    public TMPro.TextMeshProUGUI seats;
    private int occupied = 0;
    public int totalSurvivors = 4;
    public GameObject winScreen;
    private float winTimer = 1f;
    private bool win = false;

    public void UpdateSeats()
    {
        occupied++;
        seats.text = "Seats: " + occupied + "/5";
        if(occupied >= totalSurvivors)
        {
            win = true;
        }
    }

    private void Update()
    {
        if(win)
        {
            winTimer -= Time.deltaTime;
            if(winTimer < 0)
            {

                winScreen.SetActive(true);
                win = false;
                winTimer = 1f;
            }
        }
    }


}
