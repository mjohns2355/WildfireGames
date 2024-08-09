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
    public List<HouseTypeInfo> houseInfos = new List<HouseTypeInfo>();
    public List<ATC_StructureModel> allHouses = new List<ATC_StructureModel>();
    public List<HouseStructure> allMainHouses = new List<HouseStructure>();
    private void Start()
    {
        
        
        
    }
    public void InitialMainHouses()
    {
        if(allHouses.Count == 0)
        {
            allHouses = placementManager.GetAllHouses();
        }

        
        //make sure each type has at least one house in the group
        for (int i = 1; i < Enum.GetValues(typeof(HouseType)).Length; i++)
        {
            //Debug.Log(allHouses.Count);
            if (allHouses.Count == 0) return;
            var structure = allHouses[UnityEngine.Random.Range(0, allHouses.Count-1)];
            
            var house = structure.GetComponent<HouseStructure>();
            if (house && !house.isMainHouse)
            {
                house.isMainHouse = true;
                house.houseInfo = ReturnHouseInfoFor((HouseType)i);
                house.houseInfo.InitHouseInfo(house);

                //Debug.Log(house.houseInfo.GetInstanceID());
                house.SetHouseType((HouseType)i);

                allHouses.Remove(structure);
            }
        }
        foreach (var structure in allHouses)
        {
            var house = structure.GetComponent<HouseStructure>();
            // skip specified house
            if (house.houseType != HouseType.none) {
                Debug.Log("SKIP");
                return;
            }
            house.RandomizeHouseType();
        }

    }
    public void PlacePreBuiltStructures()
    {
        //List<Vector3Int> prebuiltHousePos = new List<Vector3Int>();
        //List<Vector3Int> prebuiltSpecialPos = new List<Vector3Int>();
        for (int i = 0; i < structureTilemap.transform.childCount; i++)
        {
            var structure = structureTilemap.transform.GetChild(i).GetComponent<Structure>();
            Vector3Int pos = Vector3Int.RoundToInt(structure.transform.position);
            PlacePreBuiltStructure(pos, structure);
            //if (structure.structureType == StructureType.House)
            //{
            //    PlacePreBuiltStructure(pos, structure, CellType.Structure);
            //    //if (CheckPositionBeforePlacement(pos))
            //    //{
            //    //    structure.transform.localPosition = Vector3.zero;
            //    //                    //keep pre built house model
            //    //    placementManager.PlaceObjectOnTheMap(pos, structure.gameObject, CellType.Structure);
            //    //}
            //}
            //else if (structure.structureType == StructureType.Shelter)
            //{
            //    //prebuiltSpecialPos.Add(pos);
            //    PlacePreBuiltStructure(pos,structure, CellType.SpecialStructure);

            //}
            //else
            //{
            //    Debug.Log(structure.structureType + " is built at " + structure.transform.position);
            //}
            Destroy(structureTilemap.transform.GetChild(i).gameObject);
        }

        //foreach (var pos in prebuiltHousePos)
        //{
        //    PlaceHouse(pos);
        //}

        //foreach (var pos in prebuiltSpecialPos)
        //{
        //    PlaceSpecial(pos);
        //}
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
        if (structure.isBigStructure)
        {
            PlaceBigStructure(position, structure.gameObject, structure.width, structure.height);
        }
        else if (structure.structureType == StructureType.House)
        {
            PlaceHouse(position);
        }
        else
        {
            PlaceSpecial(position);
        }
    }

    public void PlaceHouse(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            placementManager.PlaceObjectOnTheMap(position, housePrefab,CellType.Structure);
        }
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
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
        foreach(var info in houseInfos)
        {
            if (info.houseType == type)
            {
                //var result = info.Clone();
                var result = Instantiate(info);
                return result;
            }
        }
        return null;
    }

}
