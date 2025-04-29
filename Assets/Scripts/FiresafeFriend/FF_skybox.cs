using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_skybox : MonoBehaviour
{
    public Material fireSky, normalSky;
    public void ChangeSky(bool isOnFire)
    {
        if (!isOnFire)
            RenderSettings.skybox = normalSky;
        else
            RenderSettings.skybox = fireSky;
    }
}
