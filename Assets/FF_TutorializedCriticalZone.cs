using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class FF_TutorializedCriticalZone : FF_TutorializedObject
{
    Vector2 toggleEndPos, toggleStartPos;
    public GameObject plantModeToggle;
    public HighlightMesh criticalZoneHighlight;

    public override void Start()
    {
        toggleEndPos = plantModeToggle.GetComponent<RectTransform>().anchoredPosition;

        // move the plant mode toggle to the center of the screen
        RectTransform parentRect = plantModeToggle.transform.parent.GetComponent<RectTransform>();
        float centerX = parentRect.rect.width / 2;
        float centerY = -parentRect.rect.height / 2;
        toggleStartPos = new Vector2(centerX, centerY);

        plantModeToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            criticalZoneHighlight.HighlightMeshes();
            FF_TutorialManager.Instance.tutorialText.text = "The critical zone is the <b>5 feet</b> around your house, and it’s important to clear dead plants, minimize furniture, and use fire-resistant fences and gates for protection.";
            plantModeToggle.GetComponent<Toggle>().interactable = false;
            DOVirtual.DelayedCall(5f  , () =>
            {
                Destroy(criticalZoneHighlight);
                OnTutorialStepComplete();
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

        //after the animation is done, show the tutorial panel
        toggleSequence.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            FF_TutorialManager.Instance.tutorialPanel.SetActive(true);

        });
    }

    private void PrepareToggle(CanvasGroup canvasGroup, RectTransform toggleRect)
    {
        toggleRect.anchoredPosition = toggleStartPos;
        canvasGroup.alpha = 0;
        FF_TutorialManager.Instance.tutorialPanel.SetActive(false);
    }

    private Tween FadeIn(CanvasGroup canvasGroup)
    {
        return canvasGroup.DOFade(1, 1f)
                          .SetEase(Ease.InOutQuad);
    }

    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 1f)
                         .SetLoops(4, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }

    private Tween MoveToEndPosition(RectTransform rect, Vector2 endPosition)
    {
        return rect.DOAnchorPos(endPosition, 1.5f)
                         .SetEase(Ease.InOutQuad);
    }
}
