using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class FF_BudgetManager
{
    public float initBudget;
    public float currentBudget;
    public float spentBudget;
    public float rewardAmount;
    private float[] budgetThresholds;
    private bool[] triggeredThresholds;
    HouseManager owner;
    // Start is called before the first frame update

    public FF_BudgetManager(HouseManager owner,float initBudget = 2000f)
    {
        this.initBudget = initBudget;
        this.owner = owner;
        currentBudget = initBudget;
        spentBudget = 0f;
        budgetThresholds = new float[] { 20000, 10000 };
        triggeredThresholds = new bool[budgetThresholds.Length];
        HH_GameManager.Instance.uiManager.quizPopup.OnCorrectAnswer += () =>
        {
            if (HH_GameManager.Instance.currentPlayer != owner) return;
            IncreaseBudget(rewardAmount);
        };
    }

    public float CalculateRewardBudget()
    {
        var rng = Random.Range(0.05f, 0.1f);
        return initBudget * rng;
    }
    public bool SpendBudget(float amount)
    {
        rewardAmount = CalculateRewardBudget();
        if (amount > currentBudget)
        {
            Debug.Log("Not enough money!");
            return false;
        }
        currentBudget -= amount;
        spentBudget += amount;
        //invoke ui changes
        HH_GameManager.Instance.uiManager.storePanel.UpdateBudgetText(currentBudget);
        CheckBudgetThresholds();

        return true;
    }

    public void IncreaseBudget(float amount)
    {
        currentBudget += amount;
        initBudget += amount;
        //invoke ui changes
        HH_GameManager.Instance.uiManager.storePanel.UpdateBudgetText(currentBudget);
        Debug.Log($"Increased budget by {amount}!");
        //ResetBudgetThresholds();
    }
    void CheckBudgetThresholds()
    {
        //float spendingPrecentage = (spentBudget / initBudget) * 100f;

        //for (int i = 0; i < budgetThresholds.Length; i++)
        //{
        //    if (spendingPrecentage >= budgetThresholds[i] && !triggeredThresholds[i])
        //    {
        //        triggeredThresholds[i] = true;
        //        //HH_GameManager.Instance.uiManager.ShowQuizPopup();
        //        HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(true);
        //        Debug.Log($"You’ve spent {budgetThresholds[i]}% of your money!");
        //    }
        //}
       
        int money = 0;
        for (int i = 0; i < budgetThresholds.Length; i++)
        {
            if (currentBudget < budgetThresholds[i] && !triggeredThresholds[i])
            {
                triggeredThresholds[i] = true;
               
                HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(true);
                if (i == 0)
                {
                    money += 15000;
                    Debug.Log($"{owner.playerTag}'s Budget below $20,000! Quiz triggered.");
                }
                else if (i == 1)
                {
                    money += 5000;
                    Debug.Log($"{owner.playerTag}'s Budget below $10,000! Quiz triggered.");
                }
                
            }
        }
        rewardAmount = money;
    }

    void ResetBudgetThresholds()
    {
        float spendingPrecentage = (spentBudget / initBudget) * 100f;
        for (int i = 0; i < budgetThresholds.Length; i++)
        {
            if(spendingPrecentage <= budgetThresholds[i] && triggeredThresholds[i])
            {
                triggeredThresholds[i] = false;
                Debug.Log($"Threshold for {budgetThresholds[i]}% has been reset.");
            }
           
        }
    }
}
