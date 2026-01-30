using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FF_TutorializedPlantMode : FF_TutorializedObject
{
    public List<FF_Plants> bushes;
    public List<FF_DirtMound> dirtMounds;

    public GameObject leftArrow, rightArrow, dirtMoundsParent, bushesParent;
    public Transform plantModeCamH2;

    int bushesNeededToRemove;
    int dirtMoundsNeededToFill;

    [Header("Localization Keys")]
    public string removeBushesKey = "tutText4";
    public string plantModeIntroKey = "tutText6";
    public string plantModeCompleteKey = "tutText7";
    public string switchHouseKey = "tutText5";

    public override void Start()
    {
        bushesNeededToRemove = bushes.Count;
        dirtMoundsNeededToFill = dirtMounds.Count;
    }

    public void ShowBushes()
    {
        FF_TutorialManager.Instance.UpdateTutorialText(
            StringManager.Instance.GetText(removeBushesKey)
        );

        foreach (var bush in bushes)
        {
            bush.OnCombustibleDestroyed += _ =>
            {
                bushesNeededToRemove--;

                if (bushesNeededToRemove <= 0)
                {
                    bushes.Clear();
                    MoveToHouseTwo();
                }
            };
        }
    }

    public void ShowDirtMound()
    {
        FF_TutorialManager.Instance.UpdateTutorialText(
            StringManager.Instance.GetText(plantModeIntroKey)
        );

        foreach (var mound in dirtMounds)
        {
            mound.SetBubbleState(true);

            mound.OnPlanted += _ =>
            {
                dirtMoundsNeededToFill--;

                if (dirtMoundsNeededToFill <= 0)
                {
                    FF_TutorialManager.Instance.UpdateTutorialText(
                        StringManager.Instance.GetText(plantModeCompleteKey)
                    );

                    onClick.AddListener(() =>
                    {
                        OnTutorialStepComplete();
                    });
                }
            };

            mound.OnShoveled += () =>
            {
                dirtMoundsNeededToFill++;
            };
        }
    }

    private Tween FadeIn(CanvasGroup canvasGroup)
    {
        return canvasGroup.DOFade(1, 0.2f)
                          .SetEase(Ease.InOutQuad);
    }

    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 0.3f)
                         .SetLoops(2, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }

    private void MoveToHouseTwo()
    {
        HH_GameManager.Instance.cameraController.OnCameraZoomComplete += () =>
        {
            rightArrow.SetActive(false);
            HH_GameManager.Instance.cameraController.OnCameraZoomComplete = null;
        };

        var canvasGroup = rightArrow.GetComponent<CanvasGroup>();
        var rect = rightArrow.GetComponent<RectTransform>();

        canvasGroup.interactable = false;

        Sequence arrowSequence = DOTween.Sequence();

        FF_TutorialManager.Instance.UpdateTutorialText(StringManager.Instance.GetText(switchHouseKey));

        arrowSequence.PrependInterval(1f);
        arrowSequence.Append(FadeIn(canvasGroup));
        arrowSequence.Append(ScaleEffect(rect));

        arrowSequence.onComplete += () =>
        {
            canvasGroup.interactable = true;

            rightArrow.GetComponent<Button>().onClick.RemoveAllListeners();
            rightArrow.GetComponent<Button>().onClick.AddListener(() =>
            {
                HH_GameManager.Instance.cameraController.Zoomcamera(
                    plantModeCamH2, true, 60
                );

                ShowDirtMound();
            });
        };
    }
}
