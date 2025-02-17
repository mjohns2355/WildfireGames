using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FF_TutorializedHousePart : FF_TutorializedObject
{
    public HousePartType partType;
    public Button storePurchaseButton;
    public HighlightMesh highlightMesh;
    PurchaseFloatingButton bubble;
    HouseManager houseManager;
    List<BaseHousePartObject> partObjects;

    public override void Start()
    {
       StartCoroutine(Routine());
       base.Start();

    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(1f);
        houseManager = HH_GameManager.Instance.currentPlayer;       
        partObjects = houseManager.GetAllHousePartObjectsOf(partType);
        storePurchaseButton.onClick.AddListener(() =>
        { 
            OnTutorialStepComplete();
        });
    }
    public void StepFourBehaviour()
    {
        Debug.Log("Step 4 behaviour");
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
        

    }

    public void StepFiveBehaviour()
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

    public override void OnTutorialStepComplete()
    {
        if(stepIndex == 3)
        {
            stepIndex++;
        }
        base.OnTutorialStepComplete();
    }

    void OnPartTapped(GameObject obj)
    {
        if (!FF_TutorialManager.Instance.tutorialPanel.activeInHierarchy) return;
        FF_TutorialManager.Instance.tutorialPanel.SetActive(false);


    }
}
