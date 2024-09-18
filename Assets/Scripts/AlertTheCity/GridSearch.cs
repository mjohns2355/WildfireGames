using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
/// </summary>
public class GridSearch {

    public class Path
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public float Cost { get; set; }
    }

    public static List<Path> KShortestPaths(Grid grid, Point startPosition, Point endPosition, int K, bool isAgent = false)
    {
        List<Path> paths = new List<Path>();

        var priorityQueue = new SimplePriorityQueue<Path>();
        var visited = new HashSet<Point>();

        Path startPath = new Path { Points = new List<Point> { startPosition }, Cost = 0 };
        priorityQueue.Enqueue(startPath, 0);
        while (priorityQueue.Count > 0 && paths.Count < K)
        {
            Path currentPath = priorityQueue.Dequeue();
            Point lastPoint = currentPath.Points[currentPath.Points.Count - 1];
            if (lastPoint.Equals(endPosition))
            {
                paths.Add(currentPath);
                visited.Add(endPosition);
                continue;
            }
            
            foreach (Point neighbour in grid.GetAdjacentCells(lastPoint, isAgent))
            {
                if (currentPath.Points.Contains(neighbour)) continue;
                float additionalCost = grid.GetCostOfEnteringCell(neighbour);
                float newCost = currentPath.Cost + additionalCost;
                List<Point> newPathPoints = new List<Point>(currentPath.Points) { neighbour };

                Path newPath = new Path
                {
                    Points = newPathPoints,
                    Cost = newCost
                };

                float heuristic = ManhattanDistance(neighbour, endPosition);
                priorityQueue.Enqueue(newPath, newCost + heuristic);
            }
        }

        Debug.Log("Available Paths: " + paths.Count);
        return paths;

    }
    public static List<Point> AStarSearch(Grid grid, Point startPosition, Point endPosition, bool isAgent = false)
    {
        var priorityQueue = new SimplePriorityQueue<Point>();

        List<Point> path = new List<Point>();
        Dictionary<Point, float> costDictionary = new Dictionary<Point, float>();
        Dictionary<Point, Point> parentsDictionary = new Dictionary<Point, Point>();

        priorityQueue.Enqueue(startPosition, 0);
        costDictionary.Add(startPosition, 0);
        parentsDictionary.Add(startPosition, null);

        while (/*positionsTocheck.Count > 0*/priorityQueue.Count > 0)
        {
            Point current = priorityQueue.Dequeue();
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

                    //float randomFactor = UnityEngine.Random.Range(0f, 0.5f); // Slight randomization
                    float heuristic = ManhattanDistance(neighbour, endPosition);
                    float priority = newCost + heuristic /** (1.0f + randomFactor)*/;
                    if (priorityQueue.Contains(neighbour))
                    {
                        priorityQueue.UpdatePriority(neighbour, priority);
                    }
                    else
                    {
                        priorityQueue.Enqueue(neighbour, priority);
                    }
                    parentsDictionary[neighbour] = current;
                }
            }
        }
        return path;
    }



    //private static Point GetClosestVertex(List<Point> list, Dictionary<Point, float> distanceMap)
    //{
    //    Point candidate = list[0];
    //    foreach (Point vertex in list)
    //    {
    //        if (distanceMap[vertex] < distanceMap[candidate])
    //        {
    //            candidate = vertex;
    //        }
    //    }
    //    return candidate;
    //}

    public static float CalculatePathCost(Grid grid, List<Point> path)
    {
        float totalCost = 0f;
        for (int i = 1; i < path.Count; i++)
        {
            totalCost += grid.GetCostOfEnteringCell(path[i]);
        }
        return totalCost;
    }
    private static float ManhattanDistance(Point endPos, Point point)
    {
        return Math.Abs(endPos.X - point.X) + Math.Abs(endPos.Y - point.Y);
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

    public class SimplePriorityQueue<T>
    {
        private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add(new KeyValuePair<T, float>(item, priority));
            elements.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        public T Dequeue()
        {
            var bestItem = elements[0];
            elements.RemoveAt(0);
            return bestItem.Key;
        }

        public bool Contains(T item)
        {
            return elements.Exists(x => x.Key.Equals(item));
        }

        public void UpdatePriority(T item, float newPriority)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Key.Equals(item))
                {
                    elements[i] = new KeyValuePair<T, float>(item, newPriority);
                    elements.Sort((a, b) => a.Value.CompareTo(b.Value));
                    break;
                }
            }
        }
    }
}