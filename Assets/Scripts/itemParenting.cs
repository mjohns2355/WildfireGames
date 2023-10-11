using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemParenting : MonoBehaviour
{

    public bool bottom;
    public GameObject house;
    public GameObject panel;
    public GameObject other;
    private RectTransform myRect;
    private happyHouseItems myItem;

    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        myItem = GetComponent<happyHouseItems>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myItem.isDragging)
        {
            if (bottom)
            {
                if (myRect.position.y > 250)
                {
                    myRect.transform.parent = house.transform;
                    other.SetActive(false);
                }
                else
                {
                    myRect.transform.parent = panel.transform;
                    other.SetActive(true);
                }
            }
            else
            {

                if (myRect.position.x < 1000)
                {
                    myRect.transform.parent = house.transform;
                    other.SetActive(false);
                }
                else
                {
                    myRect.transform.parent = panel.transform;
                    other.SetActive(true);
                }
            }
        }
    }
}
