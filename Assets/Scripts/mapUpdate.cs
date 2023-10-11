using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapUpdate : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject[] maps;
    private int mapCount = 0;
    public GameObject steering;
    public int[] correct;
    private int turnCount;

    public TMPro.TextMeshProUGUI timerText;
    private float timer = 0;

    private bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        mapCount = 0;
        maps[0].SetActive(true);
        steering.SetActive(true);
        for(int i = 1; i < maps.Length; i++)
        {
            maps[i].SetActive(false);
        }
    }
    void Update()
    {
        if (playing)
        {

            timer += Time.deltaTime;
            timerText.text = "Timer: " + (int)timer;
        }
    }

    public void PlayGame()
    {
        timer = 0;
        playing = true;
    }

    public void ResponseTime()
    {
        timer += 2;
    }

    public void ChangeMap(int arrow)
    {
        if(arrow == correct[turnCount])
        {
            turnCount++;
            maps[mapCount].SetActive(false);
            mapCount++;
            if (mapCount < maps.Length)
            {
                maps[mapCount].SetActive(true);
            }
            else
            {
                playing = false;
                winScreen.SetActive(true);
                steering.SetActive(false);
                GameObject[] messages = GameObject.FindGameObjectsWithTag("msgPopup");
                foreach (GameObject m in messages)
                {
                    Destroy(m);
                }
            }
        }
        
    }
}
