using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public GameObject horseTrailers;
    private void Start()
    {

     Instantiate(ReturnACarPrefab(), transform);

    }

    private GameObject ReturnACarPrefab()
    {
       var randomIndex = UnityEngine.Random.Range(0, carPrefabs.Length);
        return carPrefabs[randomIndex];
    }
}
