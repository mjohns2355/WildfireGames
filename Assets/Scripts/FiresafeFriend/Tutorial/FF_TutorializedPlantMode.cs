using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FF_TutorializedPlantMode : FF_TutorializedObject
{
    public List<FF_Plants> bushes;
    public List<FF_DirtMound> dirtMounds;
    public GameObject leftArrow,rightArrow,dirtMoundsParent,bushesParent;
    public Transform plantModeCamH2;
    int bushesNeededToRemove;
    int dirtMoundsNeededToFill;

    public override void Start()
    {
        bushesNeededToRemove = bushes.Count;
        dirtMoundsNeededToFill = dirtMounds.Count;

    }

    public void ShowBushes()
    {     
        //FF_TutorialManager.Instance.tutorialText.text = "Tap to remove the dead bushes in the critical zone.";
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
            mound.SetBubbleState(true);
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
            rightArrow.GetComponent<Button>().onClick.RemoveAllListeners();
            rightArrow.GetComponent<Button>().onClick.AddListener(() =>
            {
                HH_GameManager.Instance.cameraController.Zoomcamera(plantModeCamH2, true, 60);
                ShowDirtMound();
            });
        };
    }
}
