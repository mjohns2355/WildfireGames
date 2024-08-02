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
                house.SetHouseType((HouseType)i);

                allHouses.Remove(structure);
            }
        }
        foreach (var structure in allHouses)
        {
            var house = structure.GetComponent<HouseStructure>();
            // skip specified house
            if (house.houseType != HouseType.none) return;
            house.RandomizeHouseType();
        }

    }
    public void PlacePreBuiltStructures()
    {
        List<Vector3Int> prebuiltHousePos = new List<Vector3Int>();
        List<Vector3Int> prebuiltSpecialPos = new List<Vector3Int>();
        for (int i = 0; i < structureTilemap.transform.childCount; i++)
        {
            var structure = structureTilemap.transform.GetChild(i);
            Vector3Int pos = Vector3Int.RoundToInt(structure.position);
            if (structure.name == "House")
            {
                //prebuiltHousePos.Add(pos);
                if (CheckPositionBeforePlacement(pos))
                {
                    structure.localPosition = Vector3.zero;
                    placementManager.PlaceObjectOnTheMap(pos, structure.gameObject, CellType.Structure);
                }
            }
            else if (structure.name == "Shelter")
            {
                prebuiltSpecialPos.Add(pos);
            }
            Destroy(structureTilemap.transform.GetChild(i).gameObject);
        }

        foreach (var pos in prebuiltHousePos)
        {
            //PlaceHouse(pos);
        }

        foreach (var pos in prebuiltSpecialPos)
        {
            PlaceSpecial(pos);
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

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if(placementManager.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("Out of bound");
            return false;
        }
        if (placementManager.CheckIfPositionIsFree(position) == false)
        {
            Debug.Log("Not Empty");
            return false;
        }
        if (placementManager.GetNeighbourOfTypesFor(position,CellType.Road).Count <=0 )
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
                return info;
            }
        }
        return null;
    }

}
