using System.Collections.Generic;
using System;

namespace NLite.Collections.Internal
{
    interface IQueue<T> : IEnumerable<T>,IDisposable
    {
        int Count { get; }
        T Dequeue();
        void Enqueue(T item);
    }
}
