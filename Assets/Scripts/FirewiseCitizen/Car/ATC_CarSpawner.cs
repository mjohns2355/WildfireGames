using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_CarSpawner : MonoBehaviour
{
    public GameObject horseTrailers;
    public bool hasHorseTrailer = false;
    private void Start()
    {
        StartCoroutine(CarSpawnRoutine());
    }

    private GameObject ReturnACarModel()
    {
        var carModels = ATC_AIDirector.Instance.carModels;
        var randomIndex = UnityEngine.Random.Range(0, carModels.Count);
        carModels[randomIndex].transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        return carModels[randomIndex];
    }

    IEnumerator CarSpawnRoutine()
    {
        //float[] waitTimeVarianceList = { -0.5f, 0.5f };
        //float waitTimeVariance = waitTimeVarianceList[UnityEngine.Random.Range(0, waitTimeVarianceList.Length - 1)];
        float waitTimeVariance = UnityEngine.Random.Range(-1f, 2f);
        yield return new WaitForSeconds(waitTimeVariance);

        if (!hasHorseTrailer)
        {
            Instantiate(ReturnACarModel(), transform);
        }
        else
        {
            Instantiate(horseTrailers, transform);
        }
    }
}
