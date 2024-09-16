using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
/// </summary>
public class GridSearch {

    public struct SearchResult
    {
        public List<Point> Path { get; set; }
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

                    float randomFactor = UnityEngine.Random.Range(0f, 0.5f); // Slight randomization
                    float heuristic = ManhattanDistance(neighbour, endPosition);
                    float priority = newCost + heuristic * (1.0f + randomFactor);
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
