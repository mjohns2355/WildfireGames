using HappyHouse.FireSystem;
using HappyHouse.HouseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public bool isTutorial;
    public HappyHouse.FireSystem.FireManager fireManager;
    public Transform h1, h2, h1CamPos,h2CamPos,h1PlantCamPos,h2PlantCamPos;
    public float fireTimer = 60f;
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
            IsFireStarted = false;
            var fires = FindObjectsOfType<FireController>();
            foreach (var f in fires)
            {
                Destroy(f.gameObject);
            }
        }
        //debug
        //if (Input.GetKeyDown(KeyCode.F1)|| Input.GetKeyDown(KeyCode.Q))
        //{
        //    Time.timeScale = 5.0f;
        //}
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
    public void SpawnHouses(string playerTag)
    {
        //var houses = ResourceManager.Instance.houses;
        if (playerTag == "p1")
        {

        }

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
        uiManager.HideStoreScreen();
        uiManager.HidePlantsMenu();
        IsPlantMode = false;
        if (playerTag == "p1")
        {
            currentPlayer = p1;
        }
        else if(playerTag == "p2")
        {
            currentPlayer = p2;
        }
        else
        {
            Debug.Log($"player {playerTag} doesn't exist!");
            return;
        }

        Debug.Log($"Current Player is {currentPlayer.playerTag}");
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
        uiManager.earnMoreMoney.gameObject.SetActive(false);
    }

    public void EndRound()
    {
        SetRoundStart(false);
        cameraController.ResetCamera();
        currentPlayer.ToggleAllPurchaseIcons(false);
        //uiManager.OnRoundEnd();
        StartFire();
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
}
