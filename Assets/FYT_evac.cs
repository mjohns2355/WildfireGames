using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FYT_evac : MonoBehaviour
{

    public GameObject evacButton;
    public GameObject bagMenu;

    private void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("BagPanel") == null)
        {

            evacButton.SetActive(true);
            bagMenu.SetActive(true);
        }
    }
}
