using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public HappyHouse.FireSystem.FireManager fireManager;
    public HH_UIManager uiManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;

    public bool IsGameStarted {  get; private set; }
    [SerializeField] HouseManager p1;
    [SerializeField] HouseManager p2;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("HappyHouseScene");
        }
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
        currentPlayer.OnHouseSelected(currentPlayer);
    }

    public void StartGame(HouseManager currentPlayer)
    {
        IsGameStarted = true;
        inputManager.canClickHouse = false;
        this.currentPlayer = currentPlayer;
        uiManager.ToggleInventory(true);

    }
    public BaseHousePartObject CreateHousePartObject(HousePartInfo partInfo, HouseManager owner)
    {
        var obj = new GameObject(partInfo.partID);
        var houseObj = obj.AddComponent<BaseHousePartObject>();
        houseObj.InitHousePartObject(partInfo, owner);
        return houseObj;
    }
}
