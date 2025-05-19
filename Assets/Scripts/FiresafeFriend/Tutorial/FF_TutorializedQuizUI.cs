using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FF_TutorializedQuizUI : FF_TutorializedObject
{
    public FF_QuizPopupUI quizUI;
    public Button earnMoreButton;
    

    public override void OnTutorialStepStart()
    {
        earnMoreButton.gameObject.SetActive(true);
        ScaleEffect(earnMoreButton.GetComponent<RectTransform>()).onComplete += () =>
        {
            earnMoreButton.interactable = true;
        };
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
    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 0.5f)
                         .SetLoops(2, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }
    IEnumerator StepCompleteRoutine()
    {
        yield return new WaitForSeconds(3f);
        HH_GameManager.Instance.uiManager.HideStoreScreen();
        base.OnTutorialStepComplete();
    }
}
