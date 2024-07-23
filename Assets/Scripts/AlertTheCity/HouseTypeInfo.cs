using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HouseTypeInfo", menuName = "HouseTypeInfo")]
public class HouseTypeInfo : ScriptableObject
{
    public HouseType houseType;
    public string menuTitle;
    public List<HouseChoice> normalChoices = new List<HouseChoice>();
    public List<HouseChoice> lockedChoices = new List<HouseChoice>();
    public string longerTitle;
    [TextArea(15, 20)]
    public string description;
    //[TextArea(5, 10)]
    //public string lockedOptionDetail;
    // house related properties
    public int carNumber = 1;
    public int horseNumber = 0;
    public int petNumber = 0;
    public int kidNumber = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    public float carSpawnTime = 1f;
    HouseStructure owner;

    public void InitHouseInfo(HouseStructure house)
    {
        owner = house;
        owner.carNum = carNumber;
        owner.horseNum = horseNumber;
        owner.petNum = petNumber;
        owner.carSpeed = carSpeed;
        owner.kidNum = kidNumber;
        owner.spawnTime = carSpawnTime;

        foreach(var choice in lockedChoices)
        {
            choice.isLocked = true;
        }
    }

    public HouseChoice ReturnChoiceByName(string name, bool searchLockedChoices = false)
    {
        if(!searchLockedChoices) {

            foreach (var choice in normalChoices)
            {
                if (choice.choiceName == name)
                {
                    return choice;
                }
            }
        }


        foreach(var choice in lockedChoices)
        {
            if(choice.choiceName == name)
            {
                return choice;
            }
        }

        return null;
    }

}
