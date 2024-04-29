using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HouseStructure : Structure
{
    public int pplNum;
    public int petNum;
    public int carNum;
    public bool hasElder;
    public bool hasPet;
    private void Awake()
    {

        InitializeInfoDictionary();
    }

    public override void OnStructureClick()
    {
        base.OnStructureClick();

    }

    public void AfterSpawnACar()
    {
        if (carNum <= 0) return;
        carNum--;
        structureInfoDict["Car(s)"] = carNum;
    }

    public bool CanSpawnCar()
    {
        return carNum > 0;
    }

    void InitializeInfoDictionary()
    {
        structureInfoDict.Add("People", pplNum);
        structureInfoDict.Add("Car(s)", carNum);
        structureInfoDict.Add("Pet(s)", petNum);
    }
}
