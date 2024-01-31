using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_gobag : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameObject.FindGameObjectWithTag("Dialog") == null) //prevent any action when dialog is open
        {
            hh_task car_task = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<hh_level_manager>().tasks[3].GetComponent<hh_task>();
            if (!car_task.complete)
            {
                //find the car and play its animation
                GameObject.FindGameObjectWithTag("vehicle").GetComponent<Animator>().SetTrigger("backup");
                //complete the task
                car_task.DoTask();
            }
            Instantiate(Resources.Load("bagPoof"), transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
