using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FF_BudgetManager
{
    public float initBudget;
    public float currentBudget;
    public float spentBudget;
    private float[] budgetThresholds;
    private bool[] triggeredThresholds;
    // Start is called before the first frame update

    public FF_BudgetManager(float initBudget = 2000f)
    {
        this.initBudget = initBudget;
        currentBudget = initBudget;
        spentBudget = 0f;
        budgetThresholds = new float[] { 30f, 60f, 90f };
        triggeredThresholds = new bool[budgetThresholds.Length];
    }

    public float CalculateRewardBudget()
    {
        var rng = Random.Range(0.1f, 0.2f);
        return initBudget * rng;
    }
    public bool SpendBudget(float amount)
    {
        if(amount > currentBudget)
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
        ResetBudgetThresholds();
    }
    void CheckBudgetThresholds()
    {
        float spendingPrecentage = (spentBudget / initBudget) * 100f;

        for (int i = 0; i< budgetThresholds.Length; i++)
        {
            if(spendingPrecentage >= budgetThresholds[i] && !triggeredThresholds[i])
            {
                triggeredThresholds[i] = true;
                //HH_GameManager.Instance.uiManager.ShowQuizPopup();
                HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(true);
                Debug.Log($"You’ve spent {budgetThresholds[i]}% of your money!");
            }
        }
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
