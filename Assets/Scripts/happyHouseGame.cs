using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class happyHouseGame : MonoBehaviour
{

    public GameObject happy;
    public GameObject sad;

    public bool landscape = true;

    public RectTransform trees;
    public RectTransform buckets;
    public RectTransform ladder;
    public RectTransform trees2;
    public RectTransform buckets2;
    public RectTransform ladder2;
    public RectTransform house;

    private bool happyHouse = true;

    public Slider phaseTimer;
    public TMPro.TextMeshProUGUI phaseText;
    private float timer = 0;
    private bool playing = false;

    public void PlayGame()
    {
        playing = true;
    }

    // TODO: replace phase timer with time increments based on specific items selected
    void Update()
    {

        if (playing)
        {
            timer += Time.deltaTime * 0.4f;
            phaseTimer.value = timer;
            if (timer < 1)
            {
                phaseText.text = "Phase: Safe";
            }
            else if (timer < 3)
            {
                phaseText.text = "Phase: Red Flag Day";
            }
            else if (timer < 6)
            {

                phaseText.text = "Phase: Evacuation";
            }
            else if(timer < 8)
            {
                phaseText.text = "Phase: Late Evacuation";
            } else
            {
                Evacuate();
            }
        }
       
    }

    public void Evacuate()
    {
        playing = false;
        happyHouse = true;
        //detect placed item positions
        if (landscape)
        {
            if (Vector3.Distance(trees.position, house.position) < Screen.width/3f)
            {
                happyHouse = false;
            }
            if (Vector3.Distance(ladder.position, house.position) > Screen.width / 3f || Vector3.Distance(buckets.position, house.position) > Screen.width / 3f)
            {
                happyHouse = false;
            }
        } else
        {
            if (Vector3.Distance(trees2.position, house.position) < Screen.height / 3f)
            {
                happyHouse = false;
            }
            if (Vector3.Distance(ladder2.position, house.position) > Screen.height / 3f || Vector3.Distance(buckets2.position, house.position) > Screen.height / 3f)
            {
                happyHouse = false;
            }
        }
        



        //display emote based on win/lose
        if (happyHouse)
        {
            happy.SetActive(true);
            sad.SetActive(false);
        }
        else
        {
            sad.SetActive(true);
            happy.SetActive(false);
        }
    }
}
