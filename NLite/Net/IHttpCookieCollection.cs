using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace NLite.Net
{
    public interface IHttpCookieCollection:IEnumerable<IHttpCookie>
    {
        string[] AllKeys { get; }
        IHttpCookie this[string name] { get; }
        void Add(IHttpCookie cookie);
        void Clear();
        IHttpCookie Get(string name);
        void Remove(string name);
        void Set(IHttpCookie cookie);
    }
}
