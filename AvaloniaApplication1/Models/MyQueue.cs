using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Models;

public class MyQueue<T>
{
    private readonly Queue<T> _queue;

    public MyQueue()
    {
        _queue = new Queue<T>();
    }

    public T Current
    {
        get
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _queue.Peek();
        }
    }

    public int Count => _queue.Count;

    public bool IsEmpty => _queue.Count == 0;

    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
    }

    public T Dequeue()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Queue is empty.");
        }

        return _queue.Dequeue();
    }

    public void Clear()
    {
        _queue.Clear();
    }
}