using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_car : MonoBehaviour
{

    private void OnMouseDown()
    {
        if(GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().currentPhase != hh_task.phase.evacuation)
        {
            GetComponent<Animator>().SetTrigger("backup");
            if (!GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().tasks[3].GetComponent<hh_task>().complete)
                GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().tasks[3].GetComponent<hh_task>().DoTask();
            else
                GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().tasks[3].GetComponent<hh_task>().UndoTask();
            //TODO: undo task if pulled in forward
        }
    }
}
