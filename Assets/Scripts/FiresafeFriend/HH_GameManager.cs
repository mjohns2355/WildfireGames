using HappyHouse.FireSystem;
using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public HappyHouse.FireSystem.FireManager fireManager;
    public Transform h1, h2, h1CamPos,h2CamPos,plantModeCamPos;
    public float fireTimer = 60f;
    public float plantModeCamFOV = 30f;
    public HH_UIManager uiManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;
    public HH_CameraController cameraController;
    public QuizManager quizManager;
    public FF_skybox skyboxController;
    [SerializeField] Button startFireBtn, endRoundBtn;
    public bool IsGameStarted {  get; private set; }
    public bool IsFireStarted {  get; private set; }
    private GameObject[] fences;
    public List<BaseHousePartObject> publicFences;
    public HouseManager p1;
    public HouseManager p2;
    public bool IsPlantMode { get; private set; }
    public override void Awake()
    {
        shouldNotDestroyOnLoad = false;
        IsPlantMode = false;
        base.Awake();
        fences = GameObject.FindGameObjectsWithTag("Fence");
    }
    private void Start()
    {


        
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
        if (Input.GetKeyDown(KeyCode.F1))
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
        var houses = ResourceManager.Instance.houses;
        
    }


    public void SwitchPlayer (string playerTag)
    {
        currentPlayer.OnHouseDeselected();
        uiManager.HideStoreScreen();
        
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
        IsGameStarted = true;
        inputManager.canClickHouse = false;
        this.currentPlayer = currentPlayer;
        uiManager.ToggleInventory(true);
        endRoundBtn.gameObject.SetActive(true);
        startFireBtn.gameObject.SetActive(false);
        uiManager.startText.SetActive(false) ;
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
        IsGameStarted = false;
        cameraController.ResetCamera();
        currentPlayer.ToggleAllPurchaseIcons(false);
        uiManager.OnRoundEnd();
        StartFire();
        //startFireBtn.gameObject.SetActive(true);
        endRoundBtn.gameObject.SetActive(false);
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
        IsPlantMode = isPlantMode;
        if(isPlantMode)
        {
            Debug.Log("Plant Mode");
            cameraController.Zoomcamera(plantModeCamPos,plantModeCamFOV);
            inputManager.canClickHouse = false;
            //hide ui

            if (currentPlayer)
            {
                currentPlayer.ToggleAllPurchaseIcons(false);
            }

            uiManager.HideStoreScreen();
            uiManager.ToggleInventory(false);
            uiManager.HidePurchasePopup(null);
        }
        else
        {
            Debug.Log("House Mode");
            inputManager.canClickHouse = true;
            if (IsGameStarted)
            {
                cameraController.MoveToHouse(currentPlayer);
            }
            else
            {
                cameraController.ResetCamera();
            }
        }
    }
}
