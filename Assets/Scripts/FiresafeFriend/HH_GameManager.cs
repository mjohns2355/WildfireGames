using HappyHouse.FireSystem;
using HappyHouse.HouseSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public bool isTutorial;
    public bool IsFirstRound { get => currentRoundCount == 0; }
    public HappyHouse.FireSystem.FireManager fireManager;
    public Transform h1, h2, h1CamPos,h2CamPos,h1PlantCamPos,h2PlantCamPos;
    public float fireTimer = 60f;
    public float fireChance = 0.5f; // 50% chance to start fire
    public int maxRounds = 10;
    public Action OnRoundStart, OnRoundEnd;
    public Action<bool> OnPlantModeChanged;
    public HH_UIManager uiManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;
    public HH_CameraController cameraController;
    public QuizManager quizManager;
    public FF_skybox skyboxController;

    public bool IsGameStarted { get => _currentStage != GameStage.BeforeGame; }
    public bool IsFireStarted {  get => _currentStage == GameStage.Fire; }
    private GameObject[] fences;
    public List<BaseHousePartObject> publicFences;
    public HouseManager p1;
    public HouseManager p2;
    [SerializeField]private bool _isPlantMode;
    private int consecutiveCompetitionCount,currentRoundCount = 0;
    private bool mustForceCompetition = false;
    public bool IsPlantMode
    {
        get => _isPlantMode;
        set
        {
            if (_isPlantMode != value)
            {
                _isPlantMode = value;
                OnPlantModeChanged?.Invoke(_isPlantMode);
            }
        }
    }

    [SerializeField] private GameStage _currentStage;
    public GameStage CurrentStage
    {
        get => _currentStage;
        set
        {
            if (_currentStage == value) return;

            var previousStage = _currentStage;
            _currentStage = value;
            Debug.Log($"GameStage changed from {previousStage} to {_currentStage}");

            OnStageChanged(_currentStage);
        }
    }


    public override void Awake()
    {
        shouldNotDestroyOnLoad = false;

        base.Awake();
    }
    private void Start()
    {
        CurrentStage = GameStage.BeforeGame;
        _isPlantMode = false;
        SpawnHouses();
    }

    private void Update()
    {

        if (!IsFireStarted) return;
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
        else
        {
            OnFireEnd();
        }
        //debug
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 5.0f;
        }
    }

    private void InitPublicFences(HouseManager currentPlayer)
    {
        fences = GameObject.FindGameObjectsWithTag("Fence");
        foreach (var f in fences)
        {
            if(f.TryGetComponent<BaseHousePartObject>(out var fence))
            {
                fence.InitHousePartObject(currentPlayer);
                var info = Instantiate(fence.partInfo, transform);
                info.isPublic = true;
                fence.partInfo = info;
                publicFences.Add(fence);
                currentPlayer.inventory.AddNewPartToInventory(info);
            }

        }
    }
    public void SpawnHouses()
    {
        var houses = new List<GameObject>(ResourceManager.Instance.houses);
        if (houses.Count < 2)
        {
            Debug.LogError("Not enough houses to assign different ones to P1 and P2.");
            return;
        }

        int index1 = UnityEngine.Random.Range(0, houses.Count);
        var house1Prefab = houses[index1];
        houses.RemoveAt(index1);

        int index2 = UnityEngine.Random.Range(0, houses.Count);
        var house2Prefab = houses[index2];

        var h1Instance = Instantiate(house1Prefab, h1);
        p1 = h1Instance.GetComponent<HouseManager>();
        p1.playerTag = "P1";       
        p1.arrowUI = uiManager.rightArrow.gameObject;
        // flip the model 
        h1Instance.transform.localScale = new Vector3(h1Instance.transform.localScale.x * -1 , 1, 1);
        p1.nameText.transform.localScale = new Vector3(p1.nameText.transform.localScale.x * -1, 1, 1);

        var h2Instance = Instantiate(house2Prefab, h2);
        p2 = h2Instance.GetComponent<HouseManager>();
        p2.playerTag = "P2";
        p2.arrowUI = uiManager.leftArrow.gameObject;
    }

    void OnFireEnd()
    {
        //IsFireStarted = false;
        var fires = FindObjectsOfType<FF_FireController>();
        var combustibles = FindObjectsOfType<FF_BaseCombustible>();
        skyboxController.ChangeSky(false);
        foreach (var c in combustibles)
        {
            c.isOnFire = false;
            c.StopAllCoroutines();
        }
        foreach (var f in fires)
        {
            Destroy(f.gameObject);
        }
        uiManager.ShowEndScreen(true,p1.GetBurnedPercent(),p2.GetBurnedPercent());
    }

    void OnCompetition()
    {
        var p1Socre = p1.CalculateRating();
        var p2Socre = p2.CalculateRating();
        uiManager.ShowEndScreen(false, p1Socre, p2Socre);
    }
    public void SwitchPlayer (string playerTag)
    {
        currentPlayer.OnHouseDeselected();
        List<HousePartInfo> ownedPublicFences = new List<HousePartInfo>();

        if (currentPlayer != null && !currentPlayer.inventory.ownedPublicParts.TryGetValue(HousePartType.Fence,out ownedPublicFences))
        {
            ownedPublicFences = new List<HousePartInfo>();
        }
        uiManager.HideStoreScreen();
        uiManager.HidePlantsMenu();
        IsPlantMode = false;
        if (playerTag == "P1")
        {
            currentPlayer = p1;
        }
        else if(playerTag == "P2")
        {
            currentPlayer = p2;
        }
        else
        {
            Debug.Log($"player {playerTag} doesn't exist!");
            return;
        }

        Debug.Log($"Current Player is {currentPlayer.playerTag}");
        if (ownedPublicFences.Count > 0)
        {
            foreach (var fence in ownedPublicFences)
            {
                currentPlayer.inventory.AddNewPartToInventory(fence);
            }
        }
        inputManager.OnHouseSelected.Invoke(currentPlayer);
        //currentPlayer.OnHouseSelected(currentPlayer);
    }

    public void StartRound(HouseManager currentPlayer)
    {
        CurrentStage = GameStage.RoundStart;
        this.currentPlayer = currentPlayer;
        if (isTutorial) return;
        //uiManager.OnRoundStart();
        InitPublicFences(currentPlayer);
    }
    //public BaseHousePartObject CreateHousePartObject(HousePartInfo partInfo, HouseManager owner)
    //{
    //    var obj = new GameObject(partInfo.partID);
    //    var houseObj = obj.AddComponent<BaseHousePartObject>();
    //    houseObj.InitHousePartObject(owner,partInfo );
    //    return houseObj;
    //}

    public void StartFire()
    {
        //IsFireStarted = true;
        CurrentStage = GameStage.Fire;
        uiManager.floatingIcons.gameObject.SetActive(false);
        fireManager.StartFireSimulation();
        skyboxController.ChangeSky(true);
        uiManager.ToggleEarnMoreMoneyButton(false);
    }

    // player 1 and player 2 finished upgrade
    public void EndRound()
    {
        OnRoundEnd?.Invoke();

        cameraController.ResetCamera();
        currentPlayer.ToggleAllPurchaseIcons(false);
        DecideNextEvent();
        p1.nameText.SetActive(false);
        p2.nameText.SetActive(false);
        currentRoundCount++;
        if (currentRoundCount > maxRounds)
        {
            Debug.Log("Game Ends");
            CurrentStage = GameStage.GameEnd;
            return;
        }

    }

    // fire or competition
    void DecideNextEvent()
    {
        if (mustForceCompetition)
        {
            Debug.Log("Force Competition After Fire");
            consecutiveCompetitionCount++;
            mustForceCompetition = false;
            CurrentStage = GameStage.Competition;
            return;
        }

        if(consecutiveCompetitionCount >= 2)
        {
            Debug.Log("Force Fire After Two Consecutive Competitions");
            mustForceCompetition = true;
            consecutiveCompetitionCount = 0;
            CurrentStage = GameStage.Fire;
            return;
        }

        float roll = UnityEngine.Random.value;
        if (roll < fireChance)
        {
            Debug.Log("Random Roll -> Fire.");
            mustForceCompetition = true;
            consecutiveCompetitionCount = 0;
            CurrentStage = GameStage.Fire;
        }
        else
        {
            Debug.Log("Random Roll -> Competition.");
            consecutiveCompetitionCount++;
            CurrentStage = GameStage.Competition;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("FiresafeFriendScene");
    }

    public void NextRound()
    {
        CurrentStage = GameStage.BeforeGame;
        p1.InitHouseManager();
        p2.InitHouseManager();
    }
    public void ChangeGameMode(bool isPlantMode)
    {
        uiManager.HidePlantsMenu();
        IsPlantMode = isPlantMode;
        if(isPlantMode)
        {
            var zoomPos = currentPlayer.playerTag == "P1" ? h1PlantCamPos : h2PlantCamPos;
            cameraController.Zoomcamera(zoomPos, true,60);
            Debug.Log($"Zoom Position: {zoomPos}");
            inputManager.canClickHouse = false;
            //hide ui

            if (currentPlayer)
            {
                currentPlayer.ToggleAllPurchaseIcons(false);
                currentPlayer.ToggleHousePartClickable(false);
            }

            uiManager.HideStoreScreen();
            uiManager.ToggleInventory(false);
            uiManager.HidePurchasePopup(null);

        }
        else
        {

            if (isTutorial)
            {
                cameraController.MoveToHouse(currentPlayer);
                return;
            }
            Debug.Log("House Mode");
            inputManager.canClickHouse = true;
            if (IsGameStarted)
            {
                if (currentPlayer)
                {
                    currentPlayer.ToggleAllPurchaseIcons(true);
                    currentPlayer.ToggleHousePartClickable(true);
                }
                cameraController.MoveToHouse(currentPlayer);
            }
            else
            {
                cameraController.ResetCamera();
            }
            TogglePublicFenceClickable(true);
        }
    }


    void TogglePublicFenceClickable(bool state)
    {
        foreach(var fence in fences)
        {
            fence.GetComponent<BaseHousePartObject>().isClickable = state;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnStageChanged(GameStage newStage)
    {
        switch (newStage)
        {
            case GameStage.BeforeGame:
                // Initialize the game
                Debug.Log("Game is starting...");
                uiManager.endScreenManager.HideEndScreens();
                p1.OnHouseDeselected();
                p1.nameText.SetActive(true);
                p2.OnHouseDeselected();
                p2.nameText.SetActive(true);
                cameraController.ResetCamera();
                uiManager.floatingIcons.gameObject.SetActive(true);
                break;
            case GameStage.RoundStart:
                OnRoundStart?.Invoke();
                inputManager.canClickHouse = false;
                break;

            case GameStage.Fire:
                StartFire();
                break;

            case GameStage.Competition:
                OnCompetition();
                break;

            case GameStage.RoundEnd:
                EndRound();
                break;

            case GameStage.GameEnd:
                RestartGame();
                break;
        }
    }
}
