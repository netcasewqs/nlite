using System;
using System.Collections;
namespace NLite.Net
{
    public interface IHttpStaticObjectsCollection : ICollection, IEnumerable
    {
        object GetObject(string name);
        bool IsReadOnly { get; }
        bool NeverAccessed { get; }
        void Serialize(System.IO.BinaryWriter writer);
        object this[string name] { get; }
    }
}
