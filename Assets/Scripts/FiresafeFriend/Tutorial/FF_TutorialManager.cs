using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[Serializable]
public class FF_TutorialStep
{
    public int stepNumber;

    [TextArea(1, 3)]
    public string descriptionKey;

    [NonSerialized] public string localizedDescription;

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

    [SerializeField] private int currentStepIndex = 0;

    private HH_CameraController cameraController;
    private AudioSource audioSource;

    public TextMeshProUGUI tutorialText;
    public GameObject nextButton;
    public GameObject introPanel, tutorialPanel;
    public Button startTutorialYesButton, startTutorialNoButton;

    void Start()
    {
        Time.timeScale = 1;

        cameraController = HH_GameManager.Instance.cameraController;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (StringManager.Instance != null)
        {
            StringManager.Instance.OnStringsLoadedEvent += ResolveTutorialText;
        }

        ResolveTutorialText();

        nextButton.SetActive(false);
        nextButton.GetComponent<Button>().onClick.AddListener(() => ProceedToNextStep());

        introPanel.SetActive(true);
        startTutorialYesButton.onClick.AddListener(StartTutorial);
        startTutorialNoButton.onClick.AddListener(EndTutorial);
    }

    void ResolveTutorialText()
    {
        if (StringManager.Instance == null) return;

        foreach (var step in tutorialSteps)
        {
            step.localizedDescription =
                StringManager.Instance.GetText(step.descriptionKey);
        }

        // Refresh current step text if tutorial is already running
        if (tutorialPanel.activeSelf && currentStepIndex < tutorialSteps.Count)
        {
            tutorialText.text = tutorialSteps[currentStepIndex].localizedDescription;
        }
    }

    void StartTutorial()
    {
        introPanel.SetActive(false);
        tutorialPanel.SetActive(true);
        StartStep(currentStepIndex);
    }

    void StartStep(int stepIndex)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (stepIndex >= tutorialSteps.Count)
        {
            Debug.LogError("Step index out of range.");
            return;
        }

        var step = tutorialSteps[stepIndex];

        tutorialText.text = step.localizedDescription;
        tutorialPanel.SetActive(true);

        PlayAudioForKey(step.descriptionKey);

        if (step.zoomToObject)
        {
            cameraController.Zoomcamera(step.zoomPosition, false, step.zoomSize);
        }

        step.onStepStart?.Invoke();

        if (step.autoProceed)
        {
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
        nextButton.SetActive(false);
        SceneManager.LoadScene("FireSafeFriendScene");
    }

    public void UpdateTutorialText(string text)
    {
        tutorialText.text = text;
    }

    private void PlayAudioForKey(string key)
    {
        if (!TTSManager.IsEnabled || StringManager.Instance == null) return;

        string audioPath = StringManager.Instance.GetAudioPath(key);
        if (!string.IsNullOrEmpty(audioPath))
        {
            AudioClip clip = Resources.Load<AudioClip>(audioPath);
            if (clip != null && audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Audio clip not found: {audioPath}");
            }
        }
    }

    private void OnDestroy()
    {
        if (StringManager.Instance != null)
            StringManager.Instance.OnStringsLoadedEvent -= ResolveTutorialText;
    }
}
