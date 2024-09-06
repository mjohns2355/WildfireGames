using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
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
    //public ATC_dialogManager dialogManager;
    public bool canStartSim = false;
    public bool choseGoodOption = false;
    public bool IsFirstSim { get { return currentStage == LevelStage.BeforeFirstSim; }}
    public float GameSpeed { get; private set; }
    public List<HouseType> availableHouseTypes;
    public UnityEvent SimStartsEvent;
    public UnityEvent SimEndsEvent;
    public int CurrentLevel { get; private set; }
    public int carsEvacuated, housesDestroyed, carsNotEvacuated = 0;
    public float firstEvacCarTimeStamp, lastEvacCarTimeStamp = 0f;

    public LevelStage currentStage;
    public float SimTimer { get; private set; }
    public bool SimIsEnd { get; private set; }

    public bool IsLastLevel { get { return CurrentLevel + 1 > 1; } }
    //private int previousCarsEvacuated, previousHousesDestroyed = 0; 
    [SerializeField]private float previousFirstEvacTime, previousLastEvacTime = 0f;

    public override void Awake()
    {
        base.Awake();
        currentStage = LevelStage.BeforeFirstSim;
    }
    private void Start()
    {
        SimIsEnd = false;
        InitiAvailableHouseType();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SimTimer = 0f;
        Time.timeScale = GameSpeed = 2f;
        CurrentLevel = 0;
        //inputManager.OnMouseClick += structureManager.ClickStructre;
        //inputManager.OnMouseClick += HandleMouseClick;
        //uiController.OnRoadPlacement += RoadPlacementHandler;
        //uiController.OnHousePlacement += HousePlacementHandler;
        //uiController.OnSpecialPlacement += SpecialPlacementHandler;
    }

    //private void RoadPlacementHandler()
    //{
    //    ClearInputAction();
    //    inputManager.OnMouseClick += roadManager.PlaceRoad;
    //    inputManager.OnMouseHold += roadManager.PlaceRoad;
    //    inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    //}

    //private void HousePlacementHandler()
    //{

    //    ClearInputAction();
    //    //inputManager.OnMouseClick += structureManager.PlaceHouse;
    //}
    //private void SpecialPlacementHandler()
    //{

    //    ClearInputAction();
    //    //inputManager.OnMouseClick += structureManager.PlaceSpecial;

    //}
    //private void HandleMouseClick(Vector3Int position)
    //{
    //    Debug.Log(position);
    //   // roadManager.PlaceRoad(position);
    //}

    private void Update()
    {
        //debug
        //if(Input.GetKeyDown(KeyCode.Space)) { NextLevel(); }
        //if(Input.GetKeyDown(KeyCode.LeftShift)) { Time.timeScale = 6f; }

        cameraMovement.MoveCamera(new Vector3(inputManager.cameraMovementVector.x, 0, inputManager.cameraMovementVector.y));
        cameraMovement.ZoomCamera(inputManager.cameraZoomAxis);

        if (!canStartSim) return;
        // don't forget to set it back to 70
        if (SimTimer < 70)
        {
            SimTimer += Time.deltaTime;
        }
        else if (!SimIsEnd)
        {
 
            if (!IsFirstSim)
            {
                //check win/lose
                currentStage = IsGameWon() ? LevelStage.Win : LevelStage.Lose;
            }
            else
            {
                currentStage = LevelStage.AfterFirstSim;
            }
            SimIsEnd = true;
            OnSimEnd();
            SimEndsEvent.Invoke();
        }
    }

    void OnSimEnd()
    {
        var remainingCars = GameObject.FindGameObjectsWithTag("Car");
        foreach (var car in remainingCars)
        {
            Destroy(car);
        }
        if(lastEvacCarTimeStamp == 0)
        {
            lastEvacCarTimeStamp = SimTimer;
        }
        carsNotEvacuated = ATC_AIDirector.Instance.spawnedCarNum;
        SaveResults();
    }

    void SaveResults()
    {
        //previousCarsEvacuated = carsEvacuated;
        //previousHousesDestroyed = housesDestroyed;
        previousFirstEvacTime = firstEvacCarTimeStamp;
        previousLastEvacTime = lastEvacCarTimeStamp;
    }

    bool IsGameWon()
    {
        bool won = false;
        int first = Mathf.RoundToInt(firstEvacCarTimeStamp);
        int final = Mathf.RoundToInt(lastEvacCarTimeStamp);
        if (first < (int) previousFirstEvacTime && final < (int) previousLastEvacTime)
        {
            won = true;
        }
        Debug.Log("Game win? " + won);
        return won;
    }
    //private void ClearInputAction()
    //{
    //    inputManager.OnMouseClick = null;
    //    inputManager.OnMouseHold = null;
    //    inputManager.OnMouseUp = null;
    //}

    //public void ToggleConstructionMode()
    //{
    //    constructionMode = !constructionMode;
    //    if (constructionMode == false)
    //    {
    //        ClearInputAction();
    //        inputManager.OnMouseClick += structureManager.ClickStructre;
    //    }
    //    //uiController.UpdateConstructionMode(constructionMode);
    //    inputManager.OnConstructionMode(constructionMode);
    //    Debug.Log(constructionMode);
    //}

    public void StartSimulation()
    {
        if(!IsFirstSim)
        {
            Time.timeScale = GameSpeed = 1f ;
            canStartSim = choseGoodOption;
            if (!choseGoodOption)
            {
                ATC_UIController.Instance.popUp.SetActive(true);
            }
        }
        else
        {
            canStartSim = true;
        }
        StartCoroutine(StartSimRoutine());
    }

    public void ToggleSimStatus(bool simStatus)
    {
        canStartSim = simStatus;
    }

    IEnumerator StartSimRoutine()
    {
        yield return new WaitUntil(()=>canStartSim);
        SimStartsEvent.Invoke();
        foreach(var menu in ATC_UIController.Instance.contextMenus)
        {
            menu.ApplyBehavior();
        }

        

        //debug
        yield return new WaitForSeconds(10f);
        Debug.Log($"Total cars sapwned {ATC_AIDirector.Instance.spawnedCarNum}");
    }

    public void ToggleGamePause(bool state)
    {
        Time.timeScale = state ? 0 : 1;
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

    public void ResetGame(int level = 1)
    {
        if(!IsFirstSim) {
            currentStage = LevelStage.PhaseOne;
        }
        firstEvacCarTimeStamp = 0f;
        lastEvacCarTimeStamp = 0f;
        carsEvacuated = 0;
        housesDestroyed = 0;
        SimTimer = 0;
        SimIsEnd = false;
        canStartSim = false;
        InitiAvailableHouseType();
        SimStartsEvent.RemoveAllListeners();
        SimEndsEvent.RemoveAllListeners();
        StopAllCoroutines();
        ATC_UIController.Instance.ResetUI();
        //if(level == 0)
        //{
        //    CurrentLevel = level;   
        //}
        SceneManager.LoadScene(CurrentLevel);
    }

    public void NextLevel()
    {
        CurrentLevel++;
        previousLastEvacTime = previousFirstEvacTime = 0f;
        currentStage = LevelStage.BeforeFirstSim;
        ResetGame();

        //previousHousesDestroyed = previousCarsEvacuated = 0;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        structureManager = FindObjectOfType<StructureManager>();
        roadManager = FindObjectOfType<ATC_RoadManager>();
        inputManager = FindObjectOfType<ATC_InputManager>();
        fireManager = FindObjectOfType<FireManager>();
        cameraMovement = FindObjectOfType<CameraMovement>();
        //dialogManager = FindObjectOfType<ATC_dialogManager>();
        //uiController = FindObjectOfType<ATC_UIController>();
    }

    void InitiAvailableHouseType()
    {
        if(availableHouseTypes.Count > 0)
        {
            availableHouseTypes.Clear();
        }
        
        if (CurrentLevel == 0)
        {
            availableHouseTypes.Add(HouseType.twoCar);
            availableHouseTypes.Add(HouseType.wui);
        }
        else
        {
            for (int i = 1; i < Enum.GetValues(typeof(HouseType)).Length; i++)
            {
                var houseType = (HouseType)i;
                availableHouseTypes.Add(houseType);
            }
        }
    }
}
