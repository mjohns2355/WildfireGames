using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        pos = myRect.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (brakes.isBraking)
        {
            stopped = true;
        } else
        {
            stopped = false;
        }
        if (enterIntersection)
        {
            steering.SetActive(true);
        }
        if (!stopped)
        {
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
        if(intersect != null && desiredDirection != currentDirection)
        {
            foreach(io_intersection.direction d in intersect.directions)
            {
                if(desiredDirection == d)
                {
                    currentDirection = desiredDirection;
                }
            }
            enterIntersection = false;
            steering.SetActive(false);
            Destroy(intersect.gameObject);
        }
    }

    public void SetNextTurn(int arrow)
    {
        switch (currentDirection)
        {
            case io_intersection.direction.east:
                if(arrow == 0)
                {
                    desiredDirection = io_intersection.direction.north;
                } else
                {
                    desiredDirection = io_intersection.direction.south;
                }
                break;
            case io_intersection.direction.west:
                if (arrow == 0)
                {
                    desiredDirection = io_intersection.direction.south;
                } else
                {
                    desiredDirection = io_intersection.direction.north;
                }
                break;
            case io_intersection.direction.north:
                if (arrow == 0)
                {
                    desiredDirection = io_intersection.direction.west;
                } else
                {
                    desiredDirection = io_intersection.direction.east;
                }
                break;
            case io_intersection.direction.south:
                if (arrow == 0)
                {
                    desiredDirection = io_intersection.direction.east;
                } else
                {
                    desiredDirection = io_intersection.direction.west;
                }
                break;
        }
    }
}
