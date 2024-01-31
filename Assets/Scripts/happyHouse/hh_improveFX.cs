using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_improveFX : MonoBehaviour
{
    private float timer = 1;
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
            timer = 1;
        }
    }
}
