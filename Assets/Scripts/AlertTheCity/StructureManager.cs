using System;
using System.Collections;
using System.Collections.Generic;
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
    public List<HouseStructure> allMainHouses = new List<HouseStructure>();

    Dictionary<HouseType, List<HouseChoice>> playerChoices = new Dictionary<HouseType, List<HouseChoice>>();
    Dictionary<HouseType, HouseTypeInfo> houseInfoDict = new Dictionary<HouseType, HouseTypeInfo>();
    private void Start()
    {
        PlacePreBuiltStructures();
        InitialHouseInfoDict();
        InitiPlayerChoiceDict();
        InitialMainHouses();
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
        if(allHouses.Count == 0)
        {
            allHouses = placementManager.GetAllHouses();
        }

        //make sure each type has at least one house in the group
        foreach(var houseType in GameManager.Instance.availableHouseTypes)
        {
            if (allHouses.Count == 0) return;
            var structure = allHouses[UnityEngine.Random.Range(0, allHouses.Count-1)];
            
            var house = structure.GetComponent<HouseStructure>();
            if (house && !house.isMainHouse && !CloseToMainHouse(structure.RoadPosition))
            {
                house.isMainHouse = true;
                house.houseInfo = ReturnHouseInfoFor(houseType);
                house.houseInfo.InitHouseInfo(house);
                allMainHouses.Add(house);
                //playerChoices[houseType] = house.houseInfo.defaultChoice;
                playerChoices[houseType].Add(house.houseInfo.defaultChoice);
                house.SetHouseType(houseType);
                allHouses.Remove(structure);
            }
        }
        foreach (var structure in allHouses)
        {
            var house = structure.GetComponent<HouseStructure>();
            // skip specified house
            if (house.HouseType != HouseType.none) continue;
            house.RandomizeHouseType();
        }

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
