using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FF_TutorializedPlantMode : FF_TutorializedObject
{
    public List<FF_Plants> bushes;
    public List<FF_DirtMound> dirtMounds;
    public GameObject leftArrow,rightArrow,plantModeToggle,dirtMoundsParent,bushesParent;
    public Color highlightColor;
    int bushesNeededToRemove;
    int dirtMoundsNeededToFill;
    Vector2 toggleEndPos, toggleStartPos;
    public override void Start()
    {
        toggleEndPos = plantModeToggle.GetComponent<RectTransform>().anchoredPosition;
        bushesNeededToRemove = bushes.Count;
        dirtMoundsNeededToFill = dirtMounds.Count;
        // move the plant mode toggle to the center of the screen
        RectTransform parentRect = plantModeToggle.transform.parent.GetComponent<RectTransform>();
        float centerX = parentRect.rect.width / 2;
        float centerY = -parentRect.rect.height / 2;
        toggleStartPos = new Vector2(centerX, centerY);
        plantModeToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                if(HH_GameManager.Instance.currentPlayer.playerTag == "P1")
                    ShowBushes();
                else
                {
                    Debug.Log("Show dirt mounds");
                    ShowDirtMound();
                }
            }
        });

    }

    public void ShowBushes()
    {     
        FF_TutorialManager.Instance.tutorialText.text = "Tap to remove the dead bushes in the critical zone.";
        foreach (var bush in bushes)
        {

            bush.OnCombustibleDestroyed += () =>
            {
                bushesNeededToRemove--;
                if (bushesNeededToRemove <= 0)
                {
                    bushes.Clear();
                    MoveToHouseTwo();
                    //OnTutorialStepComplete();
                }
            };
        }
    }

    public void ShowDirtMound()
    {
        FF_TutorialManager.Instance.tutorialText.text = "To decorate the yard, we can use fire-resistant plants. Click on the bubble to choose your plants.";
        //HH_GameManager.Instance.SwitchPlayer("P2");
        foreach (var mound in dirtMounds)
        {
            mound.OnPlanted += () =>
            {
                dirtMoundsNeededToFill--;
                if (dirtMoundsNeededToFill <= 0)
                {
                    //dirtMounds.Clear();
                    FF_TutorialManager.Instance.tutorialText.text = "Great job! Let’s move on to home hardening.";
                    DOVirtual.DelayedCall(2f, () => OnTutorialStepComplete());
                    
                }
            };

            mound.OnShoveled += () =>
            {
                dirtMoundsNeededToFill++;
            };
        }
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
        toggleSequence.Append(MoveToEndPosition(toggleRect,toggleEndPos));

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


    private void MoveToHouseTwo()
    {
        HH_GameManager.Instance.cameraController.OnCameraZoomComplete += () =>
        {
            //FF_TutorialManager.Instance.tutorialText.text = "Let's switch to plant mode first";
            rightArrow.SetActive(false);
            HH_GameManager.Instance.cameraController.OnCameraZoomComplete = null;
        };
        var canvasGroup = rightArrow.GetComponent<CanvasGroup>();
        var rect = rightArrow.GetComponent<RectTransform>();

        canvasGroup.interactable = false;
        Sequence arrowSequence = DOTween.Sequence();
        FF_TutorialManager.Instance.tutorialText.text = "Now that you’re done cleaning, click on the arrow to change houses.";
        arrowSequence.PrependInterval(1f);
        // Step 1: Fade In
        arrowSequence.Append(FadeIn(canvasGroup));

        // Step 2: Pulse Animation
        arrowSequence.Append(ScaleEffect(rect));

        arrowSequence.onComplete += () =>
        {
            canvasGroup.interactable = true;
        };
    }
}
