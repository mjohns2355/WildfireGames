using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_RoadManager : MonoBehaviour
{
    public ATC_PlacementManager placementManager;
    public StructureManager structureManager;
    public GameObject tileMap;
    public List<Vector3Int> tempPlacementPos = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;
    public RoadFixer roadFixer;
 
    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();

        PlacePreBuiltRoad();
        structureManager.PlacePreBuiltStructures();
    }

    void PlacePreBuiltRoad()
    {
        List<Vector3Int> prebuiltPos = new List<Vector3Int>();
        for (int i = 0; i < tileMap.transform.childCount; i++)
        {
            Vector3Int pos = Vector3Int.RoundToInt(tileMap.transform.GetChild(i).position);
            prebuiltPos.Add(pos);
            Destroy(tileMap.transform.GetChild(i).gameObject);
        }

        foreach (var pos in prebuiltPos)
        {
            tempPlacementPos.Add(pos);
            placementManager.PlaceTempStructure(pos, roadFixer.deadEnd, CellType.Road);

        }
        FixRoadPrefabs();
        FinishPlacingRoad();
    }

    public void PlaceRoad(Vector3Int pos)
    {

        if (placementManager.CheckIfPositionInBound(pos) == false) return;
        if (placementManager.CheckIfPositionIsFree(pos) == false) return;
        Debug.Log(pos);

        if (placementMode == false)
        {
            tempPlacementPos.Clear();
            roadPositionsToRecheck.Clear();

            placementMode = true;
            startPosition = pos;

            tempPlacementPos.Add(pos);
            placementManager.PlaceTempStructure(pos, roadFixer.deadEnd, CellType.Road);

        }
        else
        {
            placementManager.RemoveAllTempStructures();
            tempPlacementPos.Clear();

            foreach (var posToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, posToFix);
            }
            roadPositionsToRecheck.Clear();

            tempPlacementPos = placementManager.GetPathBetween(startPosition, pos);

            foreach (var tempPos in tempPlacementPos)
            {
                if (placementManager.CheckIfPositionIsFree(tempPos) == false)
                {
                    roadPositionsToRecheck.Add(tempPos);
                    continue;
                }
                placementManager.PlaceTempStructure(tempPos, roadFixer.deadEnd, CellType.Road);
            }
        }
        FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach(var tempPos in tempPlacementPos)
        {
            roadFixer.FixRoadAtPosition(placementManager, tempPos);
            var neighbours = placementManager.GetNeighbourOfTypesFor(tempPos, CellType.Road);
            foreach (var roadPos in neighbours)
            {
                if(roadPositionsToRecheck.Contains(roadPos) == false)
                {
                    roadPositionsToRecheck.Add(roadPos);
                }
            }
        }
        foreach(var posToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager,posToFix);
        }
    }

    public void FinishPlacingRoad()
    {
        placementMode = false;
        placementManager.AddTempStructureToStructureDict();
        tempPlacementPos.Clear();
        startPosition = Vector3Int.zero;
    }
}
