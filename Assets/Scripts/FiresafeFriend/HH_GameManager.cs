using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public HappyHouse.FireSystem.FireManager fireManager;
    public Transform h1, h2;
    public HH_UIManager uiManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;
    public HH_CameraController cameraController;
    public QuizManager quizManager;
    public FF_skybox skyboxController;
    [SerializeField] Button startFireBtn, endRoundBtn;
    public bool IsGameStarted {  get; private set; }
    public GameObject[] fences;
    public HouseManager p1;
    public HouseManager p2;
    
    public override void Awake()
    {
        shouldNotDestroyOnLoad = false;
        base.Awake();
        fences = GameObject.FindGameObjectsWithTag("Fence");
    }
    private void Start()
    {
        fireManager.fireEndEvent.AddListener(() =>
        {
            ToggleHousesClickBox(true);
        });
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 5.0f;
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


}
