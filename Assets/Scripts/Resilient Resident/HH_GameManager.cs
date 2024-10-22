using HappyHouse.HouseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HH_GameManager : UnitySingleton<HH_GameManager>
{
    public HappyHouse.FireSystem.FireManager fireManager;
    public HH_UIManager UIManager;
    public HouseManager currentPlayer;
}
