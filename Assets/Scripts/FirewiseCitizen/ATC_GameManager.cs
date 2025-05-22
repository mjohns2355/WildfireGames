using cakeslice;
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
    public float simulationTime = 70f;
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
    public override void Awake()
    {
        base.Awake();
        currentStage = LevelStage.BeforeFirstSim;
    }
    private void Start()
    {
        Time.timeScale = 1f;
        SimIsEnd = false;
        InitiAvailableHouseType();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SimTimer = 0f;
        //Time.timeScale = GameSpeed = 2f;
        currentLevel = 0;
        tutorialManager.InitTutorialManager();
        canControlCam = false;
        DOTween.Clear(true);

    }


    public void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log($"Game is Paused: {isPaused}");
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

    public void SkipSimulationRec()
    {
        currentStage = LevelStage.PhaseOne;
        ResetGame();
        Time.timeScale = 1f;
    }
    private void Update()
    {
        //debug
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space)) { NextLevel(); }
        if(Input.GetKeyDown(KeyCode.P)) { Time.timeScale = 6f; }
        if (Input.GetKeyDown(KeyCode.R)) { RestartGameFromTutorial(); }
#endif
        // camera movement
        if (!canControlCam) return;
        cameraMovement.MoveCamera(new Vector3(inputManager.cameraMovementVector.x, 0, inputManager.cameraMovementVector.y));
        cameraMovement.ZoomCamera();

        // check if simulation is end
        if (!canStartSim) return;
        if (SimTimer < simulationTime)
        {
            SimTimer += Time.deltaTime;
        }
        else if (!SimIsEnd)
        {

            //if (!IsFirstSim)
            //{
            //    //check win/lose
            //    currentStage = IsGameWon() ? LevelStage.Win : LevelStage.Lose;
            //}
            //else
            //{
            if (IsFirstSim)
            {
                currentStage = LevelStage.AfterFirstSim;
            }
                
            //}
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

    bool IsGameWon()
    {
        bool won = false;
        int first = Mathf.RoundToInt(firstEvacCarTimeStamp);
        int final = Mathf.RoundToInt(lastEvacCarTimeStamp);
        if (first < (int) previousFirstEvacTime && final < (int) previousLastEvacTime /*&& housesDestroyed < previousHousesDestroyed*/)
        {
            won = true;
        }
        Debug.Log("Game win? " + won);
        return won;
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
        yield return new WaitForSeconds(20f);
        Debug.Log($"Total cars sapwned {ATC_AIDirector.Instance.spawnedCarNum}");
#endif
    }


    //public void ToggleAssignMode()
    //{
    //    assignMode = !assignMode;
    //    if (assignMode == false)
    //    {
    //        ClearInputAction();
    //        inputManager.OnMouseClick += structureManager.ClickStructre;
    //    }
    //    uiController.UpdateConstructionMode(constructionMode);
    //    inputManager.OnConstructionMode(constructionMode);
    //    Debug.Log(assignMode);
    //}


    //public string[] ParseString( string str, char[] delimiterChars)
    //{
    //    string[] words = str.Split(delimiterChars);

    //    return words;
    //}

    public void ResetGame(bool restartFromTutorial = false)
    {
        fireManager.wind.isStill = true;
        ATC_UIController.Instance.ShowLoadingScreen();

        //Debug.Log(currentStage);
        if (restartFromTutorial)
        {
            currentStage = LevelStage.Tutorial;
        }
        else
        {
            currentStage = LevelStage.PhaseOne;
        }
        //else if (/*!IsFirstSim && */currentStage!= LevelStage.Tutorial) {
        //    currentStage = LevelStage.PhaseOne;
        //}
        firstEvacCarTimeStamp = 0f;
        lastEvacCarTimeStamp = 0f;
        //Time.timeScale = GameSpeed = 2f;
        carsEvacuated = 0;
        housesDestroyed = 0;
        houseHasHomeHardening = 0;
        totalCars = 0;
        totalHouses = 0;
        SimTimer = 0;
        SimIsEnd = true;
        canStartSim = false;
        InitiAvailableHouseType();
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
        //ATC_UIController.Instance.houseDialogManager.ResetFlags();
        var currentLevel = "FC_Level" + this.currentLevel.ToString();
        SceneManager.LoadScene(currentLevel);

        //TO-DO: Multiple levels
        //SceneManager.LoadScene("FC_Level0");

    }

    public void RestartGame()
    {
        currentLevel = 0;
        //currentStage = LevelStage.BeforeFirstSim;
        //Time.timeScale = GameSpeed = 2f;
        //ATC_UIController.Instance.startPrompt.SetActive(true);
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
        
        //previousHousesDestroyed = previousCarsEvacuated = 0;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //DOTween.Clear(true);
        if (scene.name == "MainMenu") return;
        
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
        ATC_UIController.Instance.resetCamera.onClick.RemoveAllListeners();
        ATC_UIController.Instance.resetCamera.onClick.AddListener(() =>
        {
            cameraMovement.ResetCam();
        });
        //dialogManager = FindObjectOfType<ATC_dialogManager>();
        //uiController = FindObjectOfType<ATC_UIController>();
    }

    void InitiAvailableHouseType()
    {
        if (availableHouseTypes.Count > 0)
        {
            availableHouseTypes.Clear();
        }
        //availableHouseTypes = Enum.GetValues(typeof(HouseType))
        //                             .Cast<HouseType>()
        //                             .Where(type => type != HouseType.none)
        //                             .ToList();
        //if (CurrentLevel == 0)
        //{
        //    availableHouseTypes.Add(HouseType.twoCar);
        //    availableHouseTypes.Add(HouseType.wui);
        //    // test version
        //    availableHouseTypes.Add(HouseType.kids);
        //    availableHouseTypes.Add(HouseType.elderly);
        //    availableHouseTypes.Add(HouseType.pet);
        //}
        //else
        //{
        //    for (int i = 1; i < Enum.GetValues(typeof(HouseType)).Length; i++)
        //    {
        //        var houseType = (HouseType)i;
        //        availableHouseTypes.Add(houseType);
        //    }
        //}

        switch (currentLevel)
        {
            case 0:
                availableHouseTypes.Add(HouseType.twoCar);
                availableHouseTypes.Add(HouseType.wui);
                break;
            case 1:
                availableHouseTypes.Add(HouseType.twoCar);
                availableHouseTypes.Add(HouseType.wui);
                availableHouseTypes.Add(HouseType.kids);
                availableHouseTypes.Add(HouseType.elderly);
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
