using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class hh_level_manager : MonoBehaviour
{
    public GameObject[] roofs;
    public GameObject[] bushes;
    public GameObject[] logs;
    public GameObject[] signs;

    public string[] taskItems;
    public int[] itemCounts;

    public TextMeshProUGUI[] tasks;
    public TextMeshProUGUI header;

    public hh_task.phase currentPhase;

    public GameObject phaseButton;
    public GameObject evacButton;
    public GameObject replayButton;

    public GameObject happy;
    public GameObject worried;
    public GameObject sad;

    private int completedTasks = 0;

    private void Start()
    {
       for(int i = 0; i < taskItems.Length; i++)
        {
            tasks[i].text = taskItems[i];
            tasks[i].GetComponent<hh_task>().totalCount = itemCounts[i];
            if(tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                tasks[i].gameObject.SetActive(true);
        }
    }

    public void ChangePhase()
    {
        if(currentPhase == hh_task.phase.fireSeason)
        {
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].GetComponent<hh_task>().FailTask();
            }
            currentPhase = hh_task.phase.redflag;
            header.text = "Phase: Red Flag Day";
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].gameObject.SetActive(true);
            }
        } else if (currentPhase == hh_task.phase.redflag)
        {
            signs[0].SetActive(true);
            phaseButton.SetActive(false);
            evacButton.SetActive(true);
            currentPhase = hh_task.phase.evacuation;
            header.text = "Phase: Evacuation Order";
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].gameObject.SetActive(true);
                else
                    tasks[i].GetComponent<hh_task>().FailTask();
            }
        } 
    }

    public void Evacuate()
    {
        replayButton.SetActive(true);
        tasks[5].GetComponent<hh_task>().DoTask();
        for (int i = 0; i < taskItems.Length; i++)
        {
            if (tasks[i].GetComponent<hh_task>().complete)
                completedTasks++;
            else
                tasks[i].GetComponent<hh_task>().FailTask();
        }

        if (completedTasks == taskItems.Length)
            happy.SetActive(true);
        else if (completedTasks > taskItems.Length / 2)
            worried.SetActive(true);
        else
            sad.SetActive(true);
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }


    public void Clear(int debris)
    {
        if (!tasks[0].GetComponent<hh_task>().failed)
        {

            tasks[0].GetComponent<hh_task>().DoTask();
            bushes[debris].SetActive(false);
            Instantiate(Resources.Load("sticks"), bushes[debris].transform.position, bushes[debris].transform.rotation);
        }
    }

    public void ReplaceRoof(int roof)
    {
        if (!tasks[1].GetComponent<hh_task>().failed)
        {
            tasks[1].GetComponent<hh_task>().DoTask();
            roofs[roof].GetComponent<Animator>().SetTrigger("swap");

        }
    }

    public void Chop(int log)
    {
        if (!tasks[2].GetComponent<hh_task>().failed)
        {
            tasks[2].GetComponent<hh_task>().DoTask();
            logs[log].SetActive(false);
            if (log + 1 < logs.Length)
                logs[log + 1].SetActive(true);
        }
    }

    public void PlaceSign()
    {
        tasks[4].GetComponent<hh_task>().DoTask();
        signs[0].SetActive(false);
        signs[1].SetActive(true);
    }
}
