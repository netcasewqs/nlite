using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace NLite.Net
{
    public interface IHttpCookie
    {
        string Domain { get; set; }
        DateTime Expires { get; set; }
        bool HasKeys { get; }
        bool HttpOnly { get; set; }
        string Name { get; set; }
        string Path { get; set; }
        bool Secure { get; set; }
        string Value { get; set; }
        NameValueCollection Values { get; }
        string this[string key] { get; set; }
    }
}
