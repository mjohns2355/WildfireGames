using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_log : MonoBehaviour
{
    public int log;

    private void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Dialog") == null)
        {
            GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().Chop(log);
        }
    }
}
