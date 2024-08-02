using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ATC_AIDirector : UnitySingleton<ATC_AIDirector>
{
    public ATC_PlacementManager placementManager;

    public GameObject carPrefab;
    AdjacencyGraph carGraph = new AdjacencyGraph();

    List<Vector3> carPath = new List<Vector3>();
    ATC_StructureModel startStructure;
    ATC_StructureModel endStructure;
    
    //random destination
    public void SpawnACar()
    {
        foreach(var house in placementManager.GetAllHouses())
        {
            var houseStructure = house.GetComponentInChildren<HouseStructure>();
            if(houseStructure!= null && houseStructure.CanSpawnCar())
            {
                TrySpawnACar(house, placementManager.GetRandomSpecialStrucutre());
                houseStructure.AfterSpawnACar();
            }

        }
    }

    //specified destination
    public void SpawnACar(ATC_StructureModel startStructure, ATC_StructureModel endStructure, CarSpeed carSpeed = CarSpeed.medium, int carNum = 1)
    {
        //Debug.Log(startStructure, endStructure);
        var structure = startStructure.GetComponentInChildren<HouseStructure>();
        if (structure != null && structure.CanSpawnCar())
        {
            StartCoroutine(CarSpawn(carNum, startStructure, endStructure, carSpeed));
        }
    }

    IEnumerator CarSpawn(int carNum, ATC_StructureModel startStructure, ATC_StructureModel endStructure, CarSpeed carSpeed)
    {
        for (int i = 0; i < carNum; i++)
        {

            TrySpawnACar(startStructure, endStructure, carSpeed);
            //structure.AfterSpawnACar();
            // wait for one sec to spawn a new car
            yield return new WaitForSeconds(1f);
        }
    }
    private void TrySpawnACar(ATC_StructureModel startStructure, ATC_StructureModel endStructure, CarSpeed carSpeed = CarSpeed.medium)
    {
        this.startStructure = startStructure;
        this.endStructure = endStructure;
        if (startStructure != null && endStructure != null)
        {
            var startRoadPos = ((INeedingRoad)startStructure).RoadPosition;
            var endRoadPos = ((INeedingRoad)endStructure).RoadPosition;
            //Debug.Log("start: " + startRoadPos + ",end: " + endRoadPos);
            var path = placementManager.GetPathBetween(startRoadPos, endRoadPos, true);
            path.Reverse();

            if (path.Count == 0 && path.Count > 2) return;


            var startMarkerPosition = placementManager.GetStructureAt(startRoadPos).GetCarSpawnMarker(path[1]);
            var endMarkerPosition = placementManager.GetStructureAt(endRoadPos).GetCarEndMarker(path[path.Count - 2]);
            carPath = GetCarPath(path, startMarkerPosition.Position, endMarkerPosition.Position);
            if (carPath.Count > 0)
            {
                var car = Instantiate(carPrefab, startMarkerPosition.Position, Quaternion.identity);
                car.GetComponent<CarController>().carSpeed = carSpeed;
                car.GetComponent<CarController>().start = startStructure;
                car.GetComponent<CarController>().end = endStructure;
                car.GetComponent<CarAI>().SetPath(carPath);
            }
        }
    }

    public void RespawnACar(ATC_StructureModel startStructure, ATC_StructureModel endStructure, CarSpeed carSpeed = CarSpeed.medium, int carNum = 1)
    {
        SpawnACar(startStructure, endStructure, carSpeed, carNum);
    }
    private List<Vector3> GetCarPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
    {
        carGraph.ClearGraph();
        CreateACarGraph(path);
        //Debug.Log(carGraph);
        return AdjacencyGraph.AStarSearch(carGraph, startPosition, endPosition);
    }

    private void CreateACarGraph(List<Vector3Int> path)
    {
        Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();
        for (int i = 0; i <path.Count; i++)
        {
            var currentPoistion  = path[i];
            var roadStructure = placementManager.GetStructureAt(currentPoistion);
            var markersList = roadStructure.GetCarMarkers();
            var limitDistance = markersList.Count > 3;
            tempDictionary.Clear();
            foreach (var marker in markersList) {

                carGraph.AddVertex(marker.Position);
                foreach (var markerNeighbour in marker.adjacentMarkers)
                {
                    carGraph.AddEdge(marker.Position, markerNeighbour.Position);
                }

                if(marker.OpenForconnections && i+ 1  < path.Count)
                {
                    var nextRoadPosition = placementManager.GetStructureAt(path[i+1]);
                    if (limitDistance)
                    {
                        tempDictionary.Add(marker, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                    }
                    else
                    {
                        carGraph.AddEdge(marker.Position, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                    }
                }
            }
            if (limitDistance && tempDictionary.Count > 2)
            {
                var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                for (int j = 0; j < 2; j++)
                {
                    carGraph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                }
            }
        }
    }

    private void Update()
    {
        foreach (var vertex in carGraph.GetVertices())
        {
            foreach (var vertexNeighbour in carGraph.GetConnectedVerticesTo(vertex))
            {
                Debug.DrawLine(vertex.Position + Vector3.up, vertexNeighbour.Position + Vector3.up, Color.red);
            }
        }

        for (int i = 1;i<carPath.Count;i++)
        {
            Debug.DrawLine(carPath[i - 1] + Vector3.up * 2, carPath[i] + Vector3.up * 2, Color.magenta);
        }

    }
}
