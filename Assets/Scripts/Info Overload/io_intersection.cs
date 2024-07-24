using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class io_intersection : MonoBehaviour
{
    public enum direction
    {
        north,
        south,
        east,
        west
    }

    public direction[] directions;

    public bool deadEnd = false;
    public bool shelter = false;

    public RectTransform car;

    private RectTransform myRect;
    private bool passed = false;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(Vector2.Distance(myRect.anchoredPosition,car.anchoredPosition) < 5)
        {
            if (deadEnd)
            {
                //turn car around


                car.GetComponent<io_carIcon>().Reverse();

                // GameObject.FindGameObjectWithTag("LevelManager").GetComponent<io_levelManager>().crashScreen.SetActive(true);
            } else if (shelter)
            {
                GameObject.FindGameObjectWithTag("LevelManager").GetComponent<io_levelManager>().winScreen.SetActive(true);
            }
            else
            {
                car.GetComponent<io_carIcon>().intersect = this;
                passed = true;
            }
        }
        else if (Vector2.Distance(myRect.anchoredPosition, car.anchoredPosition) < 40 && !deadEnd && !shelter)
        {
            if (!passed) {
                car.GetComponent<io_carIcon>().enterIntersection = true;
            }
            else
            {
                car.GetComponent<io_carIcon>().intersect = null;
                car.GetComponent<io_carIcon>().enterIntersection = false;
            }

        } else
        {
            passed = false;
        }
    }
}
