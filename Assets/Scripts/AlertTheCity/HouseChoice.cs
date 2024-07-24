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
    [TextArea(5, 10)]
    public string choiceDetail;
    public bool isLocked = false;   
    public HouseChoice(string choiceName)
    {
        this.choiceName = choiceName;
    }

    void ApplySpecialEffect(HouseStructure owner)
    {
        switch (choiceName)
        {
            case "Relocate 2nd Car":

                break;
            case "Relocate Horses":
                break ;
            case "Home Hardening":

                Debug.Log("Apply Home Hardening Effect");
                var combustible = owner.gameObject.GetComponent<Combustible>();
                combustible.fireChance = 1 - homeHardeningMod;
                Debug.Log(combustible.fireChance);
                break ;
        }
    }
    public void ApplyEffect(HouseStructure owner)
    {
        ApplySpecialEffect(owner);
        //CheckChocieBehavior(owner.currentOption);
        if (carSpeedMod != CarSpeed.none)
        {
            owner.carSpeed = carSpeedMod;
        }


        owner.spawnTime += carSpawnTimeMod;
        Mathf.Clamp(owner.spawnTime, 0, Mathf.Infinity);
        owner.carNum += carNumberMod;
        Mathf.Clamp(owner.carNum, 0, 10);
        owner.kidNum += kidNumberMod;
        owner.horseNum += horseNumberMod;
        //isLocked = false;
        //owner.
    }

}
