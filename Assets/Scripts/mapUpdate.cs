using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapUpdate : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject map; //map image to update, need to replace with dynamic map system
    public GameObject[] intersections;
    public GameObject carIcon;
    public GameObject steering;
    public int[] correct; //set in inspector for correct order
    private int turnCount;

    public TMPro.TextMeshProUGUI timerText;
    private float timer = 0;

    private bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        map.SetActive(true); //start on first map
        steering.SetActive(true);
    }
    void Update()
    {
        if (playing)
        {
            //update timer
            timer += Time.deltaTime;
            timerText.text = "Timer: " + (int)timer;
            
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            carIcon.GetComponent<io_carIcon>().SetNextTurn(0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            carIcon.GetComponent<io_carIcon>().SetNextTurn(1);
        }
    }

    public void PlayGame()
    {
        timer = 0;
        playing = true;
    }

    public void ResponseTime()
    {
        //when they open message in single player?
    }

    public void ChangeMap(int arrow)
    {
        //store desired next turn, use when intersect next intersection
        //0 left
        //1 right
        carIcon.GetComponent<io_carIcon>().SetNextTurn(arrow);
    }
}
