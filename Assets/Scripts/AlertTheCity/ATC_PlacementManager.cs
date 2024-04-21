using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATC_PlacementManager : MonoBehaviour
{
    public int width, height;
    Grid placementGrid;

    private Dictionary<Vector3Int, ATC_StructureModel> tempRoadObjects = new Dictionary<Vector3Int, ATC_StructureModel> ();
    private Dictionary<Vector3Int, ATC_StructureModel> structureDict = new Dictionary<Vector3Int, ATC_StructureModel> ();
    private void Start()
    {
        placementGrid = new Grid(width, height);
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    //}
    internal bool CheckIfPositionInBound(Vector3Int pos)
    {
        if (pos.x >= 0 && pos.x < width && pos.z >= 0 && pos.z < height)
        {
            return true;
        }
        return false;
    }

    internal bool CheckIfPositionIsFree(Vector3Int pos)
    {
        return CheckIfPosIsOfType(pos,CellType.Empty);
    }

    private bool CheckIfPosIsOfType(Vector3Int pos, CellType type)
    {
        return placementGrid[pos.x, pos.z] == type;
    }

    internal void PlaceTempStructure(Vector3Int pos, GameObject structurePrefab, CellType type)
    {
        placementGrid[pos.x, pos.z] = type;
        ATC_StructureModel structure = CreateANewStructureModel(pos, structurePrefab,type);
        tempRoadObjects.Add(pos, structure);
    }

    private ATC_StructureModel CreateANewStructureModel(Vector3Int pos, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = pos;
        var structureModel = structure.AddComponent<ATC_StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    public void ModifyStructureModel(Vector3Int pos, GameObject newModel, Quaternion rotation)
    {
        if (tempRoadObjects.ContainsKey(pos))
        {
            tempRoadObjects[pos].SwapModel(newModel,rotation);
        }
        else if (structureDict.ContainsKey(pos))
        {
            structureDict[pos].SwapModel(newModel, rotation);
        }
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
         
    }

    internal List<Vector3Int> GetNeighbourOfTypesFor(Vector3Int roadPos, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(roadPos.x,roadPos.z,type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in  neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return neighbours;
    }

    internal void RemoveAllTempStructures()
    {
        foreach (var structure in tempRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        tempRoadObjects.Clear();
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid,new Point(startPosition.x,startPosition.z), new Point(endPosition.x, endPosition.z));
        List<Vector3Int> path = new List<Vector3Int>();
        foreach (Point p in resultPath)
        {
            path.Add(new Vector3Int(p.X, 0, p.Y));
        }
        return path;    
    }

    internal void AddTempStructureToStructureDict()
    {
        foreach (var structure in tempRoadObjects)
        {
            structureDict.Add(structure.Key, structure.Value);
        }
        tempRoadObjects.Clear();
    }
}
