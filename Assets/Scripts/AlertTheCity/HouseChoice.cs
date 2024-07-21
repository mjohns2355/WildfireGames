using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HouseChoice
{
    public string choiceName;
    public int carNumberMod = 0;
    public float carSpawnTimeMod = 0;
    public int horseNumberMod = 0;
    public int kidNumberMod = 0;
    public int petNumberMod = 0;
    public float homeHardeningMod = 0;
    public CarSpeed carSpeedMod = CarSpeed.none;

    public HouseChoice(string choiceName)
    {
        this.choiceName = choiceName;
    }

    //public void CheckChocieBehavior(string choiceName)
    //{
    //    switch (choiceName)
    //    {
    //        case " Wait for Notice ":
    //            carSpawnTimeMod = 2;
    //            break;
    //        case " Leave One Car ":
    //            carNumberMod = -1;
    //            break;
    //        case " Take Both Car ":
    //            carNumberMod = 1;
    //            break;
    //    }
    //}

    public void ApplyEffect(HouseStructure owner)
    {
        //CheckChocieBehavior(owner.currentOption);
        if(carSpeedMod != CarSpeed.none)
        {
            owner.carSpeed = carSpeedMod;
        }
        owner.spawnTime += carSpawnTimeMod;
        owner.carNum += carNumberMod;
        owner.kidNum += kidNumberMod;
        owner.horseNum += horseNumberMod;
        //owner.
    }

}
