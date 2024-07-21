using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYTmap : MonoBehaviour
{
    public GameObject entrance;
    public GameObject bathroom;
    public GameObject bedroom;
    public GameObject office;
    public GameObject bedroom2;
    public GameObject livingRoom;
    public GameObject kitchen;
    public GameObject kitchen2;
    public GameObject garage;

    public GameObject P_entrance;
    public GameObject P_bathroom;
    public GameObject P_bedroom;
    public GameObject P_office;
    public GameObject P_bedroom2;
    public GameObject P_livingRoom;
    public GameObject P_kitchen;
    public GameObject P_kitchen2;
    public GameObject P_garage;

    public void SetPlayerPos()
    {
        P_entrance.SetActive(false);
        P_bathroom.SetActive(false);
        P_bedroom.SetActive(false);
        P_office.SetActive(false);
        P_bedroom2.SetActive(false);
        P_livingRoom.SetActive(false);
        P_kitchen.SetActive(false);
        P_kitchen2.SetActive(false);
        P_garage.SetActive(false);

        if (entrance.activeInHierarchy)
        {
            P_entrance.SetActive(true);
        }
        else if (bathroom.activeInHierarchy)
        {
            P_bathroom.SetActive(true);
        }
        else if (bedroom.activeInHierarchy)
        {
            P_bedroom.SetActive(true);
        }
        else if (office.activeInHierarchy)
        {
            P_office.SetActive(true);
        }
        else if (bedroom2.activeInHierarchy)
        {
            P_bedroom2.SetActive(true);
        }
        else if (livingRoom.activeInHierarchy)
        {
            P_livingRoom.SetActive(true);
        }
        else if (kitchen.activeInHierarchy)
        {
            P_kitchen.SetActive(true);
        }
        else if (kitchen2.activeInHierarchy)
        {
            P_kitchen2.SetActive(true);
        }
        else if (garage.activeInHierarchy)
        {
            P_garage.SetActive(true);
        }
    }
}
