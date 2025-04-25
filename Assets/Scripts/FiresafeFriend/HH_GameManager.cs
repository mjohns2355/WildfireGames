using HappyHouse.FireSystem;
using HappyHouse.HouseSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public bool isTutorial;
    public HappyHouse.FireSystem.FireManager fireManager;
    public Transform h1, h2, h1CamPos,h2CamPos,h1PlantCamPos,h2PlantCamPos;
    public float fireTimer = 60f;
    public float fireChance = 0.5f; // 50% chance to start fire
    public Action OnRoundStart, OnRoundEnd;
    public Action<bool> OnPlantModeChanged;
    public HH_UIManager uiManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;
    public HH_CameraController cameraController;
    public QuizManager quizManager;
    public FF_skybox skyboxController;

    public bool IsGameStarted {  get; private set; }
    public bool IsFireStarted {  get; private set; }
    private GameObject[] fences;
    public List<BaseHousePartObject> publicFences;
    public HouseManager p1;
    public HouseManager p2;

    [SerializeField]private bool _isPlantMode;
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
    public override void Awake()
    {
        shouldNotDestroyOnLoad = false;

        base.Awake();
        fences = GameObject.FindGameObjectsWithTag("Fence");
    }
    private void Start()
    {

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
        foreach(var f in fences)
        {
            var fence = f.GetComponent<BaseHousePartObject>();
            fence.InitHousePartObject(currentPlayer);
            var info = Instantiate(fence.partInfo,transform);
            info.isPublic = true;
            fence.partInfo = info;
            publicFences.Add(fence);
            currentPlayer.inventory.AddNewPartToInventory(info);
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
        IsFireStarted = false;
        var fires = FindObjectsOfType<FF_FireController>();
        var combustibles = FindObjectsOfType<FF_BaseCombustible>();
        
        foreach (var c in combustibles)
        {
            c.isOnFire = false;
            c.StopAllCoroutines();
        }
        foreach (var f in fires)
        {
            Destroy(f.gameObject);
        }
        uiManager.ShowEndScreen();
    }
    public void SwitchPlayer (string playerTag)
    {
        //if (isTutorial)
        //{
        //    currentPlayer = p2;
        //    IsPlantMode = false;
        //    cameraController.Zoomcamera(h2CamPos, true, 60);
        //    return;
        //}

        currentPlayer.OnHouseDeselected();
        List<HousePartInfo> ownedPublicFences = new List<HousePartInfo>();
        if (currentPlayer != null && currentPlayer.inventory.ownedPublicParts[HousePartType.Fence] != null)
        {
            ownedPublicFences = currentPlayer.inventory.ownedPublicParts[HousePartType.Fence];
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
        SetRoundStart(true);
        inputManager.canClickHouse = false;



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
        IsFireStarted = true;
        uiManager.floatingIcons.gameObject.SetActive(false);
        fireManager.StartFireSimulation();
        skyboxController.ChangeSky();
        uiManager.ToggleEarnMoreMoneyButton(false);
    }

    // player 1 and player 2 finished upgrade
    public void EndRound()
    {
        SetRoundStart(false);
        cameraController.ResetCamera();
        currentPlayer.ToggleAllPurchaseIcons(false);
        //uiManager.OnRoundEnd();
        var rng = UnityEngine.Random.Range(0, 1f);
        if (rng < fireChance)
        {
            StartFire();
        }
        else
        {
            p1.CalculateRating();
            p2.CalculateRating();
        }
        //startFireBtn.gameObject.SetActive(true);

        p1.nameText.SetActive(false);
        p2.nameText.SetActive(false);
    }

    public void ToggleHousesClickBox(bool toggle)
    {
        //p1.ToggleClickBox(toggle);
        //p2.ToggleClickBox(toggle);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("FiresafeFriendScene");
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

    void SetRoundStart(bool state)
    {
        IsGameStarted = state;

        if(state)
        {
            OnRoundStart?.Invoke();
        }
        else
        {
            OnRoundEnd?.Invoke();
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
}
