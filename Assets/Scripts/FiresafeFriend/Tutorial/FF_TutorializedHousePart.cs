using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FF_TutorializedHousePart : FF_TutorializedObject
{
    public HousePartType partType;
    public Button storePurchaseButton;
    public HighlightMesh highlightMesh;
    public GameObject plantModeToggle;
    public Transform camH1;
    PurchaseFloatingButton bubble;
    HouseManager houseManager;
    List<BaseHousePartObject> partObjects;

    public override void OnTutorialStepStart()
    {
        plantModeToggle.GetComponent<Toggle>().onValueChanged.AddListener((vlaue) =>
        {
            SwitchToHouseMode();
        });

        storePurchaseButton.onClick.AddListener(() =>
        {
            OnTutorialStepComplete();
        });

        ShowToggle();
    }

    public void ShowToggle()
    {
        plantModeToggle.SetActive(true);
        DOVirtual.DelayedCall(3f, () =>
        {
            FF_TutorialManager.Instance.tutorialText.text = "Click on the toggle to switch to “House Mode”";
        });
        Sequence toggleSequence = DOTween.Sequence();
        toggleSequence.PrependInterval(5f);
        toggleSequence.Append(ScaleEffect(plantModeToggle.GetComponent<RectTransform>()));
        toggleSequence.onComplete += () =>
        {
            plantModeToggle.GetComponent<Toggle>().interactable = true;
        };

    }
    private Tween ScaleEffect(RectTransform rect)
    {
        return rect.DOScale(Vector3.one * 1.5f, 1f)
                         .SetLoops(4, LoopType.Yoyo)
                         .SetEase(Ease.InOutQuad);
    }
    void SwitchToHouseMode()
    {
        FF_TutorialManager.Instance.tutorialText.text = "Let’s try upgrading the roof. Tap to see the roofing options.";
        HH_GameManager.Instance.cameraController.Zoomcamera(camH1, true, 60);
        houseManager = HH_GameManager.Instance.currentPlayer;
        partObjects = houseManager.GetAllHousePartObjectsOf(partType);


        foreach (var part in partObjects)
        {
            if (part.shouldDisplayBubble)
            {
                bubble = HH_GameManager.Instance.uiManager.SpawnBubble();
                bubble.InitBubbleForHousePart(part);
                part.bubble = bubble;
                bubble.button.enabled = false;
            }
            break;
        }

        ShowUpgradeHousePart();
    }
    public void ShowUpgradeHousePart()
    {
        Debug.Log("Step 5 behaviour");
        HH_GameManager.Instance.inputManager.OnObjectSelected += OnPartTapped;
        bubble.button.enabled = true;
        bubble.button.onClick.AddListener(() =>
        {
            OnPartTapped(gameObject);
        });
        //enable house part clickingß
        //HH_GameManager.Instance.SetGameStart(true);
        foreach (var part in partObjects)
        {
            part.isClickable = true;
            highlightMesh.meshRenders.AddRange(part.meshes);
        }
        highlightMesh.HighlightMeshes();

    }



    void OnPartTapped(GameObject obj)
    {
        
        //if (!FF_TutorialManager.Instance.tutorialPanel.activeInHierarchy) return;
        //FF_TutorialManager.Instance.tutorialPanel.SetActive(false);
        FF_TutorialManager.Instance.tutorialText.text = "Each material is graded for fire resistance, with Class A being the highest rating. Let’s purchase this metal roofing.";
    }

}
