using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{

    public GameObject car;
    public seatsTaken st;
    public bool clickable = true;
    public float clickTimer;
    private bool clicked = false;
    private float dist;
    private float timer = 0;
    private GameObject oldCar;
    public GameObject drivingText;

    private void Update()
    {
        if (clickable)
        {
            clickTimer += Time.deltaTime;
        }
        if (clicked)
        {
            timer += Time.deltaTime;
            if(timer > dist + 2)
            {
                drivingText.SetActive(false);
                car.SetActive(true);
                st.UpdateSeats();
                GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
                foreach (GameObject m in markers)
                {
                    m.GetComponent<pickup>().clickable = true;
                    m.GetComponent<pickup>().clickTimer = 0;
                }
                Destroy(gameObject);
            }
        }
    }

    private void OnMouseDown()
    {
        if (clickable && clickTimer > 0.9f)
        {
            drivingText.SetActive(true);
            GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
            foreach (GameObject m in markers)
            {
                m.GetComponent<pickup>().clickable = false;
            }
            GameObject[] cars = GameObject.FindGameObjectsWithTag("vehicle");
            foreach (GameObject c in cars)
            {
                if (c.activeInHierarchy)
                {
                    //get distance to current car
                    dist = Vector3.Distance(car.transform.position, c.transform.position)/10f;
                    oldCar = c;
                    c.SetActive(false);
                }
            }

            clicked = true;
        }
        
    }
}
