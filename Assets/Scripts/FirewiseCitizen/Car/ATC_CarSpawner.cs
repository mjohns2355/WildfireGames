using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public GameObject horseTrailers;
    public bool hasHorseTrailer = false;
    private void Start()
    {
        StartCoroutine(CarSpawnRoutine());
    }

    private GameObject ReturnACarPrefab()
    {
       var randomIndex = UnityEngine.Random.Range(0, carPrefabs.Length);
        return carPrefabs[randomIndex];
    }

    IEnumerator CarSpawnRoutine()
    {
        //float[] waitTimeVarianceList = { -0.5f, 0.5f };
        //float waitTimeVariance = waitTimeVarianceList[UnityEngine.Random.Range(0, waitTimeVarianceList.Length - 1)];
        float waitTimeVariance = UnityEngine.Random.Range(-1f, 2f);
        yield return new WaitForSeconds(waitTimeVariance);

        if (!hasHorseTrailer)
        {
            Instantiate(ReturnACarPrefab(), transform);
        }
        else
        {
            Instantiate(horseTrailers, transform);
        }
    }
}
