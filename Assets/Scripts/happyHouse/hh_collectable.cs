using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_collectable : MonoBehaviour
{
    public int debris;

    void OnMouseDown()
    {

        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().Clear(debris);
    }
}
