using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class ATC_AIDirector : UnitySingleton<ATC_AIDirector>
{
    public ATC_PlacementManager placementManager;

    public GameObject carPrefab;
    public List<GameObject> carModels;
    AdjacencyGraph testcarGraph = new AdjacencyGraph();

    List<Vector3Int> testcarPath = new List<Vector3Int>();
    ATC_StructureModel startStructure;
    ATC_StructureModel endStructure;

    public int spawnedCarNum, currentCarNum = 0;

    private void Start()
    {
        carModels = Resources.LoadAll<GameObject>("FirewiseCitizen/Cars").ToList();
    }
    //random destination
    public void SpawnACar()
    {
        foreach (var house in placementManager.GetAllHouses())
        {
            var houseStructure = house.GetComponentInChildren<HouseStructure>();
            if (houseStructure != null && houseStructure.CanSpawnCar())
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

        StartCoroutine(CarSpawn(carNum, startStructure, new List<ATC_StructureModel> { endStructure }, carSpeed));

    }

    public void SpawnCarWithMultipleStops(ATC_StructureModel startStructure, List<ATC_StructureModel> stops, CarSpeed carSpeed = CarSpeed.medium, int carNum = 1)
    {
        var house = startStructure.GetComponent<HouseStructure>();

        if (house != null && !house.CanSpawnCar()) return;
        StartCoroutine(CarSpawn(carNum, startStructure, stops, carSpeed, true));

    }
    IEnumerator CarSpawn(int carNum, ATC_StructureModel startStructure, List<ATC_StructureModel> endStructures, CarSpeed carSpeed, bool hasMultipleStops = false)
    {
        for (int i = 0; i < carNum; i++)
        {
            if (hasMultipleStops) {
                TrySpawnCarWithMultipleStops(startStructure, endStructures, carSpeed);
            }
            else
            {
                TrySpawnACar(startStructure, endStructures[0], carSpeed);
            }

            //structure.AfterSpawnACar();
            // wait for one sec to spawn a new car
            yield return WaitForNextCar();
        }
    }

    private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

    private IEnumerator WaitForNextCar()
    {
        // Centralized wait logic to optimize memory and avoid multiple WaitForSeconds instantiations
        yield return waitOneSecond;
    }
    private void TrySpawnACar(ATC_StructureModel startStructure, ATC_StructureModel endStructure, CarSpeed carSpeed = CarSpeed.medium)
    {
        var Thouse = startStructure.GetComponent<HouseStructure>();

        List<Vector3> carPath = new List<Vector3>();
        this.startStructure = startStructure;
        this.endStructure = endStructure;
        //bool isHouse = true;
        if (startStructure != null && endStructure != null)
        {
            Vector3Int startRoadPos;
            if (startStructure.GetComponent<HouseStructure>() != null)
            {
                startRoadPos = ((INeedingRoad)startStructure).RoadPosition;
            }
            else
            {
                startRoadPos = Vector3Int.RoundToInt(startStructure.transform.position);
                //isHouse = false;
            }
            var endRoadPos = ((INeedingRoad)endStructure).RoadPosition;
            //Debug.Log("start: " + startRoadPos + ",end: " + endRoadPos);
            var path = placementManager.GetPathBetween(startRoadPos, endRoadPos, true);
            path.Reverse();

            if (path.Count == 0 || path.Count < 2) return;
            //if (Thouse.testHouse)
            //{

            //    testcarPath = path;
            //}
            bool useInner = ShouldTakeInnerCarMarkers();

            var start = placementManager.GetStructureAt(startRoadPos);
            start.transform.GetChild(0).GetComponent<RoadHelper>().useInner = useInner;
            var end = placementManager.GetStructureAt(endRoadPos);
            end.transform.GetChild(0).GetComponent<RoadHelper>().useInner = useInner;

            var startMarkerPosition = start.GetCarSpawnMarker(path[1]);
            var endMarkerPosition = end.GetCarEndMarker(path[path.Count - 2]);
            carPath = GetCarPath(path, startMarkerPosition.Position, endMarkerPosition.Position, useInner);


            //if (Thouse.testHouse)
            //{
            //    CreateACarGraph(path, testcarGraph, useInner);

            //}
            if (carPath.Count > 0)
            {
                //var house = startStructure.GetComponent<HouseStructure>();
                var carSpawner = carPrefab.GetComponent<ATC_CarSpawner>();
                //carSpawner.hasHorseTrailer = house.HasHorseTrailers;
                var car = Instantiate(carPrefab, startMarkerPosition.Position, Quaternion.identity);
                car.GetComponent<CarController>().carSpeed = carSpeed;
                car.GetComponent<CarController>().start = startStructure;
                car.GetComponent<CarController>().ends.Add(endStructure);
                car.GetComponent<CarAI>().SetPath(carPath);

                //debug
                spawnedCarNum++;
                currentCarNum++;
            }
            else
            {
                Debug.Log("NoPath");
            }
        }
    }

    public List<Vector3> FindPath(ATC_StructureModel startStructure, ATC_StructureModel endStructure)
    {
        if (startStructure == null || endStructure == null) return null;
        //List<Vector3> carPath = new List<Vector3>();
        Vector3Int startRoadPos;
        if (startStructure.GetComponent<HouseStructure>() != null)
        {
            startRoadPos = ((INeedingRoad)startStructure).RoadPosition;
        }
        else
        {
            startRoadPos = Vector3Int.RoundToInt(startStructure.transform.position);
            //isHouse = false;
        }
        var endRoadPos = ((INeedingRoad)endStructure).RoadPosition;
        //Debug.Log("start: " + startRoadPos + ",end: " + endRoadPos);
        var path = placementManager.GetPathBetween(startRoadPos, endRoadPos, true);
        path.Reverse();

        if (path.Count == 0 || path.Count < 2) return null;
        //if (Thouse.testHouse)
        //{

        //    testcarPath = path;
        //}
        bool useInner = ShouldTakeInnerCarMarkers();

        var start = placementManager.GetStructureAt(startRoadPos);
        start.transform.GetChild(0).GetComponent<RoadHelper>().useInner = useInner;
        var end = placementManager.GetStructureAt(endRoadPos);
        end.transform.GetChild(0).GetComponent<RoadHelper>().useInner = useInner;

        var startMarkerPosition = start.GetCarSpawnMarker(path[1]);
        var endMarkerPosition = end.GetCarEndMarker(path[path.Count - 2]);
        if (startMarkerPosition == null || endMarkerPosition == null) return null;
        var carPath = GetCarPath(path, startMarkerPosition.Position, endMarkerPosition.Position, useInner);
        return carPath;
    }
    bool ShouldTakeInnerCarMarkers()
    {
        return UnityEngine.Random.Range(0f, 1f) < 0.5f? true : false;
    }
    private void TrySpawnCarWithMultipleStops(ATC_StructureModel startStructure, List<ATC_StructureModel> stops, CarSpeed carSpeed = CarSpeed.medium, int carNum = 1)
    {
        List<Vector3> carPath = new List<Vector3>(); 
        var previousStop = startStructure;
        Vector3 startPos = Vector3.zero;
        List<Vector3> stopPos = new List<Vector3>();
        if (startStructure != null && stops.Count!= 0)
        {
            bool useInner = ShouldTakeInnerCarMarkers();
            //Debug.Log($"Multi: IsUseInner: {useInner}");
            for (int i = 0; i < stops.Count; i++)
            {
                var stop = stops[i];
                var startRoadPos = ((INeedingRoad)previousStop).RoadPosition;
                var endRoadPos = ((INeedingRoad)stop).RoadPosition;
                //Debug.Log("start: " + startRoadPos + ",end: " + endRoadPos);
                var path = placementManager.GetPathBetween(startRoadPos, endRoadPos, true);
                path.Reverse();

                if (path.Count == 0 || path.Count < 2) return;


                var start = placementManager.GetStructureAt(startRoadPos);
                start.transform.GetChild(0).GetComponent<RoadHelper>().useInner = useInner;
                var end = placementManager.GetStructureAt(endRoadPos);
                end.transform.GetChild(0).GetComponent<RoadHelper>().useInner = useInner;
                var startMarkerPosition = start.GetCarSpawnMarker(path[1]);
                //var startMarkerPosition = placementManager.GetStructureAt(startRoadPos).GetCarSpawnMarker(path[1]);
                if (i == 0)
                {
                    startPos = startMarkerPosition.Position;
                }
                var endMarkerPosition = end.GetCarEndMarker(path[path.Count - 2]);
               // var endMarkerPosition = placementManager.GetStructureAt(endRoadPos).GetCarEndMarker(path[path.Count - 2]);
                stopPos.Add(endMarkerPosition.Position);
                carPath.AddRange(GetCarPath(path, startMarkerPosition.Position, endMarkerPosition.Position, useInner));
                previousStop = stop;
            }

            if (carPath.Count > 0)
            {
                var house = startStructure.GetComponent<HouseStructure>();
                var carSpawner = carPrefab.GetComponent<ATC_CarSpawner>();
                //carSpawner.hasHorseTrailer = house.HasHorseTrailers;
                var car = Instantiate(carPrefab, startPos, Quaternion.identity);
                car.GetComponent<CarController>().carSpeed = carSpeed;
                car.GetComponent<CarController>().start = startStructure;
                car.GetComponent<CarController>().ends = stops;
                car.GetComponent<CarAI>().SetPath(carPath);
                car.GetComponent<CarAI>().SetStops(stopPos);

                //debug
                spawnedCarNum++;
                currentCarNum++;
            }
            else
            {
                Debug.Log("Multi Stops: No path");
            }
        }
    }
    public void RespawnACar(ATC_StructureModel startStructure, List<ATC_StructureModel> endStructures, CarSpeed carSpeed = CarSpeed.medium, int carNum = 1)
    {
        if(startStructure == null) return;
        //Debug.Log("Respawn Car");
        //debug
        spawnedCarNum--;
        currentCarNum--;
        if (endStructures.Count == 1)
        {
            SpawnACar(startStructure, endStructures[0], carSpeed, carNum);
        }
        else
        {
            SpawnCarWithMultipleStops(startStructure, endStructures, carSpeed, carNum);
        }
    }

    private List<Vector3> GetCarPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition, bool useInner = true)
    {
        AdjacencyGraph carGraph = new AdjacencyGraph();
        //carGraph.ClearGraph();
        CreateACarGraph(path,carGraph,useInner);
        //Debug.Log(carGraph);
        return AdjacencyGraph.AStarSearch(carGraph, startPosition, endPosition);
    }

    private void CreateACarGraph(List<Vector3Int> path, AdjacencyGraph graph, bool useInner)
    {
        Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            var currentPoistion = path[i];
            var roadStructure = placementManager.GetStructureAt(currentPoistion);
            var roadHelper = roadStructure.transform.GetChild(0).GetComponent<RoadHelper>();
            roadHelper.useInner = useInner;
        }
            for (int i = 0; i <path.Count; i++)
        {
            var currentPoistion  = path[i];
            var roadStructure = placementManager.GetStructureAt(currentPoistion);
            var markersList = roadStructure.GetCarMarkers();
            var limitDistance = markersList.Count > 3;
            tempDictionary.Clear();
            foreach (var marker in markersList) {

                //carGraph.AddVertex(marker.Position);
                graph.AddVertex(marker.Position);
                foreach (var markerNeighbour in marker.adjacentMarkers)
                {
                    //carGraph.AddEdge(marker.Position, markerNeighbour.Position);
                    graph.AddEdge(marker.Position, markerNeighbour.Position);
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
                        //carGraph.AddEdge(marker.Position, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                        graph.AddEdge(marker.Position, nextRoadPosition.GetNearestCarMarkerTo(marker.Position));
                    }
                }
            }
            if (limitDistance && tempDictionary.Count > 2)
            {
                var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                for (int j = 0; j < 2; j++)
                {
                    //carGraph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                    graph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                }
            }
        }
    }

    private void Update()
    {
        //DrawCarGraph(testcarGraph);
        //DrawCarPath(testcarPath);
    }

    void DrawCarPath(List<Vector3Int> path)
    {

        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1] + Vector3.up * 2, path[i] + Vector3.up * 2, Color.magenta);
        }

    }

    void DrawCarGraph(AdjacencyGraph graph)
    {
        foreach (var vertex in graph.GetVertices())
        {
            foreach (var vertexNeighbour in graph.GetConnectedVerticesTo(vertex))
            {
                Debug.DrawLine(vertex.Position + Vector3.up, vertexNeighbour.Position + Vector3.up, Color.red);
            }
        }
    }
}
