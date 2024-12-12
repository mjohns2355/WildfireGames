using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_skybox : MonoBehaviour
{
    public Material fireSky;

    public void ChangeSky()
    {
        RenderSettings.skybox = fireSky;
    }
}
