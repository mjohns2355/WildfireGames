using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_sign : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(GameObject.FindGameObjectWithTag("Dialog") == null)
            GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().PlaceSign();
    }
}
