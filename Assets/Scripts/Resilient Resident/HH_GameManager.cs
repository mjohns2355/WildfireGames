using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public HappyHouse.FireSystem.FireManager fireManager;
    public HH_UIManager UIManager;
    public HouseManager currentPlayer;
    public HH_InputManager inputManager;

    [SerializeField] HouseManager p1;
    [SerializeField] HouseManager p2;

    private void Start()
    {
        
    }
    public void SwitchPlayer (string playerTag)
    {
        currentPlayer.OnHouseDeselected();
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
}
