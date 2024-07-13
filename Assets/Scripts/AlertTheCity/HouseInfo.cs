using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseInfo
{
    HouseType houseType;
    public string menuTitle = string.Empty;
    public List<string> normalOptions = new List<string>(); 
    public Dictionary<string,string> lockedOptions = new Dictionary<string, string> ();
    public string longerTitle = string.Empty;
    public string description = string.Empty;
    public int carNumber = 1;
    public int horseNumber = 0;
    public int petNumber = 0;
    public int kidNumber = 0;
    public CarSpeed carSpeed = CarSpeed.medium;
    // Start is called before the first frame update
    

    public HouseInfo(HouseType type)
    {
        InitHouseInfo(type);
    }

    public void InitHouseInfo(HouseType type)
    {
        houseType = type;
        char[] delimiters = { ':', '|' };
        string[] menuInfo = GameManager.Instance.ParseString(ReturnHouseInfoForType(), delimiters);
        menuTitle = menuInfo[0];
        for (int i = 1; i < menuInfo.Length; i++)
        {
            if (menuInfo[i].Contains("(locked)"))
            {
                var text = menuInfo[i].Replace("(locked)", "");
                lockedOptions[text] = ReturnLockedOptionsDetails();
            }
            else
            {
                normalOptions.Add(menuInfo[i]);
            }
        }
        string[] LearnMoreInfo = GameManager.Instance.ParseString(ReturnLearnMoreInfoForType(), delimiters);
        longerTitle = LearnMoreInfo[0];
        description = LearnMoreInfo[1];

    }


     string ReturnHouseInfoForType()
    {
        var houseInfo = "";
        switch (houseType)
        {

            case HouseType.elderly:
                carSpeed = CarSpeed.slow;
                houseInfo = "Elderly Resident: Wait for Notice | (locked) Evacuate Early | (locked) Help from Neighbor ";
                break;
            case HouseType.twoCar:
                carNumber = 2;
                houseInfo = "Two-Car House: Take Both cars | Leave One Car | (locked) Relocate 2nd Car ";
                break;
            case HouseType.horse:
                horseNumber = 1;
                carSpeed = CarSpeed.slow;
                houseInfo = "Horse Owner: Wait for Notice | Leave the Horses | (locked) Relocate Horses ";
                break;
            case HouseType.pet:
                houseInfo = "Pets: Wait for Notice | (locked) Plan Ahead | (locked) Evacuate Early" ;
                break;
            case HouseType.wui:
                houseInfo = "WUI House: Wait for Notice | (locked) Evacuate Early | (locked) Home Hardening ";
                break;
            case HouseType.kids:
                houseInfo = "School-Age Children: Wait for Notice | Pick up Children | (locked) Safer at School ";
                break;
        }
        return houseInfo;
    }

    string ReturnLearnMoreInfoForType()
    {
        var learnMore = "";
        switch (houseType)
        {
            case HouseType.none:
                break;
            case HouseType.elderly:
                learnMore = "Elderly Resident: Elderly residents often need additional time and\r\nsupport to evacuate safely. It is very important \r\nto make plans in advance and consider multiple\r\nscenarios and consequences of choices. \r\n\r\nSome adult children and grand children choose to\r\ndrive a great distance to pick up their elderly relative,\r\nbut this can cause delays and additional congestion.\r\n\r\nConsider making a plan with a neighbor who can \r\nhelp them evacuate, or arranging to evacuate them\r\nearly before there is an evacuation notice";
                break;
            case HouseType.twoCar:
                learnMore = "Multi-Car Household: Households with multiple cars face a dilemma during\r\nevacuations. Some wish to keep all their vehicles with\r\nthem when they evacuate. It is very important \r\nto consider the consequences of such choices. \r\n\r\nSplitting the family into multiple vehicles during an\r\nevacuation increases risks of being separated, as well\r\nas causing additional congestion during evacuation.\r\n\r\nIf it is important to keep both vehicles, consider \r\nrelocating one of them to a safe place on red flag days\r\nto keep yourselves and your belongings safe.";
                break;
            case HouseType.kids:
                learnMore = "School-Age Children: Families with children have additional responsibilities\r\nto juggle during an evacuation. It is very important \r\nto make plans in advance and consider multiple\r\nscenarios and consequences of choices. \r\n\r\nSome parents choose to pick up their children from\r\nschool when the evacuation notice goes out. \r\nHowever, this causes additional congestion around\r\nthe school. \r\n\r\nSchools tend to be extremely safe places to shelter\r\nduring an emergency. Parents should consider \r\nleaving the children at the school and waiting until it\r\nis safe to pick them up.";
                break;
            case HouseType.horse:
                learnMore = "Large Animals: Dealing with large animals, such as horses, during \r\nan evacuation can be difficult. It is very important \r\nto make plans in advance and consider multiple\r\nscenarios and consequences of choices. \r\n\r\nChoosing to take the animals during the \r\nevacuation notice causes additional congestion on\r\nthe roads, but for many people leaving their animals\r\nbehind is not an option. \r\n\r\nConsider relocating the animals to a defensible \r\nlocation during risky conditions to increase their\r\nsafety as well as the safety of everyone else who\r\nneeds to evacuate. ";
                break;
            case HouseType.pet:
                learnMore = "Pets: Having pets can increase the time needed to\r\nevacuate safely. It is very important to make plans in \r\nadvance and consider multiple scenarios and \r\nconsequences of choices. \r\n\r\nBe sure you have a carrier or leash readily available \r\nand that the pets are in an easily accessible space\r\non red flag days (ie. keep them inside and in a room\r\nwhere you can find/catch them easily).\r\n\r\nIt may be a good idea to evacuate early and have\r\na plan to stay in a pet-friendly shelter or with friends.";
                break;
            case HouseType.wui:
                learnMore = "WUI: Living at the Wildland Urban Interface comes with\r\nrisks and challenges. It is very important to take\r\naction early and to consider multiple scenarios and \r\nconsequences of choices. \r\n\r\nLong before fire season, consider hardening your \r\nhome and creating a defensible space. Be sure you\r\nhave a Go Bag and a plan for your belongings.\r\n\r\nConsider evacuating early before an evacuation \r\nnotice to protect yourself and reduce risk.";
                break;
            default:
                break;
        }

        return learnMore;
    }

    string ReturnLockedOptionsDetails()
    {
        var str = "Test";

        switch (houseType)
        {
            case HouseType.none:
                break;
            case HouseType.elderly:

                break;
            case HouseType.twoCar:

                str = "You know too many cars on the road cause traffic congestion, so you relocate your second car on high risk days.";
                break;

            case HouseType.kids:
                str = "You know the children are safer at school and have a plan that they know: they will stay at school until it is safe to pick them up.";
                break;
            case HouseType.horse:
                
                break;
            case HouseType.pet:
               
                break;
            case HouseType.wui:
                
                break;
            default:
                break;
        }
        return str;
    }
    
    
    
}
