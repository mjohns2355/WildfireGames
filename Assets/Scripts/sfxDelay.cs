using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfxDelay : MonoBehaviour
{

    public AudioSource sfx;
    private float delay;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 1f) < 0.5f)
        {
            this.enabled = false;
        }
        delay = Random.Range(0.2f, 1f);
        sfx.pitch = Random.Range(0.8f, 0.95f);
        sfx.volume -= Random.Range(0, 0.4f);

    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;
        if(delay < 0)
        {
            sfx.Play();
            this.enabled = false;
        }
    }
}
