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

    public RectTransform car;

    private RectTransform myRect;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(Vector2.Distance(myRect.anchoredPosition,car.anchoredPosition) < 5)
        {
            car.GetComponent<io_carIcon>().intersect = this;
        } else if(car.GetComponent<io_carIcon>().intersect == this)
        {
            car.GetComponent<io_carIcon>().intersect = null;
        }
        else if (Vector2.Distance(myRect.anchoredPosition, car.anchoredPosition) < 40)
        {
            car.GetComponent<io_carIcon>().enterIntersection = true;
        }
    }
}
