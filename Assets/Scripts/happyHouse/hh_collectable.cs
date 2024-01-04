using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_collectable : MonoBehaviour
{
    public int debris;
    public bool blocker = false;

    void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Dialog") == null)
        {
            GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().Clear(debris);
        } 
    }
}
