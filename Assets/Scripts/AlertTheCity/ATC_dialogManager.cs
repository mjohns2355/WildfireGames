using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.IO;
using System.Linq;


[System.Serializable]
public class QuoteEntry
{
    public string houseType;
    public string choice;   
    public string response;
    public string quote;    
}

[System.Serializable]
public class QuoteData
{
    public List<QuoteEntry> quotes;
}

[Serializable]
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
    [SerializeField] TextMeshProUGUI result, endQuote, firstHalf;
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
    [SerializeField] Button dialogBoxButton;
    [SerializeField] GameObject dialogBox;
    [SerializeField] Image newsImage;
    private QuoteData endQuoteData;
    private Dictionary<LevelStage, Dialog> dialogData;
    private int dialogIndex = 0;
    //private bool isLocalNewsShown = false;
    private Dialog currentDialog;
    //private bool isToolBarBroughtToFront = false;
    public bool isInstructionShown = false;
    public int endQuotesNum;
    //public Button proceedButton;
    //public GameObject arrow;
    //public GameObject arrow2;

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
        LoadQuotes("Assets/Resources/AlertTheCity/EndQuotes.json");
    }
    private void LoadQuotes(string filePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("AlertTheCity/EndQuotes");
        //string json = File.ReadAllText(filePath);
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found in Resources!");
        }
        else
        {
            string json = jsonFile.text;
            //Debug.Log($"Loaded JSON: {json}");
            if (json != null)
            {
                endQuoteData = JsonUtility.FromJson<QuoteData>(json);
                //PrintEndQuoteData();
                if (endQuoteData == null || endQuoteData.quotes.Count == 0)
                {
                    Debug.LogError("End quote data is empty or not loaded properly.");
                }
                else
                {
                    Debug.Log("End quotes loaded successfully.");
                }
            }
            else
            {
                Debug.LogError("EndQuotes.json file not found in Resources folder.");
            }
        }
        
    }
    
    public string GetEndQuote(string houseType, string choice, string response)
    {

        foreach (var entry in endQuoteData.quotes)
        {
            if (entry.response == "Followed" && entry.choice != "Wait for Notice") continue;
            if (entry.houseType == houseType && entry.choice == choice && entry.response == response)
            {
                return entry.quote;
            }
        }

        return "No quote available for this scenario.";
    }

    public void ShowDialogBox()
    {
        //dialogBox.SetActive(true);
        //dialogIndex = 0;
        //dialogIndex = isLocalNewsShown ? 2 : 0;
        //DisplayNextMessage();
        //arrow.SetActive(true);
        //if (isToolBarBroughtToFront)
        //{
        //    //ATC_UIController.Instance.toolsBar.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        //    isToolBarBroughtToFront = false;
        //}
    }

    public void HideDialogBox()
    {
        dialogBox.SetActive(false);
        dialogBoxButton.onClick.RemoveListener(DisplayNextMessage);
    }

    public void GenerateResult()
    {

        var first = Mathf.RoundToInt(GameManager.Instance.firstEvacCarTimeStamp);
        var last = Mathf.RoundToInt(GameManager.Instance.lastEvacCarTimeStamp);
        var result = $"The first car reached the shelter after {first} minutes and the final car reached the shelter after {last} minutes.";
        this.result.text = result;
        var stage = GameManager.Instance.currentStage;
        switch (stage)
        {
            case LevelStage.AfterFirstSim:
                dialogData[stage].messages[0] = result + " Can you do better?";
                break;
            //case LevelStage.Win:
            //    dialogData[stage].messages[0] = result;
            //    break;
            //case LevelStage.Lose:
            //    dialogData[stage].messages[0] = result;
            //    break;
        }
    }

    public void DisplayNextMessage()
    {
        var stage = GameManager.Instance.currentStage;

        if (stage == LevelStage.HouseDialog) return;
        dialogBoxButton.onClick.AddListener(DisplayNextMessage);
        currentDialog = dialogData[stage];
        if (dialogIndex < currentDialog.messages.Length)
        {

            //if (stage == LevelStage.Win || stage == LevelStage.Lose)
            //{
            //    if (dialogIndex == 2 && !isLocalNewsShown)
            //    {
            //        ShowLocalNews();
            //    }
            //}
            //if(stage == LevelStage.BeforeFirstSim)
            //{
            //    if (dialogIndex == 1)
            //    {
            //        ATC_UIController.Instance.toolsBar.transform.SetAsLastSibling();
            //        isToolBarBroughtToFront = true;
            //        arrow.SetActive(false);
            //        arrow2.SetActive(true);
            //    }
            //}

            dialogText.text = currentDialog.messages[dialogIndex];
            //Debug.Log($"Current Message: {currentDialog.messages[dialogIndex]}");
            //Debug.Log(dialogText.text);
            //if (currentDialog.images != null && dialogIndex < currentDialog.images.Length)
            //{
            //    foreach (var img in currentDialog.images)
            //        img.SetActive(false); 

            //    currentDialog.images[dialogIndex].SetActive(true);
            //}

            dialogIndex++;


        }
        else
        {
            OnDialogComplete();
        }
    }

    private void GenerateLocalNews()
    {
        Debug.Log("Generate Local News");
        List<string> allQuotes = new List<string>();
        //var currentLevel = GameManager.Instance.CurrentLevel;
        var availableHouseTypes = GameManager.Instance.availableHouseTypes;
        var validCount = 0; 
        var totalValidCount = 0;
        var res = "";
        var dict = GameManager.Instance.structureManager.GetPlayerChoicesDict();
        //var quote = "";
        bool followedOrders = GameManager.Instance.CountFollowedInstructions() >= 2;
        foreach (var type in availableHouseTypes)
        {
            foreach(var c in dict[type])
            {
                if(!c.isNormal) totalValidCount++;
            }
            //if (!dict[type].isNormal) totalValidCount++;
        }

        if (followedOrders && validCount != 0)
        {
            res = "We credit this to the effort citizens took to ";
           
        }
        else
        {
            res = "Despite warnings, many residents did not follow evacuation orders for a variety of personal circumstances. We need to come together as a community to prepare better for the next time.";
        }

        var rng = UnityEngine.Random.Range(0, availableHouseTypes.Count);
        var houseType = availableHouseTypes[rng];
        
        Debug.Log("Chose Quote: " + houseType.ToString());
        foreach (var c in dict[houseType])
        {
            //var choice = dict[type].choiceName;
            var choice = c.choiceName;
            var response = GameManager.Instance.houseResponses[houseType.ToString()];
            //quote += $"{GetEndQuote(type.ToString(), choice, response)}" + "\n";
            allQuotes.Add(GetEndQuote(houseType.ToString(), choice, response));
        }
        //for (int i = 0; i < availableHouseTypes.Count; i++)
        //{

        //    var type = availableHouseTypes[i];
        //    foreach (var c in dict[type])
        //    {
        //        //var choice = dict[type].choiceName;
        //        var choice = c.choiceName;
        //        var response = GameManager.Instance.houseResponses[type.ToString()];
        //        //quote += $"{GetEndQuote(type.ToString(), choice, response)}" + "\n";
        //        allQuotes.Add(GetEndQuote(type.ToString(), choice, response));
        //        //if (dict[type].isNormal) continue;
        //        if (c.isNormal) continue;
        //        validCount++;
        //        // Add and before the last choice
        //        if (validCount == totalValidCount)
        //        {
        //            res += $"and {choice}.";
        //        }
        //        else
        //        {
        //            res += $"{choice}, ";
        //        }
        //    }


        //}
        if (followedOrders)
        {
            firstHalf.text = $"Miraculously, only {GameManager.Instance.housesDestroyed} homes were damaged during the major fire that spread through the city. " + res;
        }
        else
        {
            firstHalf.text = res;
        }

        var i = new System.Random();
        List<string> randomQuotes = allQuotes.OrderBy(x => i.Next()).Take(1).ToList();
        newsImage.sprite = GameManager.Instance.structureManager.ReturnHouseInfoFor(houseType).newsUISprite;
        var quote = string.Join("\n\n", randomQuotes);
        endQuote.text = quote;

        //string twoCarRes = dict[HouseType.twoCar].endGameFeedback;
        //string wuiRes = dict[HouseType.wui].endGameFeedback;
        //debugResultText.text = "The fire's cause is not certain but likely from a downed powerline at the west edge of the town where our community meets the forest.\n\n";

        //debugResultText.text += twoCarRes + "\n\n";

        //if (currentLevel != 0)
        //{
        //    debugResultText.text += "Wildfire is always dangerous, but there are things we can all do to have a safer evacuation.\n\n";

        //    string petRes = dict[HouseType.pet].endGameFeedback;
        //    //string horseRes = dict[HouseType.horse].endGameFeedback;
        //    debugResultText.text += petRes + "\n\n";
        //    //debugResultText.text += horseRes + "\n\n";
        //}


        //if (currentLevel != 0)
        //{
        //    debugResultText2.text = "We know some residents need more time and help getting out during an evacuation.\n\n";

        //    string kidsRes = dict[HouseType.kids].endGameFeedback;
        //    string elderRes = dict[HouseType.elderly].endGameFeedback;
        //    debugResultText2.text += elderRes + "\n\n";
        //    debugResultText2.text += kidsRes + "\n\n";
        //}



        //debugResultText2.text += "Houses most at risk are the ones closest to the Wildland Urban Interface – the area where human development meets wild land and forest. \n\n";


        //debugResultText2.text += wuiRes + "\n\n";


        //debugResultText2.text += "Our community is grateful to the firefighters and emergency responders who made sure everyone got out alive. There is much to rebuild, and we will do it together. ";



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
                //isInstructionShown = true;
                break;
            case LevelStage.PhaseOne:
                //ATC_UIController.Instance.PopPanel();
                HideDialogBox();
                break;
            case LevelStage.Win:
                //LoadNewLevel();
                break;

            case LevelStage.Lose:
                //ResetLevel();
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

    public void ShowLocalNews()
    {
        GenerateLocalNews();
        ATC_UIController.Instance.PushPanel(localNews);
        ATC_UIController.Instance.statsPanel.gameObject.SetActive(false);
        //isLocalNewsShown = true;
    }

    private void HideLocalNews()
    {
        ATC_UIController.Instance.PopPanel();
        //debugResultText.text = string.Empty;
        //debugResultText2.text  = string.Empty;
        localNewsPanel1.SetActive(true);
        localNewsPanel2.SetActive(false) ;
        //isLocalNewsShown = false;
        //ATC_UIController.Instance.statsPanel.gameObject.SetActive(true);
        //if (GameManager.Instance.IsLastLevel && GameManager.Instance.currentStage == LevelStage.Win)
        //{
        //    ATC_UIController.Instance.ShowEndScreen();
        //}
    }


}
