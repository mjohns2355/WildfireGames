using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private int _count = 0; // Tie-breaker for stable sorting
    private readonly SortedSet<(T item, float priority, int index)> _queue = new SortedSet<(T item, float priority, int index)>(Comparer<(T item, float priority, int index)>.Create((a, b) =>
    {
        int result = a.priority.CompareTo(b.priority);
        if (result == 0) result = a.index.CompareTo(b.index); // Use index to break ties
        return result;
    }));

    public void Enqueue(T item, float priority)
    {
        _queue.Add((item, priority, _count++)); // Increment _count to ensure stable sorting
    }

    public T Dequeue()
    {
        var first = _queue.Min;
        _queue.Remove(first);
        return first.item;
    }

    public int Count => _queue.Count;
}
