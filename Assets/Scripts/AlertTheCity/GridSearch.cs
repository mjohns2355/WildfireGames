using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
/// </summary>
public class GridSearch
{

    public struct SearchResult
    {
        public List<Point> Path { get; set; }
    }
    public class Path
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public float Cost { get; set; }
    }

    public static List<Path> KShortestPaths(Grid grid, Point startPosition, Point endPosition, int K, bool isAgent = false)
    {
        List<Path> paths = new List<Path>();

        PriorityQueue<Path> openPaths = new PriorityQueue<Path>();
        var visited = new Dictionary<Point, int>();

        Path startPath = new Path { Points = new List<Point> { startPosition }, Cost = 0 };
        openPaths.Enqueue(startPath, 0);

        while (openPaths.Count > 0 && paths.Count < K)
        {

            // Get the current path with the lowest cost
            Path currentPath = openPaths.Dequeue();
            Point lastPoint = currentPath.Points[currentPath.Points.Count - 1];

            // If we've reached the target, add this path to the results
            if (lastPoint.Equals(endPosition))
            {
                paths.Add(currentPath);
                continue;
            }

            // Track how many times we have visited this point
            if (!visited.ContainsKey(lastPoint))
            {
                visited[lastPoint] = 0;
            }
            visited[lastPoint]++;

            if (visited[lastPoint] > K * 2) continue;
            
            // Explore adjacent cells
            foreach (Point neighbour in grid.GetAdjacentCells(lastPoint, isAgent))
            {
                // Skip if already visited or part of the current path
                if (currentPath.Points.Contains(neighbour)) continue;

                // Calculate the new cost for this neighbour
                float additionalCost = grid.GetCostOfEnteringCell(neighbour);
                float newCost = currentPath.Cost + additionalCost;
                float totalCost = newCost + ManhattanDistance(neighbour, endPosition);


                Path newPath = new Path
                {
                    Points = new List<Point>(currentPath.Points) { neighbour },
                    Cost = newCost
                };

                openPaths.Enqueue(newPath, totalCost);
            }
        }

        //Debug.Log("Available Paths: " + paths.Count);
        return paths;

    }

    private static float ManhattanDistance(Point endPos, Point point)
    {
        return Math.Abs(endPos.X - point.X) + Math.Abs(endPos.Y - point.Y);
    }

    public static List<Point> AStarSearch(Grid grid, Point startPosition, Point endPosition, bool isAgent = false)
    {
        List<Point> path = new List<Point>();

        List<Point> positionsTocheck = new List<Point>();
        Dictionary<Point, float> costDictionary = new Dictionary<Point, float>();
        Dictionary<Point, float> priorityDictionary = new Dictionary<Point, float>();
        Dictionary<Point, Point> parentsDictionary = new Dictionary<Point, Point>();

        positionsTocheck.Add(startPosition);
        priorityDictionary.Add(startPosition, 0);
        costDictionary.Add(startPosition, 0);
        parentsDictionary.Add(startPosition, null);

        while (positionsTocheck.Count > 0)
        {
            Point current = GetClosestVertex(positionsTocheck, priorityDictionary);
            positionsTocheck.Remove(current);
            if (current.Equals(endPosition))
            {
                path = GeneratePath(parentsDictionary, current);
                return path;
            }

            foreach (Point neighbour in grid.GetAdjacentCells(current, isAgent))
            {
                float newCost = costDictionary[current] + grid.GetCostOfEnteringCell(neighbour);
                if (!costDictionary.ContainsKey(neighbour) || newCost < costDictionary[neighbour])
                {
                    costDictionary[neighbour] = newCost;

                    float priority = newCost + ManhattanDistance(endPosition, neighbour);
                    positionsTocheck.Add(neighbour);
                    priorityDictionary[neighbour] = priority;

                    parentsDictionary[neighbour] = current;
                }
            }
        }
        return path;
    }

    private static Point GetClosestVertex(List<Point> list, Dictionary<Point, float> distanceMap)
    {
        Point candidate = list[0];
        foreach (Point vertex in list)
        {
            if (distanceMap[vertex] < distanceMap[candidate])
            {
                candidate = vertex;
            }
        }
        return candidate;
    }


    public static List<Point> GeneratePath(Dictionary<Point, Point> parentMap, Point endState)
    {
        List<Point> path = new List<Point>();
        Point parent = endState;
        while (parent != null && parentMap.ContainsKey(parent))
        {
            path.Add(parent);
            parent = parentMap[parent];
        }
        return path;
    }
}