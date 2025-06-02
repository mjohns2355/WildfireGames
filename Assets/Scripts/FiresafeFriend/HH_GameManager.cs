using DG.Tweening;
using HappyHouse.FireSystem;
using HappyHouse.HouseSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public bool isTutorial, isNewLevel;
    public AudioSource audioSource;
    public HappyHouse.FireSystem.FireManager fireManager;
    public Transform h1, h2, h1CamPos,h2CamPos,h1PlantCamPos,h2PlantCamPos;
    public Action OnRoundStart, OnRoundEnd;
    public Action<bool> OnPlantModeChanged;
    public HH_UIManager uiManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;
    public HH_CameraController cameraController;
    public QuizManager quizManager;
    public FF_skybox skyboxController;
    public bool IsPaused { get; private set; }
    public bool IsGameStarted { get => _currentStage != GameStage.BeforeGame; }
    public bool IsFireStarted {  get => _currentStage == GameStage.Fire; }
    public List<BaseHousePartObject> publicFences;
    public HouseManager p1;
    public HouseManager p2;
    public FF_Tree tree1, tree2;
    public List<FF_Plants> p1Bushes, p2Bushes = new();
    public List<FF_Props> p1Props, p2Props = new();
    public List<FF_DirtMound> p1Mounds, p2Mounds;
    //public Transform BushesAndPropsContainer;
    [SerializeField] private bool _isPlantMode;
    [SerializeField] private const int WinReward = 3000;
    [SerializeField] private const int TieReward = WinReward/2;
    [SerializeField] GameObject publicFencePrefab;
    [SerializeField] List<Transform> publicFencesTransforms = new();
    private bool mustForceCompetition = true;
    private bool lastRoundIsFire,lastRoundIsCompetition = false;
    private string competitionLoser = string.Empty;
    private bool publicFencesRepaired = false;
    private int _housesMadeDecisionsCount;
    public int HousesMadeDecisionsCount
    {
        get => _housesMadeDecisionsCount;
        private set
        {
            _housesMadeDecisionsCount = value;
            if (_housesMadeDecisionsCount == 2)
            {
                uiManager.endRoundBtn.gameObject.SetActive(true);
            }
                
        }
    }
    List<GameObject> houses = new();
    List<GameObject> currentHousePrefabs = new(){ null, null};
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
        audioSource = GetComponent<AudioSource>();

        base.Awake();
    }
    private void Start()
    {
        Time.timeScale = 1f;
        CurrentStage = GameStage.BeforeGame;
        _isPlantMode = false;
        houses = ResourceManager.Instance.houses;
        isNewLevel = true;
        // don't spawn house at tutorial
        if (isTutorial) return;
        SpawnHouses();
        SpawnPublicFences();
        InitTrees();
        fireManager.fireEndEvent.AddListener(OnFireEnd);
        DOTween.Init();
    }

    private void Update()
    {
#if UNITY_EDITOR
        //debug
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 3.0f;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            RepairHouse();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale = 3.0f;
            StartFire();
        }
#endif
    }
    public void PauseGame()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;            
        AudioListener.pause = true;    
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
    private void InitPublicFences(HouseManager currentPlayer)
    {
        //fences = GameObject.FindGameObjectsWithTag("Fence");
        foreach (var fence in publicFences)
        {
            var info = Instantiate(fence.partInfo, transform);
            info.isPublic = true;
            fence.InitHousePartObject(currentPlayer,info);
            currentPlayer.inventory.AddNewPartToInventory(fence.partInfo);
        }
    }

    private void SpawnPublicFences()
    {
        publicFences.Clear();
        for (int i = 0; i<publicFencesTransforms.Count;i++)
        {
            var t = publicFencesTransforms[i];
            // Destroy old models
            for(int j = 0; j<t.childCount; j++)
            {
                Destroy(t.GetChild(j).gameObject);
            }
            var obj = Instantiate(publicFencePrefab, t);
            obj.tag = "Fence";
            var fence = obj.GetComponent<BaseHousePartObject>();
                    
            if (i == 1) fence.shouldDisplayBubble = true;
            publicFences.Add(fence);
        }
    }
    private void InitTrees()
    {
        var rng = UnityEngine.Random.Range(0, 1f);
        tree1.gameObject.SetActive(rng < 0.5f);
        tree2.gameObject.SetActive(rng >= 0.5f);
    }
    public void SpawnHouses()
    {
        if (houses.Count < 2)
        {
            Debug.LogError("Not enough houses to assign different ones to P1 and P2.");
            return;
        }

        p1 = SpawnSingleHouse("P1",h1);    


        p2 = SpawnSingleHouse("P2", h2);
    }

    private HouseManager SpawnSingleHouse(string playerTag, Transform parent,bool reRoll = true)
    {
        GameObject housePrefab;
        if (reRoll)
        {
            int index = UnityEngine.Random.Range(0, houses.Count);
            housePrefab = houses[index];
            var i = playerTag == "P1" ? 0 : 1;
            currentHousePrefabs[i] = housePrefab;
            houses.RemoveAt(index);
        }
        else
        {
            housePrefab = currentHousePrefabs[playerTag == "P1" ? 0 : 1];
        }

        var instance = Instantiate(housePrefab, parent);
        var house = instance.GetComponent<HouseManager>();
        house.playerTag = playerTag;
        house.arrowUI = playerTag == "P1" ? uiManager.rightArrow.gameObject : uiManager.leftArrow.gameObject;
        if(playerTag == "P1")
        {
            // flip the model 
            instance.transform.localScale = new Vector3(instance.transform.localScale.x * -1, 1, 1);
            house.nameText.transform.localScale = new Vector3(house.nameText.transform.localScale.x * -1, 1, 1);
        }

        var dirtMounds = playerTag == "P1" ? p1Mounds : p2Mounds;
        //var props = playerTag == "P1" ? p1Props : p2Props;
        //var bushes = playerTag == "P1" ? p1Bushes : p2Bushes;
        //Debug.Log($"PROPS: {props.Count}");
        foreach (var mound in dirtMounds)
        {
            mound.owner = house;
        }
        //house.props = props;
        //house.deadBushes = bushes;
        return house;
    }

    private void RemoveHouse(string playerTag, bool reRoll)
    {
        var houseToRemove = playerTag == "P1" ? p1 : p2;
        inputManager.OnHouseSelected -= houseToRemove.OnHouseSelected;
        currentPlayer = null;
        if (reRoll)
        {
            var index = playerTag == "P1" ? 0 : 1;
            houses.Add(currentHousePrefabs[index]);
        }
        Destroy(houseToRemove.gameObject);
    }
    public void RepairHouse()
    {
        //dynamic repair cost
        HousesMadeDecisionsCount++;
        var repairCost = currentPlayer.GetRepairCost();
        if (currentPlayer.budgetManager.SpendBudget(repairCost))
        {
            RespawnHouse(false);
            return;
        }
        uiManager.ShowPurchasePopup(null,true);

    }

    public void MoveHouse()
    {
        HousesMadeDecisionsCount++;
        RespawnHouse(true);
    }
    public void RespawnHouse(bool reRoll)
    {
        var temp = currentPlayer;
        var upgradeDict = temp.upgradeClassDictionary;
        var currentBudget = temp.budgetManager.currentBudget;
        var canEarnMoreMoney = temp.budgetManager.canEarnMoreMoney;
        var ownedPlants = temp.ownedPlants;
        var ownedBushes = currentPlayer.playerTag == "P1" ? p1Bushes : p2Bushes;
        var ownedProps = currentPlayer.playerTag == "P1" ? p1Props: p2Props;
        RemoveHouse(temp.playerTag,reRoll);
        var newHouse = SpawnSingleHouse(temp.playerTag, temp.transform.parent, reRoll);
        if(!publicFencesRepaired) SpawnPublicFences();
        publicFencesRepaired = true;
        currentPlayer = newHouse;
        if (reRoll)
        {
            newHouse.isMoving = true;
            foreach (var plant in ownedPlants)
            {
                Destroy(plant.gameObject);
            }

            foreach (var bush in ownedBushes)
            {
                bush.gameObject.SetActive(true);
                bush.ResetCombustible();
            }
            //foreach (var prop in ownedProps)
            //{
            //    prop.gameObject.SetActive(true);
            //    prop.ResetCombustible();
            //}
        }
        else
        {
            foreach(var plant in ownedPlants)
            {
                plant.gameObject.SetActive(true);
                plant.ResetCombustible();
            }
        }
        newHouse.nameText.SetActive(false);
        DOVirtual.DelayedCall(0.3f, () =>
        {
            // repair should keep the current material and budget
            // but move should reset everything
            var newPlayerTag = newHouse.playerTag;
            newHouse.Repair(upgradeDict, reRoll);
            if (!reRoll)
            {
                newHouse.budgetManager.currentBudget = currentBudget;
                newHouse.budgetManager.canEarnMoreMoney = canEarnMoreMoney;
                newHouse.ownedPlants = ownedPlants;
                //newHouse.deadBushes = ownedBushes;
                //newHouse.props = ownedProps;
            }

            if (newHouse.playerTag == "P1")
            {
                p1 = newHouse;
            }
            else
            {
                p2 = newHouse;
            }
            newHouse.ToggleClickBox(false);
            InitPublicFences(currentPlayer);
            
        });
    }
    void OnFireEnd()
    {
        fireManager.startFire = false;
        var fires = FindObjectsOfType<FF_FireController>();
        var combustibles = FindObjectsOfType<FF_BaseCombustible>();
        skyboxController.ChangeSky(false);
        foreach (var c in combustibles)
        {
            c.isOnFire = false;
            c.heat = 50;
            c.isOverHeated = false;
            c.StopAllCoroutines();
        }
        foreach (var f in fires)
        {
            Destroy(f.gameObject);
        }
        uiManager.ShowEndScreen(true,p1.GetBurnedPercent(),p2.GetBurnedPercent());
        publicFencesRepaired = false;
    }

    void OnCompetition()
    {
        HousesMadeDecisionsCount = 0;
        IsPlantMode = false;
        var p1Score = p1.CalculateRating();
        var p2Score = p2.CalculateRating();
        competitionLoser = p1Score > p2Score ? p2.playerTag
                           : p1Score < p2Score ? p1.playerTag
                           : string.Empty;
        int rewardP1 = p1Score > p2Score ? WinReward
              : p1Score < p2Score ? 0
              : TieReward;

        int rewardP2 = p2Score > p1Score ? WinReward
                      : p2Score < p1Score ? 0
                      : TieReward;

        uiManager.OnCompetitionResultEnabled = null;

        uiManager.OnCompetitionResultEnabled += () =>
        {
            p1.budgetManager.IncreaseBudget(rewardP1);
            p2.budgetManager.IncreaseBudget(rewardP2);
        };

        uiManager.ShowEndScreen(isFire: false,p1Score ,p2Score);

    }

    public void EarnReward()
    {
        currentPlayer.budgetManager.IncreaseBudget(WinReward);
    }
    public void SwitchPlayer (string playerTag)
    {
        uiManager.ToggleInventory(true);
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
    }

    // after player tap either house
    public void StartRound(HouseManager currentPlayer)
    {
        CurrentStage = GameStage.RoundStart;
        this.currentPlayer = currentPlayer;
        if (isTutorial) return;
        if (lastRoundIsCompetition)
        {
            Debug.Log("Loser is " + competitionLoser);
            var isLoser = competitionLoser != string.Empty && competitionLoser == currentPlayer.playerTag;
            uiManager.joinConcilPopup.SetActive(isLoser);
        }
        
        if (!lastRoundIsFire)
        {
            InitPublicFences(currentPlayer);
        }
        if (lastRoundIsFire && !currentPlayer.hasMadeDecisions)
        {
            uiManager.ShowAftermathScreen();
            uiManager.endRoundBtn.gameObject.SetActive(false);
        }
        
    }

    public void StartFire()
    {
        //IsFireStarted = true;
        IsPlantMode = false;
        CurrentStage = GameStage.Fire;
        uiManager.ShowFireAnnouncement();
        uiManager.floatingIcons.gameObject.SetActive(false);
        fireManager.StartFireSimulation();
        skyboxController.ChangeSky(true);
        uiManager.ToggleEarnMoreMoneyButton(false);
        p1.CalculateTotalHousePartWeight();
        p2.CalculateTotalHousePartWeight();
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
        p1.burnedPercent = 0f;
        p2.burnedPercent = 0f;
        p1.hasMadeDecisions = p2.hasMadeDecisions = false;

    }

    // fire or competition
    void DecideNextEvent()
    {
        if (mustForceCompetition || lastRoundIsFire)
        {
            //Debug.Log("Force Competition After Fire");
            lastRoundIsCompetition = true;
            lastRoundIsFire = false;
            mustForceCompetition = false;
            CurrentStage = GameStage.Competition;
            return;
        }

        if (lastRoundIsCompetition)
        {
            //Debug.Log("Random Roll -> Fire.");
            lastRoundIsFire = true;
            lastRoundIsCompetition = false;
            mustForceCompetition = true;
            CurrentStage = GameStage.Fire;
        }
    }
    
    public void RestartGame()
    {
        ResumeGame();
        SceneManager.LoadScene("FiresafeFriendScene");
    }

    public void NextRound()
    {
        CurrentStage = GameStage.BeforeGame;
        if (!lastRoundIsFire) return;
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
            //Debug.Log($"Zoom Position: {zoomPos}");
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
            //inputManager.canClickHouse = true;
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
            uiManager.ToggleInventory(true);
        }
    }


    void TogglePublicFenceClickable(bool state)
    {
        foreach(var fence in publicFences)
        {
            fence.GetComponent<BaseHousePartObject>().isClickable = state;
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnStageChanged(GameStage newStage)
    {
        switch (newStage)
        {
            case GameStage.BeforeGame:
                // Initialize the game
                Debug.Log("Game is starting...");
                inputManager.canClickHouse = true;
               
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
                isNewLevel = false;
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
