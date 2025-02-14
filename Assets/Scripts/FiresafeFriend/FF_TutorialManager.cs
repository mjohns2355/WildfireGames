using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[System.Serializable]
public class FF_TutorialStep
{
    public int stepNumber;
    public string description;
    //public List<GameObject> animationObjects = new List<GameObject>();
    public bool zoomToObject;
    public Transform zoomPosition;
    public float zoomSize;
    public UnityEvent onStepStart;
    public UnityEvent onStepComplete;
}
public class FF_TutorialManager : MonoBehaviour
{
    public List<FF_TutorialStep> tutorialSteps;
    private int currentStepIndex = 0;
    private HH_CameraController cameraController;
    public TextMeshProUGUI tutorialText;
    public GameObject nextButton;
    public GameObject introPanel, tutorialPanel; 
    public Button introStartButton; 
    // Start is called before the first frame update
    void Start()
    {
        cameraController = HH_GameManager.Instance.cameraController;
        nextButton.SetActive(false);

        // Start the tutorial with the intro panel
        introPanel.SetActive(true);
        introStartButton.onClick.AddListener(() => StartTutorial());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartTutorial()
    {
        introPanel.SetActive(false);
        tutorialPanel.SetActive(true);
        StartStep(currentStepIndex);
    }

    void StartStep(int stepIndex)
    {
        if(stepIndex >= tutorialSteps.Count)
        {
            Debug.LogError("Step index out of range.");
            return;
        }
        var step = tutorialSteps[stepIndex];
        tutorialText.text = step.description;

        // Temp Implemetation for playing animation
        //foreach (var obj in step.animationObjects)
        //{
        //    if (obj) obj.SetActive(true);
        //}

        if (step.zoomToObject)
        {
            cameraController.Zoomcamera(step.zoomPosition, step.zoomSize);
        }

        step.onStepStart?.Invoke();

        if (step.onStepComplete.GetPersistentEventCount() > 0)
        {
            step.onStepComplete.AddListener(() => ProceedToNextStep());
        }
        else
        {
            nextButton.SetActive(true);
        }
    }

    void ProceedToNextStep()
    {
        if (currentStepIndex < tutorialSteps.Count)
        {
            // Clean up the previous step
            FF_TutorialStep step = tutorialSteps[currentStepIndex];
            //foreach (var obj in step.animationObjects)
            //{
            //    if (obj) obj.SetActive(false);
            //}
            step.onStepComplete.RemoveAllListeners();

            currentStepIndex++;
            if (currentStepIndex < tutorialSteps.Count)
            {
                StartStep(currentStepIndex);
            }
            else
            {
                EndTutorial();
            }
        }
    }

    void EndTutorial()
    {
        tutorialText.text = "Tutorial Complete!";

        // Load the game scene 
        
    }
}
