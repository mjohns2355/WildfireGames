using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_carHonks : MonoBehaviour
{

    private CarAI car;
    private float cooldown = 4;

    // Start is called before the first frame update
    void Start()
    {
        // only 40% cars honks
        if(Random.Range(0,1f) < 0.6f)
        {
            enabled = false;
        }
        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.25f);
        GetComponent<AudioSource>().volume = Random.Range(0.6f, 1);
        car = GetComponentInParent<CarAI>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;

        if (car != null && car.collisionStop && !GetComponent<AudioSource>().isPlaying)
        {
            if(cooldown < 0)
            {
                cooldown = 4;
                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.25f);
                GetComponent<AudioSource>().volume = Random.Range(0.6f, 1);
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
