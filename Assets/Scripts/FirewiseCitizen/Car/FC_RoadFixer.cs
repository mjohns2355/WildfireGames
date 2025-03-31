using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, roadStraight, corner, threeway, fourway;

    public void FixRoadAtPosition(ATC_PlacementManager placementManager, Vector3Int tempPosition)
    {
        //[left, top, right, down]
        var result = placementManager.GetNeighbourTypesFor(tempPosition);
        int roadCount = 0;
        roadCount = result.Where(x=> x == CellType.Road).Count();
        if(roadCount == 0 || roadCount == 1)
        {
             CreateDeadEnd(placementManager, result, tempPosition);
        }
        else if (roadCount == 2)
        {
            if(CreateStraightRoad(placementManager, result, tempPosition))
                return;
            CreateCorner(placementManager, result, tempPosition);
        }
        else if (roadCount == 3)
        {
            CreateThreeWay(placementManager, result, tempPosition);
        }
        else
        {
            CreateFourWay(placementManager, result, tempPosition);
        }
    }

    private void CreateFourWay(ATC_PlacementManager placementManager, CellType[] result, Vector3Int tempPosition)
    {
        placementManager.ModifyStructureModel(tempPosition, fourway,Quaternion.identity);
    }
    //[left, top, right, down]
    private void CreateThreeWay(ATC_PlacementManager placementManager, CellType[] result, Vector3Int tempPosition)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, threeway, Quaternion.identity);
        } 
        else if (result[2] == CellType.Road && result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, threeway, Quaternion.Euler(0,90,0));
        }
        else if (result[3] == CellType.Road && result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, threeway, Quaternion.Euler(0, 180, 0));
        }
        else if (result[0] == CellType.Road && result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, threeway, Quaternion.Euler(0, 270, 0));
        }
    }

    private void CreateCorner(ATC_PlacementManager placementManager, CellType[] result, Vector3Int tempPosition)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, corner, Quaternion.Euler(0, 90, 0));
        }
        else if (result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, corner, Quaternion.Euler(0, 180, 0));
        }
        else if (result[3] == CellType.Road && result[0] == CellType.Road )
        {
            placementManager.ModifyStructureModel(tempPosition, corner, Quaternion.Euler(0, 270, 0));
        }
        else if (result[0] == CellType.Road && result[1] == CellType.Road )
        {
            placementManager.ModifyStructureModel(tempPosition, corner, Quaternion.identity);
        }
    }

    private bool CreateStraightRoad(ATC_PlacementManager placementManager, CellType[] result, Vector3Int tempPosition)
    {
        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, roadStraight, Quaternion.identity);
            return true;
        }
        else if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, roadStraight, Quaternion.Euler(0, 90, 0));
            return true;
        }
        return false;
    }

    private void CreateDeadEnd(ATC_PlacementManager placementManager, CellType[] result, Vector3Int tempPosition)
    {
        if (result[1] == CellType.Road )
        {
            placementManager.ModifyStructureModel(tempPosition, deadEnd, Quaternion.Euler(0, 270, 0));
        }
        else if (result[2] == CellType.Road )
        {
            placementManager.ModifyStructureModel(tempPosition, deadEnd, Quaternion.identity);
        }
        else if (result[3] == CellType.Road )
        {
            placementManager.ModifyStructureModel(tempPosition, deadEnd, Quaternion.Euler(0, 90, 0));
        }
        else if (result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(tempPosition, deadEnd, Quaternion.Euler(0, 180, 0));
        }
    }
}
