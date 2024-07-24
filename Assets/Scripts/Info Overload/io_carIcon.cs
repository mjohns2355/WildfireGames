using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class io_carIcon : MonoBehaviour
{
    public io_intersection.direction currentDirection;
    private io_intersection.direction desiredDirection;
    public bool stopped = false;
    private RectTransform myRect;
    private int speed = 10;
    private Vector2 pos;

    public io_intersection intersect;
    private io_intersection.direction desiredTurn;
    public bool enterIntersection = false;

    public GameObject steering;
    public io_brakes brakes;

    public io_levelManager levelManager;

    public GameObject leftSignal;
    public GameObject rightSignal;

    public GameObject turnScreen;
    public TextMeshProUGUI turnText;

    private string directionText;
    private string compassText;

    public float stoppedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        pos = myRect.anchoredPosition;
    }

    public void Reverse()
    {
        Debug.Log("Dead End going " + currentDirection);
        if (currentDirection == io_intersection.direction.north)
        {
            desiredDirection = io_intersection.direction.south;
            currentDirection = io_intersection.direction.south;
        }
        else if (currentDirection == io_intersection.direction.south)
        {
            desiredDirection = io_intersection.direction.north;
            currentDirection = io_intersection.direction.north;
        }
        else if (currentDirection == io_intersection.direction.east)
        {

            desiredDirection = io_intersection.direction.west;
            currentDirection = io_intersection.direction.west;
        }
        else if (currentDirection == io_intersection.direction.west)
        {
            desiredDirection = io_intersection.direction.east;
            currentDirection = io_intersection.direction.east;

        }

        Debug.Log("New direction " + currentDirection);
    }

    // Update is called once per frame
    void Update()
    {
        if (levelManager.playing)
        {

            if (brakes.isBraking)
            {
                stopped = true;
                stoppedTime += Time.deltaTime;
                Debug.Log(stoppedTime);
            }
            else
            {
                stopped = false;
            }
            if (enterIntersection)
            {
                steering.SetActive(true);
            }
            if (!stopped)
            {
                pos = myRect.anchoredPosition;
                switch (currentDirection)
                {
                    case io_intersection.direction.east://move right
                        pos.x += Time.deltaTime * speed;
                        break;
                    case io_intersection.direction.west://move left
                        pos.x -= Time.deltaTime * speed;
                        break;
                    case io_intersection.direction.north://move forward
                        pos.y += Time.deltaTime * speed;
                        break;
                    case io_intersection.direction.south://move down
                        pos.y -= Time.deltaTime * speed;
                        break;
                }
            }
            myRect.anchoredPosition = pos;
            if (!stopped && intersect != null && desiredDirection != currentDirection) //check if turning
            {
                foreach (io_intersection.direction d in intersect.directions)
                {
                    if (desiredDirection == d)
                    {//Do the turn
                        myRect.anchoredPosition = intersect.GetComponent<RectTransform>().anchoredPosition;
                        currentDirection = desiredDirection;
                        turnText.text = directionText + " Turn\nHeading " + compassText;
                        turnScreen.SetActive(false);
                        turnScreen.SetActive(true);
                        levelManager.ResetBrakes();
                    }
                }
                enterIntersection = false;
                steering.SetActive(false);

                rightSignal.SetActive(false);
                leftSignal.SetActive(false);

                intersect = null;
            } else if (!enterIntersection && intersect == null)
            {
                steering.SetActive(false);

                rightSignal.SetActive(false);
                leftSignal.SetActive(false);
            }
        }
    }

    public void SetNextTurn(int arrow)
    {
        if(arrow == 0)
        {
            rightSignal.SetActive(false);
            leftSignal.SetActive(true);
            directionText = "Left";
        } else
        {

            rightSignal.SetActive(true);
            leftSignal.SetActive(false);
            directionText = "Right";
        }
        switch (currentDirection)
        {
            case io_intersection.direction.east:
                if(arrow == 0)
                {
                    desiredDirection = io_intersection.direction.north;
                    compassText = "North";
                } else
                {
                    desiredDirection = io_intersection.direction.south;
                    compassText = "South";
                }
                break;
            case io_intersection.direction.west:
                if (arrow == 0)
                {
                    desiredDirection = io_intersection.direction.south;
                    compassText = "South";
                } else
                {
                    desiredDirection = io_intersection.direction.north;
                    compassText = "North";
                }
                break;
            case io_intersection.direction.north:
                if (arrow == 0)
                {
                    desiredDirection = io_intersection.direction.west;
                    compassText = "West";
                } else
                {
                    desiredDirection = io_intersection.direction.east;
                    compassText = "East";
                }
                break;
            case io_intersection.direction.south:
                if (arrow == 0)
                {
                    desiredDirection = io_intersection.direction.east;
                    compassText = "East";
                } else
                {
                    desiredDirection = io_intersection.direction.west;
                    compassText = "West";
                }
                break;
        }
    }
}
