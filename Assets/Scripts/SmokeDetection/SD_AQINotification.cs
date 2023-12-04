using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_AQINotification : MonoBehaviour
{
    [SerializeField] private GameObject positiveAQI;
    [SerializeField] private GameObject negativeAQI;

    public void PositiveAQINotification()
    {
        Animation positiveAnimation = positiveAQI.GetComponent<Animation>();
        if(positiveAnimation != null)
        {
            positiveAnimation.Play();
        }
    }

    public void NegativeAQINotification()
    {
        Animation negativeAnimation = negativeAQI.GetComponent<Animation>();
        if(negativeAnimation != null)
        {
            negativeAnimation.Play();
        }
    }
}
