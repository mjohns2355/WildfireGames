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
    public GameObject neutral;
    public GameObject worried;
    public GameObject sad;

    public GameObject ffSad;
    public GameObject ffWorried;

    private int completedTasks = 0;

    public GameObject smoke;

    public string mood = "";

    public AudioSource hammer;

    public GameObject gobag;

    public GameObject houseVFX;

    public GameObject dialogPanel;
    public hh_dialogManager dialog;

    public TextMeshProUGUI areYouSure;

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

    private void HouseMoodChange()
    {
        if (currentPhase == hh_task.phase.early)
        {
            Debug.Log("completed: " + completedTasks);
            if (completedTasks == 1) //progress to worried
            {
                AllMoodOff();
                worried.SetActive(true);
            } else if(completedTasks == 2) //progress to neutral
            {
                AllMoodOff();
                neutral.SetActive(true);
            } else if(completedTasks >= 3) // progress to happy
            {
                AllMoodOff();
                happy.SetActive(true);
            }
        }
        else if (currentPhase == hh_task.phase.fireSeason)
        {
            Debug.Log("completed: " + completedTasks);
            if (completedTasks >= 3) // progress to happy
            {
                AllMoodOff();
                happy.SetActive(true);
            }
            else if (completedTasks == 2) // progress to neutral
            {
                AllMoodOff();
                neutral.SetActive(true);
            }
        }
        else if (currentPhase == hh_task.phase.redflag)
        {
            Debug.Log("completed: " + completedTasks);
            if (completedTasks >= 4) // progress to happy
            {
                AllMoodOff();
                happy.SetActive(true);
            }
            else if (completedTasks == 3) // progress to neutral
            {
                AllMoodOff();
                neutral.SetActive(true);
            }
            else
            {
                AllMoodOff();
                worried.SetActive(true);
            }
        }
        houseVFX.SetActive(true);
    }

    public void HeaderAnim(string trig)
    {
        header.gameObject.transform.parent.gameObject.SetActive(true);
        header.GetComponent<Animator>().SetTrigger(trig);
    }

    public void ChangePhase()
    {
        //update skybox and lighting
        GetComponent<hh_sky>().ChangeSky();

        //progress from Early to Fire Season
        if (currentPhase == hh_task.phase.early)
        {
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].GetComponent<hh_task>().FailTask();
            }
            currentPhase = hh_task.phase.fireSeason;
            header.text = "Phase: Fire Season";
            AllMoodOff();
            worried.SetActive(true);
            ffSad.SetActive(false);
            ffWorried.SetActive(true);
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].gameObject.SetActive(true);
            }
            ResetDebris();
            GameObject[] roofs = GameObject.FindGameObjectsWithTag("Roof");
            foreach(GameObject r in roofs)
            {
                r.GetComponent<Outline>().enabled = false;
            }
        }

        //progress from Fire Season to Red Flag Day
        else if (currentPhase == hh_task.phase.fireSeason)
        {
            AllMoodOff();
            worried.SetActive(true);
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].GetComponent<hh_task>().FailTask();
            }
            currentPhase = hh_task.phase.redflag;
            gobag.SetActive(true);
            header.text = "Phase: Red Flag Day";
            for (int i = 0; i < taskItems.Length; i++)
            {
                if (tasks[i].GetComponent<hh_task>().taskPhase == currentPhase)
                    tasks[i].gameObject.SetActive(true);
            }
        }
        
        //progress from Red Flag Day to Evacuation Order
        else if (currentPhase == hh_task.phase.redflag)
        {
            AllMoodOff();
            worried.SetActive(true);
            currentPhase = hh_task.phase.evacuation;
            signs[0].SetActive(true);
            phaseButton.SetActive(false);
            evacButton.SetActive(true);
            smoke.SetActive(true);
            header.text = "Phase: EVACUATION ORDER";
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
        evacButton.SetActive(false);
        dialogPanel.SetActive(true);
        smoke.GetComponent<ParticleSystem>().Stop();
        completedTasks = 0;
        replayButton.SetActive(true);
        tasks[5].GetComponent<hh_task>().DoTask(); //do Evacuate task
        //count completed tasks, fail non-completed tasks
        for (int i = 0; i < taskItems.Length; i++)
        {
            if (tasks[i].GetComponent<hh_task>().complete)
                completedTasks++;
            else
                tasks[i].GetComponent<hh_task>().FailTask();
        }
        currentPhase = hh_task.phase.done;
        Debug.Log(hh_task.phase.done);
        AllMoodOff();
        //if all tasks done, get happy result
        if (completedTasks == taskItems.Length)
        {
            header.text = "House: Happy";
            HeaderAnim("done");
            header.color = Color.green;
            happy.SetActive(true);
            mood = "Happy";
            ffSad.SetActive(false);
            ffWorried.SetActive(false);
        }
        //if at least half tasks done, get worried result
        else if (completedTasks > taskItems.Length / 2)
        {
            header.text = "House: Worried";
            HeaderAnim("done");
            header.color = Color.yellow;
            worried.SetActive(true);
            mood = "Worried";
            ffSad.SetActive(false);
            ffWorried.SetActive(true);
        }
        //if less than half tasks done, get sad result
        else
        {
            header.text = "House: Sad";
            HeaderAnim("done");
            header.color = Color.red;
            sad.SetActive(true);
            mood = "Sad";
            ffSad.SetActive(true);
            ffWorried.SetActive(false);
        }

        dialog.StepTextForward();
    }

    private void AllMoodOff()
    {
        sad.SetActive(false);
        worried.SetActive(false);
        neutral.SetActive(false);
        happy.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }


    public void Clear(int debris)
    {
        //if the bush clearing task is not failed, clear the bush
        if (!tasks[0].GetComponent<hh_task>().failed)
        {
            tasks[0].GetComponent<hh_task>().DoTask();
            bushes[debris].SetActive(false);
            Instantiate(Resources.Load("sticks"), bushes[debris].transform.position, bushes[debris].transform.rotation);
            if (tasks[0].GetComponent<hh_task>().complete)
            {
                completedTasks++;
                HouseMoodChange();
            }
        } else if (bushes[debris].GetComponent<hh_collectable>().blocker)
        {
            bushes[debris].SetActive(false);
        }
    }

    public void ResetDebris()
    {
        if(tasks[0].GetComponent<hh_task>().complete)
            completedTasks--;
        foreach(GameObject b in bushes)
        {
            if (!b.activeInHierarchy)
            {
                b.SetActive(true);
                tasks[0].GetComponent<hh_task>().UndoTask();
            }
        }
    }

    public void ReplaceRoof(int roof)
    {
        //if the roof task is not failed, replace the roof
        if (!tasks[1].GetComponent<hh_task>().failed)
        {
            tasks[1].GetComponent<hh_task>().DoTask();
            roofs[roof].GetComponent<Animator>().SetTrigger("swap");
            if (!hammer.isPlaying)
            {
                hammer.pitch = Random.Range(1, 1.4f);
                hammer.Play();
            }
            if (tasks[1].GetComponent<hh_task>().complete)
            {
                completedTasks++;
                HouseMoodChange();
            }
        }
    }

    public void CarPacked()
    {
        if (!tasks[3].GetComponent<hh_task>().complete)
        {
            tasks[3].GetComponent<hh_task>().DoTask();
            completedTasks++;
            HouseMoodChange();
        }
        else
        {
            tasks[3].GetComponent<hh_task>().UndoTask();
            completedTasks--;
            HouseMoodChange();
        }
    }

    public void Chop(int log)
    {
        //if the log task is not failed, chop the log
        if (!tasks[2].GetComponent<hh_task>().failed)
        {
            tasks[2].GetComponent<hh_task>().DoTask();
            logs[log].SetActive(false);
            if (log + 1 < logs.Length)
                logs[log + 1].SetActive(true);
            else
                Instantiate(Resources.Load("chop"), logs[log].transform.position, transform.rotation);

            if (tasks[2].GetComponent<hh_task>().complete)
            {
                completedTasks++;
                HouseMoodChange();
            }
        }
    }

    public void PlaceSign()
    {
        //place the evacuated sign
        tasks[4].GetComponent<hh_task>().DoTask();
        signs[0].SetActive(false);
        signs[1].SetActive(true);
        completedTasks++;
        HouseMoodChange();
    }
}
