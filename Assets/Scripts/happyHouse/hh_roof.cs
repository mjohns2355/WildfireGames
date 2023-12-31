using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_roof : MonoBehaviour
{
    public int roof;

    private void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().ReplaceRoof(roof);
    }
}
