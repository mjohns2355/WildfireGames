using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class FF_EndScreensManager : MonoBehaviour
{
    public GameObject fireResultScreen, competitionResultScreen;
    public TextMeshProUGUI p1BurnPercentText, p2BurnPercentText, p1CompetitionScore, p2CompetitionScore, winnerText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowFireResultScreen(float p1Score, float p2Score)
    {
        p1BurnPercentText.text = $"{p1Score}%";
        p2BurnPercentText.text = $"{p2Score}%";
        fireResultScreen.SetActive(true);
    }

    public void ShowCompetitionResult(float p1Score, float p2Score)
    {
        var p1 = (int)(p1Score * 10f);
        var p2 = (int)(p2Score * 10f);
        p1CompetitionScore.text = $"{p1} pts";
        p2CompetitionScore.text = $"{p2} pts";
        winnerText.text = p1 > p2 ? "Player 1 Wins!" : p1 < p2 ? "Player 2 Wins!" : "It's a Tie!";
        competitionResultScreen.SetActive(true);
    }

    public void HideEndScreens()
    {
        fireResultScreen.SetActive(false);
        competitionResultScreen.SetActive(false);
        gameObject.SetActive(false);
    }
}
