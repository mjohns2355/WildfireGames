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
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI debugResultText;
    [SerializeField] TextMeshProUGUI debugResultText2;
    [SerializeField] GameObject localNewsPanel1, localNewsPanel2;
    [SerializeField] Dialog beforefirstSimDialog;
    [SerializeField] Dialog afterfirstSimDialog;
    [SerializeField] Dialog phaseOneDialog;
    [SerializeField] Dialog winDialog;
    [SerializeField] Dialog loseDialog;
    [SerializeField] Button dialogButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button localNewsCloseButton;
    [SerializeField] GameObject localNews;

    private Dictionary<LevelStage, Dialog> dialogData;
    private int dialogIndex = 0;
    private bool isLocalNewsShown = false;
    private Dialog currentDialog;
    private bool isToolBarBroughtToFront = false;
    public bool isInstructionShown = false;
    public GameObject arrow;
    public GameObject arrow2;

    private void Awake()
    {
        dialogData = new Dictionary<LevelStage, Dialog>
        {
            { LevelStage.BeforeFirstSim, beforefirstSimDialog },
            { LevelStage.AfterFirstSim, afterfirstSimDialog },
            { LevelStage.PhaseOne, phaseOneDialog },
            { LevelStage.Win, winDialog },
            { LevelStage.Lose, loseDialog }
        };
    }
    private void Start()
    {
        localNewsCloseButton.onClick.AddListener(HideLocalNews);
    }

    private void OnEnable()
    {
        dialogIndex = isLocalNewsShown ? 2 : 0;
        DisplayNextMessage();
        arrow.SetActive(true);
        if (isToolBarBroughtToFront)
        {
            ATC_UIController.Instance.toolsBar.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            isToolBarBroughtToFront=false;
        }
        


    }

    public void GenerateResult()
    {

        var first = Mathf.RoundToInt(GameManager.Instance.firstEvacCarTimeStamp);
        var last = Mathf.RoundToInt(GameManager.Instance.lastEvacCarTimeStamp);
        var result = $"The first car reach the shelter after {first} seconds and the final car reached the shelter after {last} seconds.";
        var stage = GameManager.Instance.currentStage;
        switch (stage)
        {
            case LevelStage.AfterFirstSim:
                dialogData[stage].messages[0] = result + "Can you do better?"; 
                break;
            case LevelStage.Win:
                dialogData[stage].messages[0] = result;
                break;
            case LevelStage.Lose:
                dialogData[stage].messages[0] = result;
                break;
        }
    }


    public void DisplayNextMessage()
    {
        var stage = GameManager.Instance.currentStage;
        currentDialog = dialogData[stage];
        if (dialogIndex < currentDialog.messages.Length)
        {

            if (stage == LevelStage.Win || stage == LevelStage.Lose)
            {
                if (dialogIndex == 2 && !isLocalNewsShown)
                {
                    ShowLocalNews();
                }
            }
            if(stage == LevelStage.BeforeFirstSim)
            {
                if (dialogIndex == 1)
                {
                    ATC_UIController.Instance.toolsBar.transform.SetAsLastSibling();
                    isToolBarBroughtToFront = true;
                    arrow.SetActive(false);
                    arrow2.SetActive(true);
                }
            }

            dialogText.text = currentDialog.messages[dialogIndex];
            //Debug.Log(dialogText.text);
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

    private void GenerateLocalNews()
    {
        //Debug.Log("Generate Local News");
        var currentLevel = GameManager.Instance.CurrentLevel;
        var dict = GameManager.Instance.structureManager.GetPlayerChoicesDict();

        string twoCarRes = dict[HouseType.twoCar].endGameFeedback;
        string wuiRes = dict[HouseType.wui].endGameFeedback;
        debugResultText.text = "The fire's cause is not certain but likely from a downed powerline at the west edge of the town where our community meets the forest.\n\n";

        debugResultText.text += twoCarRes + "\n\n";

        if (currentLevel != 0)
        {
            debugResultText.text += "Wildfire is always dangerous, but there are things we can all do to have a safer evacuation.\n\n";

            string petRes = dict[HouseType.pet].endGameFeedback;
            string horseRes = dict[HouseType.horse].endGameFeedback;
            debugResultText.text += petRes + "\n\n";
            debugResultText.text += horseRes + "\n\n";
        }


        if (currentLevel != 0)
        {
            debugResultText2.text = "We know some residents need more time and help getting out during an evacuation.\n\n";

            string kidsRes = dict[HouseType.kids].endGameFeedback;
            string elderRes = dict[HouseType.elderly].endGameFeedback;
            debugResultText2.text += elderRes + "\n\n";
            debugResultText2.text += kidsRes + "\n\n";
        }



        debugResultText2.text += "Houses most at risk are the ones closest to the Wildland Urban Interface – the area where human development meets wild land and forest. \n\n";


        debugResultText2.text += wuiRes + "\n\n";


        debugResultText2.text += "Our community is grateful to the firefighters and emergency responders who made sure everyone got out alive. There is much to rebuild, and we will do it together. ";



    }
    public void SetStage(LevelStage stage)
    {
        GameManager.Instance.currentStage = stage;

        dialogIndex = 0;
        DisplayNextMessage();
    }

    private void OnDialogComplete()
    {
        //Debug.Log("Dialog Complete");
        switch (GameManager.Instance.currentStage)
        {
            case LevelStage.BeforeFirstSim:

                break;
            case LevelStage.AfterFirstSim:
                ResetLevel();
                isInstructionShown = true;
                break;
            case LevelStage.PhaseOne:
                ATC_UIController.Instance.PopPanel();
                break;
            case LevelStage.Win:
                LoadNewLevel();
                break;

            case LevelStage.Lose:
                ResetLevel();
                break;
            case LevelStage.End:
                break;
        }
    }

 
 
    private void LoadNewLevel()
    {
        isInstructionShown = false;
        GameManager.Instance.NextLevel();

    }
    private void ResetLevel()
    {
        GameManager.Instance.ResetGame();

        
    }

    private void ShowLocalNews()
    {
        GenerateLocalNews();
        ATC_UIController.Instance.PushPanel(localNews);
        ATC_UIController.Instance.statsPanel.gameObject.SetActive(false);
        isLocalNewsShown = true;
    }

    private void HideLocalNews()
    {
        ATC_UIController.Instance.PopPanel();
        debugResultText.text = string.Empty;
        debugResultText2.text  = string.Empty;
        localNewsPanel1.SetActive(true);
        localNewsPanel2.SetActive(false) ;
        isLocalNewsShown = false;
        ATC_UIController.Instance.statsPanel.gameObject.SetActive(true);
        if (GameManager.Instance.IsLastLevel && GameManager.Instance.currentStage == LevelStage.Win)
        {
            ATC_UIController.Instance.ShowEndScreen();
        }
    }


}
