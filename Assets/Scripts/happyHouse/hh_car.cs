using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_car : MonoBehaviour
{

    private void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Dialog") == null) //prevent any action when dialog is open
        {
            if (GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().currentPhase != hh_task.phase.evacuation)
            {
                GetComponent<Animator>().SetTrigger("backup");
                GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().CarPacked();

            }
        }
            
    }
}
