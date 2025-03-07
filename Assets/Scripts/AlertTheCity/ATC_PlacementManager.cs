using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ATC_PlacementManager : MonoBehaviour
{
    public Mesh mesh;
    public int width, height;
    Grid placementGrid;

    private Dictionary<Vector3Int, ATC_StructureModel> tempRoadObjects = new Dictionary<Vector3Int, ATC_StructureModel> ();
    private Dictionary<Vector3Int, ATC_StructureModel> structureDict = new Dictionary<Vector3Int, ATC_StructureModel> ();

    private void Awake()
    {
        placementGrid = new Grid(width, height);
    }
    private void Start()
    {
    }

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

        GameObject structure = Instantiate(structurePrefab,transform);
        structure.transform.localPosition = pos;
        var structureModel = structure.AddComponent<ATC_StructureModel>();
        structureModel.CreateModel(structure);
        //GameObject structure = new GameObject(type.ToString());
        //structure.transform.SetParent(transform);
        //structure.transform.localPosition = pos;
        //var structureModel = structure.AddComponent<ATC_StructureModel>();
        //structureModel.CreateModel(structurePrefab);
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

    internal void  SetCostFor (Vector3Int pos, float cost)
    {
        var newCost = GetCostFor(pos) + cost;
        placementGrid.SetCellCost(pos.x, pos.z, newCost);
    }

    internal float GetCostFor (Vector3Int pos)
    {
        return placementGrid.GetCellCost(pos.x, pos.z);
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

    public List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition, bool isAgent = false)
    {
        //var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x, endPosition.z), isAgent);
        //List<Vector3Int> path = new List<Vector3Int>();
        //Debug.Log("Astar: ");
        //foreach (Point point in resultPath)
        //{
        //    path.Add(new Vector3Int(point.X, 0, point.Y));
        //    Debug.Log(new Vector3Int(point.X, 0, point.Y));
        //}
        //return path;
       
        var paths = GridSearch.KShortestPaths(placementGrid,
                                      new Point(startPosition.x, startPosition.z),
                                      new Point(endPosition.x, endPosition.z),
                                      2, isAgent);

        
        if (paths.Count == 0)
        {
            return new List<Vector3Int>(); // Return an empty list if no paths are found
        }

        int index = UnityEngine.Random.Range(0, paths.Count);

        var selectedPath = paths[index];
        //Debug.Log("Kshortest: ");
        selectedPath.Points.Reverse();
        List<Vector3Int> convertedPath = selectedPath.Points.Select(p => new Vector3Int(p.X, 0, p.Y)).ToList();
        return convertedPath;
    }

    internal void AddTempStructureToStructureDict()
    {
        foreach (var structure in tempRoadObjects)
        {
            //Debug.Log($"Added {structure.Key} to structure dict");
            structureDict.Add(structure.Key, structure.Value);
        }
        tempRoadObjects.Clear();
    }

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type, int width = 1, int height = 1)
    {
        ATC_StructureModel structure = CreateANewStructureModel(position, structurePrefab, type);
        structure.gameObject.layer = 10;

        var structureNeedingRoad = structure.GetComponent<INeedingRoad>();
        if (structureNeedingRoad != null)
        {
            structureNeedingRoad.RoadPosition = GetNearestRoad(position, width, height).Value;
            //Debug.Log("My nearest road position is: " + structureNeedingRoad.RoadPosition);
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                //Debug.Log(newPosition); 
                placementGrid[newPosition.x, newPosition.z] = type;
                structureDict.Add(newPosition, structure);
            }
        }

    }

    internal Vector3Int? GetNearestRoad(Vector3Int position, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newPosition = position + new Vector3Int(x, 0, y);
                var roads = GetNeighbourOfTypesFor(newPosition, CellType.Road);
                if (roads.Count > 0)
                {
                    return roads[0];
                }


            }
        }
        return null;
    }
    internal Vector3Int? GetNearestHouse(Vector3Int position, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newPosition = position + new Vector3Int(x, 0, y);
                var roads = GetNeighbourOfTypesFor(newPosition, CellType.Structure);
                if (roads.Count > 0)
                {
                    return roads[0];
                }


            }
        }
        return null;
    }

    public List<ATC_StructureModel> GetAllHouses()
    {
        List<ATC_StructureModel> returnList = new List<ATC_StructureModel>();
        var housePositions = placementGrid.GetAllHouses();
        foreach (var point in housePositions)
        {
            returnList.Add(structureDict[new Vector3Int(point.X, 0, point.Y)]);
        }
        return returnList;
    }

    public ATC_StructureModel GetStructureAt(Vector3Int position)
    {
        if (structureDict.ContainsKey(position))
        {
            return structureDict[position];
        }
        return null;
    }
    private ATC_StructureModel GetStructureAt(Point point)
    {
        if (point != null)
        {
            return structureDict[new Vector3Int(point.X, 0, point.Y)];
        }
        return null;
    }
    public ATC_StructureModel GetRandomRoad()
    {
        var point = placementGrid.GetRandomRoadPoint();
        return GetStructureAt(point);
    }

    public ATC_StructureModel GetRandomSpecialStrucutre()
    {
        var point = placementGrid.GetRandomSpecialStructurePoint();
        return GetStructureAt(point);
    }

    public ATC_StructureModel GetRandomHouseStructure()
    {
        var point = placementGrid.GetRandomHouseStructurePoint();
        return GetStructureAt(point);
    }

    public ATC_StructureModel GetRandomSpecialStructursOfType(StructureType structureType)
    {
        List<ATC_StructureModel> structureList = new List<ATC_StructureModel>();
        foreach (var p in placementGrid.GetAllSpecialStructure())
        {
            var structureModel = GetStructureAt(p);
            var s = structureModel.GetComponent<Structure>();
            if (s.structureType == structureType)
            {
                structureList.Add(structureModel);
            }
        }
        var structure = structureList[UnityEngine.Random.Range(0, structureList.Count-1)];
        return structure;
    }

}
