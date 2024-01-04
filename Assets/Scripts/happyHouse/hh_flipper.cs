using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_flipper : MonoBehaviour
{
    public GameObject camLand;
    public GameObject camPort;

    private void OnRectTransformDimensionsChange()
    {
        if (Screen.width < Screen.height)
        {
            if (camLand != null && camPort != null)
            {
                camLand.SetActive(false);
                camPort.SetActive(true);
            }
        }
        else
        {
            if (camLand != null && camPort != null)
            {
                camLand.SetActive(true);
                camPort.SetActive(false);
            }
        }
    }

    }
