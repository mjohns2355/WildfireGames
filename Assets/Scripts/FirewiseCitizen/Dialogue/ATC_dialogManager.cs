using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.IO;
using System.Linq;


[Serializable]
public class QuoteEntry
{
    public string houseType;
    public string choice;   
    public string response;
    public string quote;  
    public string quoteES;  
}

[Serializable]
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
    [SerializeField] TextMeshProUGUI endQuote, firstHalf;
    [SerializeField] GameObject localNewsPanel1, localNewsPanel2;
    [SerializeField] Button dialogButton;
    [SerializeField] Button nextButton;
    [SerializeField] GameObject localNews;
    [SerializeField] Button dialogBoxButton;
    [SerializeField] GameObject dialogBox;
    [SerializeField] Image newsImage;
    [SerializeField] private QuoteData endQuoteData;
    private Dictionary<LevelStage, Dialog> dialogData;
    private int dialogIndex = 0;
    //private bool isLocalNewsShown = false;
    private Dialog currentDialog;
    public bool isInstructionShown = false;
    public int endQuotesNum;

    private void Awake()
    {
    }
    private void Start()
    {
        //localNewsCloseButton.onClick.AddListener(HideLocalNews);
        LoadQuotes("Assets/Resources/FirewiseCitizen/EndQuotes.json");
    }
    private void LoadQuotes(string filePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("FirewiseCitizen/EndQuotes");
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
        bool isSpanish = LocalizationManager.CurrentLanguage == "es";
        //Debug.Log($"Get end qupte for house: {houseType}, Selected Choice: {choice}, Response: {response}");
        foreach (var entry in endQuoteData.quotes)
        {
            //if (entry.response == "Followed" && entry.choice != "Wait for Notice") continue;
            if (string.Equals(entry.houseType,houseType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(entry.choice, choice, StringComparison.OrdinalIgnoreCase)
                && string.Equals(entry.response, response, StringComparison.OrdinalIgnoreCase))
            {
                if (isSpanish && !string.IsNullOrEmpty(entry.quoteES))
                {
                    return entry.quoteES;
                }
                return entry.quote;
            }
        }

        return isSpanish ? "No hay comentario disponible." : "No quote available for this scenario.";

        
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
        //this.result.text = result;
    }

    public void DisplayNextMessage()
    {
        var stage = GameManager.Instance.currentStage;

        if (stage == LevelStage.HouseDialog || stage == LevelStage.Tutorial) return;
        dialogBoxButton.onClick.AddListener(DisplayNextMessage);
        currentDialog = dialogData[stage];
        if (dialogIndex < currentDialog.messages.Length)
        {


            dialogText.text = currentDialog.messages[dialogIndex];

            dialogIndex++;


        }
        else
        {
            OnDialogComplete();
        }
    }

    private void GenerateLocalNews()
    {
        List<string> allQuotes = new List<string>();
        //var currentLevel = GameManager.Instance.CurrentLevel;
        var availableHouseTypes = GameManager.Instance.availableHouseTypes;
        var validCount = 0; 
        var totalValidCount = 0;
        var res = "";
        var dict = GameManager.Instance.structureManager.GetPlayerChoicesDict();
        bool followedOrders = GameManager.Instance.CountFollowedInstructions() >= 2;
        foreach (var type in availableHouseTypes)
        {
            foreach(var c in dict[type])
            {
                if(!c.isNormal) totalValidCount++;
            }
        }

        if (followedOrders && validCount != 0)
        {
            res = StringManager.Instance.GetText("newsQuote1Text");
        }
        else
        {
            res = StringManager.Instance.GetText("newsQuote2Text");
        }

        var rng = UnityEngine.Random.Range(0, availableHouseTypes.Count);
        var houseType = availableHouseTypes[rng];

        foreach (var c in dict[houseType])
        {
            var choice = c.choiceName;
            var response = GameManager.Instance.houseResponses[houseType];
            allQuotes.Add(GetEndQuote(houseType.ToString(), choice, response));
        }

        if (followedOrders)
        {
            string survivalIntro = StringManager.Instance.GetText("newsQuote3Text");
            string formattedIntro = string.Format(survivalIntro, GameManager.Instance.housesDestroyed);
            
            firstHalf.text = formattedIntro + " " + res;
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
        localNewsPanel1.SetActive(true);
        ATC_UIController.Instance.statsPanel.gameObject.SetActive(false);
        //isLocalNewsShown = true;
    }

    private void HideLocalNews()
    {
        ATC_UIController.Instance.PopPanel();
        localNewsPanel1.SetActive(true);
        localNewsPanel2.SetActive(false) ;

    }


}
