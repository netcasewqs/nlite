using System;
using System.Collections;
namespace NLite.Net
{
    public interface IHttpApplicationState : ICollection, IEnumerable
    {
        void Add(string name, object value);
        string[] AllKeys { get; }
        void Clear();
        IHttpApplicationState Contents { get; }
        object Get(int index);
        object Get(string name);
        string GetKey(int index);
        void Lock();
        void Remove(string name);
        void RemoveAll();
        void RemoveAt(int index);
        void Set(string name, object value);
        IHttpStaticObjectsCollection StaticObjects { get; }
        object this[int index] { get; }
        object this[string name] { get; set; }
        void UnLock();
    }
}
