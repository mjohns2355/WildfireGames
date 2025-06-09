using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class FF_EndScreensManager : MonoBehaviour
{
    public GameObject fireResultScreen, competitionResultScreen;
    public TextMeshProUGUI p1BurnPercentText, p2BurnPercentText, p1CompetitionScore, p2CompetitionScore, winnerText,p1Reward,p2Reward;

    public void ShowFireResultScreen(float p1Score, float p2Score)
    {
        Debug.Log($"P1: {p1Score}, P2: {p2Score}");
        var p1 = p1Score < 1 ? p1Score : Mathf.Round(p1Score);
        var p2 = p2Score < 1 ? p2Score : Mathf.Round(p2Score); ;
        p1BurnPercentText.text = $"{p1}%";
        p2BurnPercentText.text = $"{p2}%";
        fireResultScreen.SetActive(true);
    }

    public void ShowCompetitionResult(int p1, int p2) { 
        p1CompetitionScore.text = $"{p1} pts";
        p2CompetitionScore.text = $"{p2} pts";
        winnerText.text = p1 > p2 ? "Player 1 Wins!" : p1 < p2 ? "Player 2 Wins!" : "It's a Tie!";
        UpdateRewards(p1, p2);
        competitionResultScreen.SetActive(true);
    }

    private void UpdateRewards(int p1Score, int p2Score)
    {
        
        int rewardP1 = p1Score > p2Score ? HH_GameManager.Instance.WinReward : (p1Score == p2Score ? HH_GameManager.Instance.WinReward/2 : 0);
        int rewardP2 = p2Score > p1Score ? HH_GameManager.Instance.WinReward : (p1Score == p2Score ? HH_GameManager.Instance.WinReward/2 : 0);

        
        ApplyRewardUI(p1Reward, rewardP1);
        ApplyRewardUI(p2Reward, rewardP2);
    }

    private void ApplyRewardUI(TextMeshProUGUI rewardText, int amount)
    {
        var container = rewardText.transform.parent.gameObject;

        if (amount > 0)
        {
            rewardText.text = $"+ ${amount}";
            container.SetActive(true);
        }
        else
        {
            container.SetActive(false);
        }
    }

    public void HideEndScreens()
    {
        fireResultScreen.SetActive(false);
        competitionResultScreen.SetActive(false);
        gameObject.SetActive(false);
    }
}
