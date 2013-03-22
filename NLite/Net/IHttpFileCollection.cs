using System;
using System.Collections;
namespace NLite.Net
{
    public interface IHttpFileCollection : ICollection, IEnumerable
    {
        string[] AllKeys { get; }
        IHttpPostedFile Get(int index);
        IHttpPostedFile Get(string name);
        string GetKey(int index);
        IHttpPostedFile this[int index] { get; }
        IHttpPostedFile this[string name] { get; }
    }
}
