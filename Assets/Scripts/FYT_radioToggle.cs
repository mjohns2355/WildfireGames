using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_radioToggle : MonoBehaviour
{
    public AudioSource radio;

    public void ToggleRagio()
    {
        if (radio.isPlaying)
        {
            radio.Pause();
        } else
        {
            radio.Play();
        }
    }
}
