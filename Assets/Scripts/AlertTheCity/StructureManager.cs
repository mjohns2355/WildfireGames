using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public GameObject housePrefab;
    public GameObject specialPrefab;
    public ATC_PlacementManager placementManager;

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
}
