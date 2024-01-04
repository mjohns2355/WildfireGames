using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_sky : MonoBehaviour
{
    public Material[] phaseSky;
    private int currentSky = 0;


    private void Start()
    {
        RenderSettings.skybox = phaseSky[currentSky];
    }

    public void ChangeSky()
    {
        currentSky++;
        RenderSettings.skybox = phaseSky[currentSky];
        DynamicGI.UpdateEnvironment();
    }
}
