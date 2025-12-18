using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FF_TutorializedCriticalZone : FF_TutorializedObject
{
    Vector2 toggleEndPos, toggleStartPos;

    public GameObject plantModeToggle;
    public RectTransform parentRect;
    public HighlightMesh criticalZoneHighlight;
    public float waitTime;

    [Header("Localization Keys")]
    public string criticalZoneInfoKey = "tutText2";

    public override void Start()
    {
        plantModeToggle.SetActive(false);
        toggleEndPos = plantModeToggle.GetComponent<RectTransform>().anchoredPosition;

        // move the plant mode toggle to the center of the screen
        float centerX = parentRect.rect.width / 2;
        float centerY = -parentRect.rect.height / 2;
        toggleStartPos = new Vector2(centerX, centerY);

        plantModeToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            criticalZoneHighlight.HighlightMeshes();

            FF_TutorialManager.Instance.UpdateTutorialText(
                StringManager.Instance.GetText(criticalZoneInfoKey)
            );

            plantModeToggle.GetComponent<Toggle>().interactable = false;

            DOVirtual.DelayedCall(
                criticalZoneHighlight.flashDuration * criticalZoneHighlight.flashCount,
                () =>
                {
                    onClick.AddListener(() =>
                    {
                        Destroy(criticalZoneHighlight);
                        OnTutorialStepComplete();
                        plantModeToggle.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                    });
                });
        });
    }

    public void MoveIcon()
    {
        var canvasGroup = plantModeToggle.GetComponent<CanvasGroup>();
        var toggleRect = plantModeToggle.GetComponent<RectTransform>();

        // Start Position and Visibility
        PrepareToggle(canvasGroup, toggleRect);

        // Create the Animation Sequence
        Sequence toggleSequence = DOTween.Sequence();

        toggleSequence.PrependInterval(1.5f);

        // Step 1: Fade In
        toggleSequence.Append(FadeIn(canvasGroup));

        // Step 2: Pulse Animation
        toggleSequence.Append(ScaleEffect(toggleRect));

        // Step 3: Move to End Position
        toggleSequence.Append(MoveToEndPosition(toggleRect, toggleEndPos));

        // After animation is done, show the tutorial panel
        toggleSequence.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            FF_TutorialManager.Instance.tutorialPanel.SetActive(true);
        });
    }

    private void PrepareToggle(CanvasGroup canvasGroup, RectTransform toggleRect)
    {
        plantModeToggle.SetActive(true);
        toggleRect.anchoredPosition = toggleStartPos;
        canvasGroup.alpha = 0;
        FF_TutorialManager.Instance.tutorialPanel.SetActive(false);
    }

    private Tween FadeIn(CanvasGroup canvasGroup)
    {
        return canvasGroup.DOFade(1, 0.2f)
                          .SetEase(Ease.InOutQuad);
    }

    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 0.5f)
                         .SetLoops(4, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }

    private Tween MoveToEndPosition(RectTransform rect, Vector2 endPosition)
    {
        return rect.DOAnchorPos(endPosition, 1.5f)
                   .SetEase(Ease.InOutQuad);
    }
}
