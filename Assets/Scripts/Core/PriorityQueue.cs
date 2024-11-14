using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T Item, float Priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((x, y) => x.Priority.CompareTo(y.Priority));
    }

    public T Dequeue()
    {
        var bestItem = elements[0].Item;
        elements.RemoveAt(0);
        return bestItem;
    }
}
