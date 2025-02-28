using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public GameObject housePrefab;
    public GameObject specialPrefab;
    public ATC_PlacementManager placementManager;
    public GameObject structureTilemap;
    [SerializeField] List<HouseTypeInfo> houseInfos = new List<HouseTypeInfo>();
    public List<ATC_StructureModel> allHouses = new List<ATC_StructureModel>();
    public Dictionary<HouseType, HouseStructure> allMainHouses = new Dictionary<HouseType, HouseStructure>();

    Dictionary<HouseType, List<HouseChoice>> playerChoices = new Dictionary<HouseType, List<HouseChoice>>();
    Dictionary<HouseType, HouseTypeInfo> houseInfoDict = new Dictionary<HouseType, HouseTypeInfo>();
    public Dictionary<HouseType, List<HouseStructure>> houseTypeDict;
    public Dictionary<StructureType, ATC_StructureModel> specialStructureDict;
    private void Start()
    {
        PlacePreBuiltStructures();
        InitHouseTypeDict();
        InitialHouseInfoDict();
        InitiPlayerChoiceDict();
        InitSpecialStructDict();
        InitialMainHouses();

    }

    void InitSpecialStructDict()
    {
        if (specialStructureDict == null)
        {
            specialStructureDict = new Dictionary<StructureType, ATC_StructureModel>();
            StructureType[] values = (StructureType[])Enum.GetValues(typeof(StructureType));
            foreach (var type in values)
            {
                // house is not a special structure
                if (type == StructureType.House) continue;
                specialStructureDict[type] = placementManager.GetRandomSpecialStructursOfType(type);
            }
        }
    }

    void InitHouseTypeDict()
    {
        if (houseTypeDict == null)
        {
            houseTypeDict = new Dictionary<HouseType, List<HouseStructure>>();

            foreach (var house in placementManager.GetAllHouses())
            {
                if (house == null) continue;

                var houseStructure = house.GetComponent<HouseStructure>();
                if (houseStructure == null) continue;

                if (!houseTypeDict.ContainsKey(houseStructure.houseType))
                {
                    houseTypeDict[houseStructure.houseType] = new List<HouseStructure>();
                }

                houseTypeDict[houseStructure.houseType].Add(houseStructure);
            }
        }


    }
    private void InitialHouseInfoDict()
    {
        houseInfoDict.Clear();

        foreach (var info in houseInfos)
        {
            houseInfoDict[info.houseType] = Instantiate(info);
        }
    }

    private void InitiPlayerChoiceDict()
    {
        foreach (var houseType in GameManager.Instance.availableHouseTypes)
        {
            playerChoices[houseType] = new List<HouseChoice>();
        }
    }

    private void OnEnable()
    {
        
    }

    
    public void InitialMainHouses()
    {

        //make sure each type has at least one house in the group
        foreach (var houseType in GameManager.Instance.availableHouseTypes)
        {
            if (allHouses.Count == 0) return;
            var structure = allHouses[UnityEngine.Random.Range(0, allHouses.Count-1)];
            if (allMainHouses.ContainsKey(houseType))
            {
                InitMainHouse(houseType, allMainHouses[houseType]);
                //allMainHouses[houseType].outline.enabled = true;
                continue;
            }
            var house = structure.GetComponent<HouseStructure>();
            var housePosition = Vector3Int.RoundToInt(structure.transform.position);
            if (house != null && !house.isMainHouse && !CloseToMainHouse(housePosition))
            {
                InitMainHouse(houseType, house);
            }
        }


    }

    private void InitMainHouse(HouseType houseType, HouseStructure house)
    {
        house.outline.enabled = true;
        house.isMainHouse = true;
        house.houseInfo = ReturnHouseInfoFor(houseType);
        house.houseInfo.InitHouseInfo(house);
        allMainHouses[houseType] = house;
        //playerChoices[houseType] = house.houseInfo.defaultChoice;
        playerChoices[houseType].Add(house.houseInfo.defaultChoice);
        house.SetHouseType(houseType);
        //allHouses.Remove(structure);
        // remove main house from same type house list
        houseTypeDict[houseType].Remove(house);
        house.InitMainHouse();
    }

    public void PlacePreBuiltStructures()
    {
        for (int i = 0; i < structureTilemap.transform.childCount; i++)
        {
            var structure = structureTilemap.transform.GetChild(i).GetComponent<Structure>();
            Vector3Int pos = Vector3Int.RoundToInt(structure.transform.position);
            PlacePreBuiltStructure(pos, structure);
            Destroy(structureTilemap.transform.GetChild(i).gameObject);
        }

        EvenlyRanomizeHouse();
        //// randomize house type at start
        //foreach (var structure in allHouses)
        //{
        //    var house = structure.GetComponent<HouseStructure>();
        //    // skip specified house
        //    if (house.HouseType != HouseType.none) continue;
        //    house.RandomizeHouseType();
        //}
    }

    void EvenlyRanomizeHouse()
    {
        if (allHouses.Count == 0)
        {
            allHouses = placementManager.GetAllHouses();
        }
        // Collect houses that need randomization
        List<HouseStructure> housesToRandomize = new List<HouseStructure>();
        foreach (var structure in allHouses)
        {
            var house = structure.GetComponent<HouseStructure>();
            if (house.isMainHouse && house.houseType != HouseType.none)
            {
                allMainHouses[house.houseType] = house;
            }
            else
            {
                housesToRandomize.Add(house);
            }
            
        }
        //Debug.Log("Houses to randomize: " + housesToRandomize.Count);
        // Get all house types except 'none'
        HouseType[] houseTypes = Enum.GetValues(typeof(HouseType))
                                     .Cast<HouseType>()
                                     .Where(type => type != HouseType.none)
                                     .ToArray();

        int totalHouses = housesToRandomize.Count;
        int numTypes = houseTypes.Length;

        // Calculate the base number of houses per type and the remainder
        int baseCount = totalHouses / numTypes;
        int remainder = totalHouses % numTypes;

        // Create the target distribution list
        List<HouseType> targetDistribution = new List<HouseType>();

        // Distribute the base count for each type
        foreach (var type in houseTypes)
        {
            for (int i = 0; i < baseCount; i++)
            {
                targetDistribution.Add(type);
            }
        }

        // Distribute the remainder randomly across types
        List<int> remainderIndices = Enumerable.Range(0, numTypes).OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < remainder; i++)
        {
            targetDistribution.Add(houseTypes[remainderIndices[i]]);
        }

        // Shuffle the list to randomize the order
        for (int i = targetDistribution.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (targetDistribution[i], targetDistribution[j]) = (targetDistribution[j], targetDistribution[i]);
        }

        // Assign the shuffled types to the houses
        for (int i = 0; i < housesToRandomize.Count; i++)
        {
            housesToRandomize[i].SetHouseType(targetDistribution[i]);
        }

    }
    public void ClickStructre(Vector3Int position)
    {
        var clickedStructure = placementManager.GetStructureAt(position);
        if (clickedStructure == null) return;
        var structure = clickedStructure.gameObject.GetComponentInChildren<Structure>();
        if(structure == null) return;
        structure.OnStructureClick();
    }

    public void PlacePreBuiltStructure(Vector3Int position, Structure structure)
    {
        structure.transform.localPosition = Vector3.zero;
        if (structure.IsBigStructure)
        {
            PlaceBigStructure(position, structure.gameObject, structure.width, structure.height);
        }
        else if (structure.structureType == StructureType.House)
        {
            PlaceHouse(position, structure.gameObject);
        }
        else
        {
            PlaceSpecial(position, structure.gameObject);
        }
    }

    public void PlaceHouse(Vector3Int position, GameObject obj = null)
    {
        if (CheckPositionBeforePlacement(position))
        {
            if (obj != null)
            {
                placementManager.PlaceObjectOnTheMap(position, obj, CellType.Structure);
                return;
            }
            placementManager.PlaceObjectOnTheMap(position, housePrefab,CellType.Structure);
        }
    }

    public void PlaceSpecial(Vector3Int position, GameObject obj = null)
    {
        if (CheckPositionBeforePlacement(position))
        {
            if(obj != null)
            {
                placementManager.PlaceObjectOnTheMap(position, obj, CellType.SpecialStructure);
                return;
            }
            placementManager.PlaceObjectOnTheMap(position, specialPrefab, CellType.SpecialStructure);
        }
    }

    public void PlaceBigStructure(Vector3Int position,GameObject prefab,int width,int height)
    {
        if(CheckBigStructure(position,width, height))
        {
            placementManager.PlaceObjectOnTheMap(position, prefab, CellType.SpecialStructure,width,height);
        }
    }
    private bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        bool nearRoad = false;
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                var newPos = position + new Vector3Int(x, 0, z);
                if (!nearRoad)
                {
                    nearRoad = RoadCheck(newPos);
                }
                if(DeafaultCheck(newPos))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
        }
        return nearRoad;
    }
    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if(!DeafaultCheck(position)) return false;
        if(!RoadCheck(position)) return false;
        return true;
    }

    // check if the cell is on the map or empty
    private bool DeafaultCheck(Vector3Int position)
    {
        if (!placementManager.CheckIfPositionInBound(position))
        {
            Debug.Log("Out of bound");
            return false;
        }
        if (!placementManager.CheckIfPositionIsFree(position))
        {
            Debug.Log("Not Empty");
            return false;
        }
        return true;
    }

    // check if the cell is near the road
    private bool RoadCheck(Vector3Int position)
    {
        if (placementManager.GetNeighbourOfTypesFor(position, CellType.Road).Count <= 0)
        {
            Debug.Log("Must be placed near a road");
            return false;
        }
        return true;
    }
    public HouseTypeInfo ReturnHouseInfoFor(HouseType type)
    {
        HouseTypeInfo result;
        houseInfoDict.TryGetValue(type, out result);
        return result;
    }

    public void UpdatePlayerChoicesDict(HouseType type, HouseChoice choice)
    {
        if(playerChoices.ContainsKey(type))
        {
            var choices = playerChoices[type];
            //playerChoices[type] = choice;
            choices.Add(choice);
            if (choices.Count > ReturnHouseInfoFor(type).requiredChoicesCount)
            {
                choices.RemoveAt(0);
            }
            return;
        }
        //playerChoices.Add(type, choice);
        playerChoices.Add(type, new List<HouseChoice> { choice });
    }

    public Dictionary<HouseType, List<HouseChoice>> GetPlayerChoicesDict()
    {
        return playerChoices;
    }

    bool CloseToMainHouse(Vector3Int position)
    {
        foreach(var pos in placementManager.GetNeighbourOfTypesFor(position, CellType.Structure))
        {
            var house = placementManager.GetStructureAt(pos).GetComponent<HouseStructure>();
            if (house.isMainHouse) return true;
        }

        return false;
    }
}
