using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public bool autoProceed;
}
public class FF_TutorialManager : UnitySingleton<FF_TutorialManager>
{
    public List<FF_TutorialStep> tutorialSteps;
    [SerializeField]private int currentStepIndex = 0;
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
        nextButton.GetComponent<Button>().onClick.AddListener(() => ProceedToNextStep());
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
        Debug.Log($"Starting tutorial step {stepIndex + 1}");
        if (stepIndex >= tutorialSteps.Count)
        {
            Debug.LogError("Step index out of range.");
            return;
        }
        var step = tutorialSteps[stepIndex];
        tutorialText.text = step.description;
        tutorialPanel.SetActive(true);

        // Temp Implemetation for playing animation
        //foreach (var obj in step.animationObjects)
        //{
        //    if (obj) obj.SetActive(true);
        //}

        if (step.zoomToObject)
        {
            cameraController.Zoomcamera(step.zoomPosition, false, step.zoomSize);
        }

        step.onStepStart?.Invoke();

        if (/*step.onStepComplete.GetPersistentEventCount() > 0 &&*/ step.autoProceed)
        {
            Debug.Log("Step is auto-proceeding");
            step.onStepComplete.AddListener(() => ProceedToNextStep());
            nextButton.SetActive(false);
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
            //step.onStepStart.RemoveAllListeners();
            step.onStepComplete.RemoveAllListeners();

            currentStepIndex++;
            if (currentStepIndex < tutorialSteps.Count)
            {
                Debug.Log($"Proceeding to tutorial step {currentStepIndex+1}");
                StartStep(currentStepIndex);
            }
            else
            {
                Debug.Log($"End Tutorial");
                EndTutorial();
            }
        }
    }

    void EndTutorial()
    {
        tutorialText.text = "Tutorial Complete!";
        nextButton.SetActive(false);
        // Load the game scene 
        SceneManager.LoadScene("FireSafeFriendScene");
    }
}
