using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class HouseModel
{
    public HouseType houseType;
    public GameObject model;
}
public class StructureManager : MonoBehaviour
{
    public List<HouseModel> houseModels = new();
    public List<GameObject> allHouseModels = new();
    public GameObject housePrefab;
    public GameObject specialPrefab;
    public ATC_PlacementManager placementManager;
    public GameObject structureTilemap;
    [SerializeField] List<HouseTypeInfo> houseInfos = new ();
    public List<ATC_StructureModel> allHouses = new ();
    public Dictionary<HouseType, HouseStructure> allMainHouses = new();

    // player choices
    Dictionary<HouseType, List<HouseChoice>> playerChoices = new();
    public Dictionary<HouseType, HouseTypeInfo> houseInfoDict = new();
    Dictionary<HouseType, GameObject> houseModelsDict = new ();
    public Dictionary<HouseType, List<HouseStructure>> houseTypeDict = new();
    public Dictionary<StructureType, ATC_StructureModel> specialStructureDict;
    private void Awake()
    {
        InitHouseModelsDict();
    }
    private void Start()
    {
        
        PlacePreBuiltStructures();
        InitialHouseInfoDict();
        //InitiPlayerChoiceDict();

        InitSpecialStructDict();
        InitialMainHouses();
        
        CalculateTotalCars();
        GameManager.Instance.totalHouses = allHouses.Count;
        //Debug.Log($"Total Houses: {GameManager.Instance.totalHouses}, Total Cars: {GameManager.Instance.totalCars}");
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
                //Debug.Log($"Init special structure dict: {type}");
                var structure = placementManager.GetRandomSpecialStructursOfType(type);
                if(structure != null)
                {
                    specialStructureDict[type] = structure;
                }
            }
        }
    }

    void InitHouseModelsDict()
    {
        if (houseModels.Count == 0)
        {
            Debug.LogError("No house models found");
            return;
        }

        foreach (var model in houseModels)
        {
            if (!houseModelsDict.ContainsKey(model.houseType))
            {
                houseModelsDict.Add(model.houseType, model.model);
            }
        }
    }
    void UpdateSameTypeHouseDict(HouseType key, HouseStructure house)
    {

        if (!houseTypeDict.ContainsKey(house.houseType))
        {
            houseTypeDict[house.houseType] = new List<HouseStructure>();
        }

        houseTypeDict[key].Add(house);

    }
    private void InitialHouseInfoDict()
    {
        houseInfoDict.Clear();

        foreach (var info in houseInfos)
        {
            houseInfoDict[info.houseType] = Instantiate(info);
        }
    }

    //private void InitiPlayerChoiceDict()
    //{
    //    foreach (var houseType in GameManager.Instance.availableHouseTypes)
    //    {
    //        playerChoices[houseType] = new List<HouseChoice>();
    //    }
    //}

    
    private void InitialMainHouses()
    {
        foreach(var pair in houseTypeDict)
        {
            var houseType = pair.Key;
            var sameTypeHouseList = pair.Value;
            if (!allMainHouses.ContainsKey(houseType))
            {
                HouseStructure selectedHouse = null;

                // Try to find a valid house that is not close to another main house
                for (int i = 0; i < sameTypeHouseList.Count; i++)
                {
                    var house = sameTypeHouseList[UnityEngine.Random.Range(0, sameTypeHouseList.Count)];
                    var housePosition = Vector3Int.RoundToInt(house.transform.position);

                    if (!CloseToMainHouse(housePosition))
                    {
                        selectedHouse = house;
                        break;
                    }
                }

                // If no valid house was found, pick the first available one
                if (selectedHouse == null)
                {
                    selectedHouse = sameTypeHouseList[0];
                }

                InitMainHouse(houseType, selectedHouse);

                sameTypeHouseList.Remove(selectedHouse);
            }
        }

        // spawn the models after the type is set
        foreach (var s in allHouses)
        {
            var house = s.GetComponent<HouseStructure>();
            if(house.isMainHouse) continue;
            house.SpawnHouseModel();
        }
    }

    private void InitMainHouse(HouseType houseType, HouseStructure house)
    {
        //house.outline.enabled = true;

        house.isMainHouse = true;
        house.SetHouseType(houseType);
        house.houseInfo = ReturnHouseInfoFor(houseType);
        house.houseInfo.InitHouseInfo(house);
        allMainHouses[houseType] = house;
        UpdatePlayerChoicesDict(houseType, house.houseInfo.defaultChoice);
        // remove main house from same type house list
        houseTypeDict[houseType].Remove(house);
        house.InitMainHouse();

    }

    private void PlacePreBuiltStructures()
    {
        for (int i = 0; i < structureTilemap.transform.childCount; i++)
        {
            var structure = structureTilemap.transform.GetChild(i).GetComponent<Structure>();
            Vector3Int pos = Vector3Int.RoundToInt(structure.transform.position);
            PlacePreBuiltStructure(pos, structure);
            Destroy(structureTilemap.transform.GetChild(i).gameObject);
        }

        EvenlyRandomizeHouse();
    }

    void EvenlyRandomizeHouse()
    {
        if (allHouses.Count == 0)
        {
            allHouses = placementManager.GetAllHouses();
        }

        Dictionary<HouseType, int> presetHouseCounts = new Dictionary<HouseType, int>();
        List<HouseStructure> housesToRandomize = new List<HouseStructure>();
        foreach (var structure in allHouses)
        {
            var house = structure.GetComponent<HouseStructure>();

            if (house.houseType == HouseType.none || !GameManager.Instance.availableHouseTypes.Contains(house.houseType))
            {
                housesToRandomize.Add(house);
            }

            else
            {
                UpdateSameTypeHouseDict(house.houseType, house);
                // Count pre-set house types
                if (!presetHouseCounts.ContainsKey(house.houseType))
                {
                    presetHouseCounts[house.houseType] = 0;
                }
                presetHouseCounts[house.houseType]++;
            }
        }


        int totalHouses = housesToRandomize.Count;
        int numTypes = GameManager.Instance.availableHouseTypes.Count();
        //Debug.Log($"Total Houses: {totalHouses}, Num Types: {numTypes}");
        Dictionary<HouseType, int> targetCounts = new Dictionary<HouseType, int>();

        // Calculate base counts
        int baseCount = allHouses.Count / numTypes;
        int remainder = allHouses.Count % numTypes;

        // Create the target distribution list
        List<HouseType> targetDistribution = new List<HouseType>();

        // Distribute the base count for each type
        foreach (var type in GameManager.Instance.availableHouseTypes)
        {
            int alreadyAssigned = presetHouseCounts.ContainsKey(type) ? presetHouseCounts[type] : 0;
            int countToAdd = baseCount - alreadyAssigned;
            for (int i = 0; i < countToAdd; i++)
            {
                targetDistribution.Add(type);
            }
        }

        // Distribute the remainder randomly across types
        List<int> remainderIndices = Enumerable.Range(0, numTypes).OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < remainder; i++)
        {
            targetDistribution.Add(GameManager.Instance.availableHouseTypes[remainderIndices[i]]);
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
            UpdateSameTypeHouseDict(targetDistribution[i], housesToRandomize[i]);
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

    private void PlacePreBuiltStructure(Vector3Int position, Structure structure)
    {
        structure.transform.localPosition = Vector3.zero;
        if (structure.IsBigStructure)
        {
            PlaceBigStructure(position, structure.gameObject, structure.width, structure.height);
        }
        else if (structure.structureType == StructureType.House)
        {
            //structure.GetComponent<HouseStructure>().SpawnHouseModel();
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

    public bool IsAllHouseChoseGoodOptions()
    {
        int count = 0;
        foreach (var type in GameManager.Instance.availableHouseTypes)
        {
            if (playerChoices.TryGetValue(type, out var choices) && choices.Any(c => c.isLocked)) count++;
        }

        Debug.Log($"Count: {count}, availableHouseTypes.Count: {GameManager.Instance.availableHouseTypes.Count}");
        return count == GameManager.Instance.availableHouseTypes.Count;
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

    public void ClearAllPlayerChoices()
    {
       foreach( var house in allMainHouses)
        {
            UpdatePlayerChoicesDict(house.Key, house.Value.houseInfo.defaultChoice);
        }
    }

    void CalculateTotalCars()
    {
        int totalCars = 0;
        foreach (var pair in houseTypeDict)
        {
            if(pair.Key == HouseType.twoCar)
            {
                totalCars += pair.Value.Count * 2;
            }
            else
            {
                totalCars += pair.Value.Count;
            }
        }

        GameManager.Instance.totalCars = totalCars;
    }

    public GameObject GetHouseModel(HouseType type)
    {
        if (houseModelsDict.TryGetValue(type, out GameObject model))
        {
            return model;
        }
        else
        {
            Debug.LogError($"House model not found for type: {type}");
            return null;
        }
    }
}
