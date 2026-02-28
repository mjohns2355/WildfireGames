using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FF_QuizPopupUI : MonoBehaviour
{
    public Dictionary<Button, int> optionDict = new Dictionary<Button, int>();
    public Button close;
    public TextMeshProUGUI questionText;
    public Transform buttonsParent;
    public GameObject quizOptionButtonPrefab,quizPanel;
    public Color normalColor, correctColor, wrongColor;
    public Action OnCorrectAnswer;
    private int correctAnswerIndex;
    private Question question;
    bool answeredCorrectly = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitQuizPopup()
    {
        answeredCorrectly = false;

        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }
        optionDict.Clear();

        question = HH_GameManager.Instance.quizManager.ReturnRandomQuestion();
        questionText.text = question.GetLocalizedQuestion(LocalizationManager.CurrentLanguage);

        string[] localizedOptions = question.GetLocalizedOptions(LocalizationManager.CurrentLanguage);

        correctAnswerIndex = question.correctAnswerIndex - 1;

        for (int i = 0; i < localizedOptions.Length; i++)
        {
            var obj = Instantiate(quizOptionButtonPrefab, buttonsParent);
            var button = obj.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = localizedOptions[i];

            int capturedIndex = i;
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                StartCoroutine(OnOptionButtonClickedRoutine(button));
            });

            optionDict.Add(button, capturedIndex);
        }

        quizPanel.SetActive(true);
        if (HH_GameManager.Instance.isTutorial)
        {
            ScriptedTutorialExample();
        }
        
        Debug.Log($"Correct index: {correctAnswerIndex}, Option: {localizedOptions[correctAnswerIndex]}");
    }

    bool OnOptionButtonClicked(Button button)
    {

        int index = -1;
        optionDict.TryGetValue(button, out index);

        foreach (var i in optionDict)
        {
            if (i.Value == correctAnswerIndex)
            {
                i.Key.GetComponent<Image>().color = Color.green;
            }
        }

        if (index == correctAnswerIndex)
        {
            //Debug.Log("Correct Answer");
            button.GetComponent<Image>().color = Color.green;
            //ChangeButtonColor(button, correctColor);
            //var budgetManager = HH_GameManager.Instance.currentPlayer.budgetManager;
            //budgetManager.IncreaseBudget(budgetManager.CalculateRewardBudget());
            //OnCorrectAnswer.Invoke();

            //answeredCorrectly = true;
            return true;
        }
        else
        {
            button.GetComponent<Image>().color = Color.red;
            return false;

        }
    }


    IEnumerator OnOptionButtonClickedRoutine(Button button)
    {
        foreach (var btn in optionDict.Keys)
        {
            btn.interactable = false;
        }

        bool isCorrect = OnOptionButtonClicked(button);
        
        yield return new WaitForSeconds(1f);

        quizPanel.SetActive(false);

        HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(false);

        if (isCorrect) 
        {
            yield return new WaitForSeconds(0.5f);
            OnCorrectAnswer?.Invoke();
        }

        gameObject.SetActive(false);
        //For reference
        /*OnOptionButtonClicked(button);
        foreach (var btn in optionDict.Keys)
        {
            btn.interactable = false;
        }
        yield return new WaitForSeconds(1f);

        quizPanel.SetActive(false);
        //hide earn more button
        HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(false);
        if (answeredCorrectly)
        {
            yield return new WaitForSeconds(0.5f);
            OnCorrectAnswer?.Invoke();
        }
        gameObject.SetActive(false);
        */

    }

    private void OnDisable()
    {
        HH_GameManager.Instance.currentPlayer.budgetManager.canEarnMoreMoney = false;
        for (int i = 0; i < buttonsParent.childCount; i++)
        {
            Destroy(buttonsParent.GetChild(i).gameObject);
        }
        optionDict.Clear();
        StopAllCoroutines();
    }

    public void ScriptedTutorialExample()
    {
        Debug.Log("This is a scripted tutorial example");
        foreach (var btn in optionDict.Keys)
        {
            if (optionDict[btn] != correctAnswerIndex)
                btn.interactable = false;
        }
    }
}
