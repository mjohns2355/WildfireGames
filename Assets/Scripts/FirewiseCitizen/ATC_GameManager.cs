using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class GameManager : UnitySingleton<GameManager>
{
    public CameraMovement cameraMovement;
    public ATC_RoadManager roadManager;
    public StructureManager structureManager;
    public ATC_InputManager inputManager;
    public FireManager fireManager;
    public AudioSource fireSFX;
    public FC_TutorialManager tutorialManager;
    //public ATC_dialogManager dialogManager;
    public bool canStartSim = false;
    public bool IsFirstSim { get { return currentStage == LevelStage.BeforeFirstSim; }}
    //public float GameSpeed { get; private set; }
    public List<HouseType> availableHouseTypes;
    public UnityEvent SimStartsEvent;
    public UnityEvent SimEndsEvent;
    public int currentLevel = 0;
    public int carsEvacuated, housesDestroyed, carsNotEvacuated, totalCars, totalHouses, houseHasHomeHardening = 0;
    public float firstEvacCarTimeStamp, lastEvacCarTimeStamp = 0f;
    [Range(0,1f)]
    public float houseFollowOrderChance,spawnCarChance = 1f;
    public LevelStage currentStage;
    public float SimTimer { get; private set; }
    public float SimulationTime => currentLevel switch
    {
        0 => 30f,
        1 => 45f,
        2 => 70f,
        _ => 30f 
    };
    public float SimulationSpeed => currentLevel switch
    {
        0 => 3f,
        1 => 2f,
        2 => 1f,
        _ => 1f
    };
    public bool SimIsEnd { get; private set; }
    public bool canControlCam;
    public int levelNum = 3;
    public bool IsLastLevel { get { return currentLevel + 1 == levelNum; } }
    public bool HasIncentives { get { return currentLevel > 0; } }
    public ATC_DialogTree[] houseDialogs;
    private int previousHousesDestroyed = 0; 
    private float previousFirstEvacTime, previousLastEvacTime = 0f;
    public Dictionary<HouseType,string> houseResponses = new();
    public Dictionary<HouseType, HouseChoice> finalChoices = new();
    //Quick Fix - This needs to be changed after IndieCade
    public GameObject toolBar;
    public GameObject topBanner;
    
    private bool isPaused = false;
    public float loadingTime = 1f;

    public override void Awake()
    {
        base.Awake();
        currentStage = LevelStage.Tutorial;
    }
    private void Start()
    {
        Time.timeScale = 1f;
        SimIsEnd = false;
        //InitiAvailableHouseType();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SimTimer = 0f;
        //Time.timeScale = GameSpeed = 2f;
        currentLevel = 0;
        if (currentStage == LevelStage.Tutorial)
        {
            tutorialManager.InitTutorialManager();
            canControlCam = false;
        }
        else
        {
            InitiAvailableHouseType();
            canControlCam = true;
        }
        DOTween.Clear(true);

    }


    public void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log($"Game is Paused: {isPaused}");
        canControlCam = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            ATC_UIController.Instance.TogglePauseMenu(true);
        }
        else
        {
            //Time.timeScale = GameSpeed;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            ATC_UIController.Instance.TogglePauseMenu(false);
        }
    }
    public void SkipSimulation()
    {
        Time.timeScale = 6f;
        currentStage = LevelStage.Skipping;
        fireManager.HideFireParticle();
        ATC_UIController.Instance.skippingOverlay.SetActive(true);

    }
    //public void SkipSimulationRec()
    //{
    //    currentStage = LevelStage.PhaseOne;
    //    ResetGame();
    //    Time.timeScale = 1f;
    //}
    private void Update()
    {
        //debug
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space)) { NextLevel(); }
        if(Input.GetKeyDown(KeyCode.P)) { Time.timeScale = 6f; }
        if (Input.GetKeyDown(KeyCode.R)) { ResetGame(); };
#endif
        // camera movement
        if (!canControlCam) return;
        cameraMovement.MoveCamera(new Vector3(inputManager.cameraMovementVector.x, 0, inputManager.cameraMovementVector.y));
        cameraMovement.ZoomCamera();

        // check if simulation is end
        if (!canStartSim) return;
        if (SimTimer < SimulationTime)
        {
            SimTimer += Time.deltaTime;
        }
        else if (!SimIsEnd)
        {

            if (IsFirstSim)
            {
                currentStage = LevelStage.AfterFirstSim;
            }
            SimIsEnd = true;
            OnSimEnd();
            SimEndsEvent.Invoke();
        }
    }

    // record if house follow the instruction
    public void UpdateHouseResponse(HouseType houseType, string response)
    {
        if (houseResponses.ContainsKey(houseType))
        {
            houseResponses[houseType] = response;
            return;
        }
        houseResponses.Add(houseType, response);
    }
    void OnSimEnd()
    {
        // clear all remaining cars
        if(currentStage == LevelStage.Skipping)
        {
            currentStage = LevelStage.PhaseOne;
            ATC_UIController.Instance.skippingOverlay.SetActive(false);
        }
        var remainingCars = GameObject.FindGameObjectsWithTag("Car");
        foreach (var car in remainingCars)
        {
            Destroy(car);
            fireSFX.Pause();
        }
        if(lastEvacCarTimeStamp == 0)
        {
            lastEvacCarTimeStamp = SimTimer;
        }
        carsNotEvacuated = ATC_AIDirector.Instance.currentCarNum;

        SaveResults();
    }

    void SaveResults()
    {
        //previousCarsEvacuated = carsEvacuated;
        previousHousesDestroyed = housesDestroyed;
        previousFirstEvacTime = firstEvacCarTimeStamp;
        previousLastEvacTime = lastEvacCarTimeStamp;
    }

    public void StartSimulation()
    {

        if (!IsFirstSim)
        {
            //Time.timeScale = GameSpeed = 1f ;
            //ATC_UIController.Instance.popUp.SetActive(true);
        }
        else
        {
            //Time.timeScale = GameSpeed = 2f;
            canStartSim = true;
            ATC_UIController.Instance.replayOverlay.SetActive(true);
        }
        //Debug.Log("Game Speed: " + GameSpeed);
        StartCoroutine(StartSimRoutine());
    }

    public void ToggleSimStatus(bool simStatus)
    {
        canStartSim = simStatus;
    }

    IEnumerator StartSimRoutine()
    {
        yield return new WaitUntil(()=>canStartSim);
        fireSFX.Play();
        SimStartsEvent.Invoke();
        SimIsEnd = false;
        foreach(var menu in ATC_UIController.Instance.contextMenus)
        {
            menu.ApplyBehavior();
        }
        //debug
#if UNITY_EDITOR
        yield return new WaitForSeconds(20f/SimulationSpeed);
        Debug.Log($"Total cars sapwned {ATC_AIDirector.Instance.spawnedCarNum}");
#endif
    }

    public void ResetGame(bool restartFromTutorial = false)
    {
       
        fireManager.wind.isStill = true;
        ATC_UIController.Instance.ShowLoadingScreen();
        InitiAvailableHouseType();
        //Debug.Log(currentStage);
        if (restartFromTutorial)
        {
            Debug.Log(" Restart game from tutorial");
            currentStage = LevelStage.Tutorial;
        }

        firstEvacCarTimeStamp = 0f;
        lastEvacCarTimeStamp = 0f;
        carsEvacuated = 0;
        housesDestroyed = 0;
        houseHasHomeHardening = 0;
        totalCars = 0;
        totalHouses = 0;
        SimTimer = 0;
        SimIsEnd = true;
        canStartSim = false;
        SimStartsEvent.RemoveAllListeners();
        SimEndsEvent.RemoveAllListeners();
        StopAllCoroutines();
        // unpause the game
        isPaused = false;
        Time.timeScale = 1f;
        ATC_UIController.Instance.ResetUI();
        
        fireSFX.Stop();
        ATC_AIDirector.Instance.currentCarNum = 0;
        ATC_AIDirector.Instance.spawnedCarNum = 0;
        var currentLevel = "FC_Level" + this.currentLevel.ToString();
        SceneManager.LoadScene(currentLevel);


    }

    public void RestartGame()
    {
        currentLevel = 0;
       
        ResetGame();
    }
    public void BackToMainMenu()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
        Destroy(ATC_UIController.Instance.gameObject);
        // reset time scale
        Time.timeScale = 1f;
    }
    public void RestartGameFromTutorial()
    {
        currentLevel = 0;
        ResetGame(true);
        
        tutorialManager.ReloadTutorial();
    }

    public void NextLevel()
    {
        currentLevel++;
        previousLastEvacTime = previousFirstEvacTime = 0f;
        previousHousesDestroyed = 0;
        //currentStage = LevelStage.BeforeFirstSim;
        //SkipSimulationRec();
        
        ResetGame();
        if (currentLevel == 1)
        {
            ATC_UIController.Instance.ShowDialog();
            ATC_UIController.Instance.houseDialogManager.StartDialogue("incentives");
        }
        //StartCoroutine(ShowIncentiveIntroduction());
        //previousHousesDestroyed = previousCarsEvacuated = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //DOTween.Clear(true);
        if (scene.name == "MainMenu") return;
        if (!scene.name.Contains("FC")) return;
        structureManager = FindObjectOfType<StructureManager>();
        roadManager = FindObjectOfType<ATC_RoadManager>();
        inputManager = FindObjectOfType<ATC_InputManager>();
        fireManager = FindObjectOfType<FireManager>();
        cameraMovement = FindObjectOfType<CameraMovement>();
        tutorialManager = FindObjectOfType<FC_TutorialManager>();
        if (tutorialManager && currentStage == LevelStage.Tutorial)
        {
            tutorialManager.InitTutorialManager();

        }
        else
        {
            InitiAvailableHouseType();
        }
        ATC_UIController.Instance.resetCamera.onClick.RemoveAllListeners();
        ATC_UIController.Instance.resetCamera.onClick.AddListener(() =>
        {
            cameraMovement.ResetCam();
        });
        //ATC_UIController.Instance.HideLoadingScreen();
        DOVirtual.DelayedCall(loadingTime, () =>
        {
            ATC_UIController.Instance.HideLoadingScreen();
        });

    }

    void InitiAvailableHouseType()
    {
        Debug.Log("Init Available House");
        if (availableHouseTypes.Count > 0)
        {
            availableHouseTypes.Clear();
        }


        switch (currentLevel)
        {
            case 0:
                availableHouseTypes.Add(HouseType.twoCar);
                availableHouseTypes.Add(HouseType.elderly);
                break;
            case 1:
                availableHouseTypes.Add(HouseType.wui);
                availableHouseTypes.Add(HouseType.kids);
                availableHouseTypes.Add(HouseType.pet);
                break;
            case 2:
                availableHouseTypes.Add(HouseType.twoCar);
                availableHouseTypes.Add(HouseType.wui);
                availableHouseTypes.Add(HouseType.kids);
                availableHouseTypes.Add(HouseType.elderly);
                availableHouseTypes.Add(HouseType.pet);
                break;
        }
    }

    public int CountFollowedInstructions()
    {
        int count = 0;
       
        foreach (var response in houseResponses.Values)
        {
            if (response == "Followed")
            {
                count++;
            }
        }
        Debug.Log($"Response Dict Count: {houseResponses.Values.Count} and Followed Count: {count}");
        return count;
    }
}
