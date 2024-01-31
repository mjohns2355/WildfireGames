using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_flowerbed : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Dialog") == null) //prevent any action when dialog is open
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            Instantiate(Resources.Load("flowerPoof"), transform.position, transform.rotation);
        }
    }
}
