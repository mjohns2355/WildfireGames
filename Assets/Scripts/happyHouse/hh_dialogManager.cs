using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hh_dialogManager : MonoBehaviour
{
    public string[] phaseOneDialog;
    public string[] phaseTwoDialog;
    public string[] phaseThreeDialog;
    public string[] phaseFourDialog;
    public string[] finalHappyDialog;
    public string[] finalWorriedDialog;
    public string[] finalSadDialog;

    public TextMeshProUGUI dialog;

    private int counter = 0;
    public hh_level_manager house_level;

    public GameObject taskListBtn;
    public GameObject doneBtn;

    public GameObject sadFireFighter;
    public GameObject worriedFireFighter;

    private string phaseTrigger;

    public AudioClip wind2;

    private void Start()
    {
        StepTextForward();
    }

    public void DoneBtnOn()
    {
        if (house_level.currentPhase != hh_task.phase.evacuation && house_level.currentPhase != hh_task.phase.done)
            doneBtn.SetActive(true);
    }

    public void StepTextForward()
    {
        if(counter == -1)
        {
            doneBtn.SetActive(false);
            counter = 0;
            if (house_level.currentPhase != hh_task.phase.done)
            {

                taskListBtn.SetActive(true);
                if (house_level.currentPhase != hh_task.phase.evacuation)
                    doneBtn.SetActive(true);
                house_level.HeaderAnim(phaseTrigger);
            }
            else
            {
                if (house_level.mood == "Happy")
                {
                    dialog.text = finalHappyDialog[0];
                }
                if (house_level.mood == "Worried")
                {
                    dialog.text = finalWorriedDialog[0];
                }
                if (house_level.mood == "Sad")
                {
                    dialog.text = finalSadDialog[0];
                }
            }
            gameObject.SetActive(false);
        } else
        {
            if (house_level.currentPhase == hh_task.phase.early)
            {
                dialog.text = phaseOneDialog[counter];
                counter++;
                if (counter >= phaseOneDialog.Length)
                {
                    GetComponent<AudioSource>().clip = wind2;
                    phaseTrigger = "phase1";
                    counter = -1;
                }
            }
            else if (house_level.currentPhase == hh_task.phase.fireSeason)
            {
                dialog.text = phaseTwoDialog[counter];
                counter++;
                if (counter >= phaseTwoDialog.Length)
                {

                    
                    phaseTrigger = "phase2";
                    counter = -1;
                }
            }
            else if (house_level.currentPhase == hh_task.phase.redflag)
            {
                dialog.text = phaseThreeDialog[counter];
                counter++;
                if (counter >= phaseThreeDialog.Length)
                {
                    GetComponent<AudioSource>().playOnAwake = false;
                    phaseTrigger = "phase3";
                    counter = -1;
                }
            }
            else if (house_level.currentPhase == hh_task.phase.evacuation)
            {
                dialog.text = phaseFourDialog[counter];
                counter++;
                if (counter >= phaseFourDialog.Length)
                {

                    phaseTrigger = "phase3";
                    counter = -1;
                }
            }
            else if (house_level.mood == "Happy")
            {
                dialog.text = finalHappyDialog[counter];
                counter++;
                if (counter >= finalHappyDialog.Length)
                    counter = -1;
            }
            else if (house_level.mood == "Worried")
            {
                worriedFireFighter.SetActive(true);
                dialog.text = finalWorriedDialog[counter];
                counter++;
                if (counter >= finalWorriedDialog.Length)
                    counter = -1;
            }
            else if (house_level.mood == "Sad")
            {
                Debug.Log(house_level.currentPhase);
                sadFireFighter.SetActive(true);
                dialog.text = finalSadDialog[counter];
                counter++;
                if (counter >= finalSadDialog.Length)
                    counter = -1;
            }
        }
       
    }

}
