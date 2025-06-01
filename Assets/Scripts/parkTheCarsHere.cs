using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parkTheCarsHere : MonoBehaviour
{

    public Transform[] parkingSpots;
    int availableSpots;

    private void Start()
    {
        availableSpots = parkingSpots.Length;
    }
    public bool ParkCar(GameObject car)
    {
        if(availableSpots > 0)
        {
            Debug.Log("park car");
            var parkingSpot = parkingSpots[availableSpots - 1];
            car.transform.position = parkingSpot.position;
            car.transform.rotation = Quaternion.Euler(Vector3.zero);
            availableSpots--;
            return true;
        }
        return false;
    }
}
