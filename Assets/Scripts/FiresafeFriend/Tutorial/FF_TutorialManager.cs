using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
[System.Serializable]
public class FF_TutorialStep
{
    public int stepNumber;
    [TextArea (1,20)]
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
    public Button startTutorialYesButton,startTutorialNoButton;

    //public override void Awake()
    //{
    //    base.Awake();
    //    // Ensure DOTween is freshly initialized
    //    DOTween.KillAll();        // Kill all active tweens
    //    DOTween.Clear(true);      // Clear all cached tweens and Sequences
    //    DOTween.Init(true, true); // Force full reinitialization
    //}
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        cameraController = HH_GameManager.Instance.cameraController;
        nextButton.SetActive(false);
        nextButton.GetComponent<Button>().onClick.AddListener(() => ProceedToNextStep());
        // Start the tutorial with the intro panel
        introPanel.SetActive(true);
        startTutorialYesButton.onClick.AddListener(StartTutorial);
        startTutorialNoButton.onClick.AddListener(EndTutorial);
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

        if (step.zoomToObject)
        {
            cameraController.Zoomcamera(step.zoomPosition, false, step.zoomSize);
        }

        step.onStepStart?.Invoke();


        if (step.autoProceed)
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
            
            FF_TutorialStep step = tutorialSteps[currentStepIndex];
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
        //tutorialText.text = "Tutorial Complete!";
        nextButton.SetActive(false);
        // Load the game scene 
        SceneManager.LoadScene("FireSafeFriendScene");
    }


    public void UpdateTutorialText(string text)
    {
        tutorialText.text = text;
    }

}
