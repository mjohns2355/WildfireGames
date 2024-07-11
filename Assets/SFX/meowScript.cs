using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meowScript : MonoBehaviour
{

    private float timer;
    private AudioSource meow;
    public GameObject cat;
    public GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        meow = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = Random.Range(2.5f, 4.5f);
            meow.pitch = Random.Range(0.75f, 1.25f);
            meow.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Carrier")
        {
            door.SetActive(false);
            cat.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
