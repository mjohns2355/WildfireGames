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
        if (currentPhase == hh_task.phase.early && completedTasks == 1)
        {
            worried.SetActive(true);
            sad.SetActive(false);
        }
        else if (currentPhase == hh_task.phase.early && completedTasks == 2)
        {

            worried.SetActive(false);
            neutral.SetActive(true);
        }
        else if (currentPhase == hh_task.phase.early && completedTasks == 3)
        {

            happy.SetActive(true);
            neutral.SetActive(false);
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

        GetComponent<hh_sky>().ChangeSky();
        if (currentPhase == hh_task.phase.early)
        {
            currentPhase = hh_task.phase.fireSeason;
            header.text = "Phase: Fire Season";
            worried.SetActive(true);
            sad.SetActive(false);
            happy.SetActive(false);
            neutral.SetActive(false);
            ResetDebris();
        }
        else if (currentPhase == hh_task.phase.fireSeason)
        {
            
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
        } else if (currentPhase == hh_task.phase.redflag)
        {
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
        completedTasks = 0;
        replayButton.SetActive(true);
        tasks[5].GetComponent<hh_task>().DoTask();
        for (int i = 0; i < taskItems.Length; i++)
        {
            if (tasks[i].GetComponent<hh_task>().complete)
                completedTasks++;
            else
                tasks[i].GetComponent<hh_task>().FailTask();
        }
        currentPhase = hh_task.phase.done;
        if (completedTasks == taskItems.Length)
        {
            header.text = "House: Happy";
            HeaderAnim("done");
            header.color = Color.green;
            happy.SetActive(true);
            mood = "Happy";
        }
        else if (completedTasks > taskItems.Length / 2)
        {

            header.text = "House: Worried";
            HeaderAnim("done");
            header.color = Color.yellow;
            worried.SetActive(true);
            mood = "Worried";
        }
        else
        {

            header.text = "House: Sad";
            HeaderAnim("done");
            header.color = Color.red;
            sad.SetActive(true);
            mood = "Sad";
        }
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
        foreach(GameObject b in bushes)
        {
            b.SetActive(true);
            tasks[0].GetComponent<hh_task>().UndoTask();
        }
    }

    public void ReplaceRoof(int roof)
    {
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

    public void Chop(int log)
    {
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
        tasks[4].GetComponent<hh_task>().DoTask();
        signs[0].SetActive(false);
        signs[1].SetActive(true);
        completedTasks++;
        HouseMoodChange();
    }
}
