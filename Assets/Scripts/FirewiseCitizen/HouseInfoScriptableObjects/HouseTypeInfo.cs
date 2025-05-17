using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "HouseTypeInfo", menuName = "HouseTypeInfo")]
public class HouseTypeInfo : ScriptableObject
{
    public HouseType houseType;
    public string menuTitle;
    [SerializeField] List<HouseChoice> choices = new List<HouseChoice>();
    //public List<HouseChoice> lockedChoices = new List<HouseChoice>();
    public Dictionary<string, (HouseChoice choice, int index)> houseChoicesDict;
    public string longerTitle;
    [TextArea(15, 20)]
    //public string description;
    public string[] descriptions;
    //[TextArea(5, 10)]
    //public string lockedOptionDetail;
    // house related properties
    public int carNumber = 1;
    public int horseNumber = 0;
    public int petNumber = 0;
    public int kidNumber = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    public HouseChoice defaultChoice;
    public bool allowMultipleChoices;
    public int requiredChoicesCount;
    public Sprite newsUISprite;
    public Sprite choicePicture;
    public Sprite houseIcon;
    public float homeHardeningChance = 0.4f;
    public List<string> incentiveOptions;
public int allChoicesCount => choices.Count;
    //public float carSpawnTime = 1f;
    HouseStructure owner;

    public void InitHouseInfo(HouseStructure house)
    {
        owner = house;
        owner.carNum = carNumber;
        owner.horseNum = horseNumber;
        owner.petNum = petNumber;
        owner.carSpeed = carSpeed;
        owner.kidNum = kidNumber;
        owner.homeHardeningChance = homeHardeningChance;
        //owner.carSpawnWaitTime = carSpawnTime;

        //foreach(var choice in lockedChoices)
        //{
        //    choice.isLocked = true;
        //}
        int currentIndex = 0;
        houseChoicesDict = choices
                                .Select(choice => new
                                {
                                    choice,
                                    index = choice.isNormal ? -1 : currentIndex++ // Skip normal choices (set index to -1) and increment for non-normal
                                })
                        .ToDictionary(x => x.choice.choiceName, x => (x.choice, x.index));
        defaultChoice = choices[0];
    }

    public (HouseChoice choice, int index) ReturnChoiceByName(string name, bool searchLockedChoices = false)
    {
        houseChoicesDict.TryGetValue(name, out var choice);
        return choice;
    }

    public bool AllChoicesAreUnlocked()
    {
        var lockedChoicesCount = houseChoicesDict.Where(x => x.Value.choice.isLocked == true).Count();
        //Debug.Log("Check " + houseType + " 's locked choices count "+  lockedChoicesCount);
        return lockedChoicesCount == 0;
    }

}
