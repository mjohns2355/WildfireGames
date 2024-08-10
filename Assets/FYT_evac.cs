using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_evac : MonoBehaviour
{

    public GameObject evacButton;
    public GameObject bagMenu;
    public bool hasKey = false;
    public GameObject needKeyPopup;

    private void OnMouseUp()
    {
        if (GameObject.FindGameObjectWithTag("BagPanel") == null && hasKey)
        {

            evacButton.SetActive(true);
            bagMenu.SetActive(true);
        } else
        {
            needKeyPopup.SetActive(true);
        }
    }
}
