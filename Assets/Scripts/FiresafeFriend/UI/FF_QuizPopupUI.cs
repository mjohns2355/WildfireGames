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
    public GameObject quizOptionButtonPrefab, quizPanel;
    public Color normalColor, correctColor, wrongColor;
    public Action OnCorrectAnswer;
    private int correctAnswerIndex;
    private Question question;
    bool answeredCorrectly = false;

    void Start()
    {

    }

    public void InitQuizPopup()
    {
        question = HH_GameManager.Instance.quizManager.GetRandomQuestion();

        questionText.text = question.questionText;

        correctAnswerIndex = question.correctAnswerIndex - 1;

        for (int i = 0; i < question.options.Length; i++)
        {
            var obj = Instantiate(quizOptionButtonPrefab, buttonsParent);
            var button = obj.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];

            button.onClick.AddListener(() =>
            {
                StartCoroutine(OnOptionButtonClickedRoutine(button));
            });

            optionDict.Add(button, i);
        }

        quizPanel.SetActive(true);

        if (HH_GameManager.Instance.isTutorial)
        {
            ScriptedTutorialExample();
        }
    }

    void OnOptionButtonClicked(Button button)
    {
        int index = -1;
        optionDict.TryGetValue(button, out index);

        if (index == correctAnswerIndex)
        {
            button.GetComponent<Image>().color = Color.green;
            answeredCorrectly = true;
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

        quizPanel.SetActive(false);

        //hide earn more button
        HH_GameManager.Instance.uiManager.earnMoreMoney.gameObject.SetActive(false);

        if (answeredCorrectly)
        {
            yield return new WaitForSeconds(0.5f);
            OnCorrectAnswer?.Invoke();
        }

        gameObject.SetActive(false);
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
