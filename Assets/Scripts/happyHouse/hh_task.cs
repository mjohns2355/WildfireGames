using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hh_task : MonoBehaviour
{
    public enum phase
    {
        early,
        fireSeason,
        redflag,
        evacuation,
        done
    }

    public phase taskPhase;

    public GameObject checkmark;
    public TMPro.TextMeshProUGUI taskCount;
    public int totalCount;
    private int currentCount = 0;
    public bool failed = false;
    public bool canFail;
    public bool complete = false;

    // Start is called before the first frame update
    void Start()
    {
        taskCount.text = currentCount.ToString() + "/" + totalCount.ToString();
    }

    public void DoTask()
    {
        if (!failed)
        {

            currentCount++;
            if (currentCount == totalCount)
            {
                complete = true;
                checkmark.SetActive(true);
                taskCount.gameObject.SetActive(false);
            }
            else
            {
                taskCount.text = currentCount.ToString() + "/" + totalCount.ToString();
            }
        }
        
    }

    public void UndoTask()
    {
        if (!failed)
        {

            currentCount--;
                complete = false;
                checkmark.SetActive(false);
                taskCount.gameObject.SetActive(true);
                taskCount.text = currentCount.ToString() + "/" + totalCount.ToString();
        }
    }

    public void FailTask()
    {
        if (canFail && !complete)
        {
            taskCount.text = "X";
            taskCount.color = Color.red;
            failed = true;
        } else
        {
            canFail = true;
        }
    }
}
