using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_carHonks : MonoBehaviour
{

    private CarAI car;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.25f);
        GetComponent<AudioSource>().volume = Random.Range(0.6f, 1);
       // car = GetComponentInParent<CarAI>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (car.collisionStop && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.25f);
            GetComponent<AudioSource>().volume = Random.Range(0.6f, 1);
            GetComponent<AudioSource>().Play();
        }*/
    }
}
