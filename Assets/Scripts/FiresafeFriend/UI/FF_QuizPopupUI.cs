using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FF_QuizPopupUI : MonoBehaviour
{
    public Dictionary<Button,int> optionDict = new Dictionary<Button,int>();
    public TextMeshProUGUI questionText;
    public Transform buttonsParent;
    public GameObject quizOptionButtonPrefab;
    public Color normalColor, correctColor, wrongColor;
    public Action OnCorrectAnswer;
    private int correctAnswerIndex;
    private Question question;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitQuizPopup()
    {
        question = HH_GameManager.Instance.quizManager.ReturnRandomQuestion();
        questionText.text = question.questionText;
        // make index starts from 0
        correctAnswerIndex = question.correctAnswerIndex - 1;
        for(int i = 0; i < question.options.Length; i++)
        {
            var obj = Instantiate(quizOptionButtonPrefab, buttonsParent);
            var button = obj.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];
            //button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                StartCoroutine(OnOptionButtonClickedRoutine(button));
            });
            optionDict.Add(button,i);
            
        }
    }

    void OnOptionButtonClicked(Button button)
    {

        int index = -1;
        optionDict.TryGetValue(button, out index);

        if (index == correctAnswerIndex)
        {
            //Debug.Log("Correct Answer");
            button.GetComponent<Image>().color = Color.green;
            //ChangeButtonColor(button, correctColor);
            //var budgetManager = HH_GameManager.Instance.currentPlayer.budgetManager;
            //budgetManager.IncreaseBudget(budgetManager.CalculateRewardBudget());
            OnCorrectAnswer.Invoke();
        }
        else
        {
            button.GetComponent<Image>().color = Color.red;

        }
    }


    IEnumerator OnOptionButtonClickedRoutine(Button button)
    {
        OnOptionButtonClicked(button);
        foreach (var btn in optionDict.Keys)
        {
            btn.interactable = false;
        }
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        //hide earn more button
        HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(false);
    }

    private void OnDisable()
    {

        for(int i = 0; i < buttonsParent.childCount; i++)
        {
            Destroy(buttonsParent.GetChild(i).gameObject);
        }
        optionDict.Clear();
        StopAllCoroutines();
    }
}
