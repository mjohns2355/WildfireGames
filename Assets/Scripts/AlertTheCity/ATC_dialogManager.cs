using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

[System.Serializable]
public struct Dialog
{
    public string[] messages;
    public GameObject[] images;
}
public class ATC_dialogManager : MonoBehaviour
{
    //public string[] phaseOneDialog;
    //public string[] endDialog;

    public TextMeshProUGUI dialogText;

    public Dialog beforefirstSimDialog;
    public Dialog afterfirstSimDialog;
    public Dialog phaseOneDialog;
    public Dialog winDialog;
    public Dialog loseDialog;
    [SerializeField] Button dialogButton;
    [SerializeField] Button nextButton;
    //private int counter = 0;
    private Dictionary<LevelStage, Dialog> dialogData;
    private int dialogIndex;
    //public int houseDestroyed;
    //public int acresDestroyed;
    private bool isLocalNewsShown = false;
    private LevelStage currentStage;

    public bool done;

    public GameObject[] images;

    public GameObject localNews;
    public GameObject timer;


    private void Awake()
    {
        //StepTextForward();
        dialogData = new Dictionary<LevelStage, Dialog>
        {
            { LevelStage.BeforeFirstSim, beforefirstSimDialog },
            { LevelStage.AfterFirstSim, afterfirstSimDialog },
            { LevelStage.PhaseOne, phaseOneDialog },
            { LevelStage.Win, winDialog },
            { LevelStage.Lose, loseDialog }
        };
    }
    private void OnEnable()
    {
        currentStage = GameManager.Instance.currentStage;
        Debug.Log(currentStage);
        dialogIndex = 0;
        DisplayNextMessage();
    }

    private void OnDisable()
    {
        
    }
    public void SetStage(LevelStage stage)
    {
        GameManager.Instance.currentStage = stage;
        currentStage = stage;
        dialogIndex = 0;
        DisplayNextMessage();
    }

    public void DisplayNextMessage()
    {
        Dialog currentDialog = dialogData[currentStage];
        if (dialogIndex < currentDialog.messages.Length)
        {
            dialogText.text = currentDialog.messages[dialogIndex];
            Debug.Log(dialogText.text);
            if (currentDialog.images != null && dialogIndex < currentDialog.images.Length)
            {
                foreach (var img in currentDialog.images)
                    img.SetActive(false); 

                currentDialog.images[dialogIndex].SetActive(true);
            }

            dialogIndex++;
        }
        else
        {
            OnDialogComplete();
        }
    }

    private void OnDialogComplete()
    {
        Debug.Log("Dialog Complete");
        switch (currentStage)
        {
            case LevelStage.BeforeFirstSim:
                StartWorstSim();
                break;
            case LevelStage.AfterFirstSim:
                GameManager.Instance.ResetGame();
                break;
            case LevelStage.PhaseOne:
                ATC_UIController.Instance.PopPanel();
                break;
            case LevelStage.Win:
                ShowEndScene();  
                break;

            case LevelStage.Lose:
                ShowEndScene();  
                break;
            case LevelStage.End:
                break;
        }
    }

    private void StartWorstSim()
    {
        GameManager.Instance.StartSimulation();

    }
 
    private void ShowEndScene()
    {
        SetStage(LevelStage.End);
    }

    private void ShowLocalNews()
    {
        ATC_UIController.Instance.PopPanel();
        ATC_UIController.Instance.PushPanel(localNews);
    }
    public void EndDialog()
    {
        done = true;
        //counter = 0;
        //acresDestroyed = houseDestroyed / 5 + 12;
        //endDialog[0] = "The fire tore through our community. Thankfuly everyone survived, but " + houseDestroyed + " houses were destroyed and " + acresDestroyed + " acres were burned. ";
        //dialogText.text = endDialog[0];

    }


    public void StepTextForward()
    {
        //if (done)
        //{
        //    counter++;
        //    if(counter < endDialog.Length)
        //    {
        //        dialogText.text = endDialog[counter];
        //    } else
        //    {
        //        //localNews.SetActive(true);
        //        ATC_UIController.Instance.PopPanel();
        //        //gameObject.SetActive(false);
        //        ATC_UIController.Instance.PushPanel(localNews);
        //    }
        //}
        //else
        //{
        //    if (counter == -1)
        //    {
        //        if(timer != null)
        //        {
        //            timer.SetActive(true);
        //        }
        //        //gameObject.SetActive(false);
        //        ATC_UIController.Instance.PopPanel();
        //        // start auto simulation
        //        if (GameManager.Instance.FirstTimeLoading)
        //        {
        //            GameManager.Instance.StartSimulation();
        //        }
        //    }
        //    else
        //    {
        //        if (images.Length >= counter + 1)
        //        {
        //            if (counter >= 1)
        //            {

        //                images[counter - 1].SetActive(false);
        //            }
        //            if (counter >= 0)
        //            {

        //                images[counter].SetActive(true);
        //            }

        //        }
        //        dialogText.text = phaseOneDialog[counter];
        //        counter++;
        //        if (counter >= phaseOneDialog.Length)
        //        {

        //            counter = -1;
        //        }

        //    }
        //}
        
       
    }

}
