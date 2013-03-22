using System;
using System.Collections;
using System.Collections.Generic;
namespace NLite.Net
{
    public interface IHttpSessionState : ICollection, IEnumerable
    {
        void Abandon();
        void Add(string name, object value);
        void Clear();
        //int CodePage { get; set; }
        //IHttpSessionState Contents { get; }
        //System.Web.HttpCookieMode CookieMode { get; }
      
        //bool IsCookieless { get; }
        //bool IsNewSession { get; }
        //bool IsReadOnly { get; }
        IEnumerable<string> Keys { get; }
        //int LCID { get; set; }
        //System.Web.SessionState.SessionStateMode Mode { get; }
        void Remove(string name);
        void RemoveAll();
        //void RemoveAt(int index);
        string SessionID { get; }
        //IHttpStaticObjectsCollection StaticObjects { get; }
        object this[string name] { get; set; }
        int Timeout { get; set; }
    }
}
