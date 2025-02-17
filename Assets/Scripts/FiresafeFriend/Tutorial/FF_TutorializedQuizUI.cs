using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FF_TutorializedQuizUI : FF_TutorializedObject
{
    public FF_QuizPopupUI quizUI;
    public Button earnMoreButton;
    
    public override void Start()
    {
        quizUI.OnCorrectAnswer += OnScriptedQuizFinished;
        quizUI.close.onClick.AddListener(OnScriptedQuizFinished);
        earnMoreButton.onClick.AddListener(() =>
        {
            FF_TutorialManager.Instance.tutorialPanel.SetActive(false);
        });
    }
    void OnScriptedQuizFinished()
    {
        HH_GameManager.Instance.uiManager.ShowStoreScreen(HousePartType.Roof);
        // disable part buttons in store
        foreach (var btn in FindObjectsOfType<PartButton>())
        {
            btn.GetComponent<Button>().interactable = false;
        }
        StartCoroutine(StepCompleteRoutine());
    }

    IEnumerator StepCompleteRoutine()
    {
        yield return new WaitForSeconds(3f);
        HH_GameManager.Instance.uiManager.HideStoreScreen();
        base.OnTutorialStepComplete();
    }
}
