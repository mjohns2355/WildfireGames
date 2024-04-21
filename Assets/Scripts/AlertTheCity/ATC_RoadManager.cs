using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_RoadManager : MonoBehaviour
{
    public ATC_PlacementManager placementManager;

    public List<Vector3Int> tempPlacementPos = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;
    public RoadFixer roadFixer;

    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();
    }
    public void PlaceRoad(Vector3Int pos)
    {
        if (placementManager.CheckIfPositionInBound(pos) == false) return;
        if (placementManager.CheckIfPositionIsFree(pos) == false) return;
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
