using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HouseChoice
{
    public string choiceName;
    public int carNumberMod = 0;
    public int horseNumberMod = 0;
    public int kidNumberMod = 0;
    public int petNumberMod = 0;
    public float homeHardeningMod = 0;
    public CarSpeed carSpeedMod = CarSpeed.none;
    public CarLeaveTime carLeaveTime = CarLeaveTime.normal;
    [TextArea(5, 10)]
    public string choiceDetail;
    [TextArea(5, 10)]
    public string choiceThought;
    public Sprite choiceThoughtSprite;
    [TextArea(5, 10)]
    public string endGameFeedback;
    public bool isLocked = false;
    public bool isNormal;
    public HouseChoice(string choiceName)
    {
        this.choiceName = choiceName;
    }

    float carSpawnTimeMod = 0;

    public void ApplySpecialEffect(HouseStructure owner)
    {
        //if(choiceName.Contains("Home Hardening"))
        //{
        //    owner.ApplyHomeHardeningToAllHouses(homeHardeningMod);
        //    return;

        //}
        switch (choiceName)
        {
            case "Relocate 2nd Car":
                Debug.Log("Relocated 2nd Car to Shelter");
                owner.RelocateSecondCar();
                break;

            case "Relocate the Horse":
                Debug.Log("Relocated Horse to Stable");
                owner.RelocateHorses();
                break ;
            case "Full Home Hardening":
                owner.ApplyHomeHardeningToAllHouses(homeHardeningMod);
                break;
        }       
    }
    public void ApplyEffect(HouseStructure owner)
    {

        //CheckChocieBehavior(owner.currentOption);
        if (carSpeedMod != CarSpeed.none)
        {
            owner.carSpeed = carSpeedMod;
        }
        GetSpawnTimeMod();
        //Debug.Log(carSpawnTimeMod);
        owner.carSpawnWaitTime += carSpawnTimeMod;
        Mathf.Clamp(owner.carSpawnWaitTime, 0, Mathf.Infinity);
        owner.carNum += carNumberMod;
        Mathf.Clamp(owner.carNum, 0, 10);
        owner.kidNum += kidNumberMod;
        owner.horseNum += horseNumberMod;
        //isLocked = false;
        //owner.
    }

    public void ApplyHomeHardening(HouseStructure owner)
    {
        Debug.Log("Applied Home Hardening Effect & Early Evacuate");
        owner.ApplyHomeHardeningToAllHouses(homeHardeningMod);
    }
    void GetSpawnTimeMod()
    {
        switch(carLeaveTime)
        {
            case CarLeaveTime.early:
                carSpawnTimeMod = -3f;
                break;
            case CarLeaveTime.normal:
                carSpawnTimeMod = 0f;
                break;
            case CarLeaveTime.delayed:
                carSpawnTimeMod = 3f;
                break;
        }

    }
}
