using System;
namespace NLite.Net
{
    public interface IHttpServerUtility
    {
        void ClearError();
        object CreateObject(string progID);
        object CreateObject(Type type);
        object CreateObjectFromClsid(string clsid);
        void Execute(string path);
        void Execute(string path, bool preserveForm);
        void Execute(string path, System.IO.TextWriter writer);
        void Execute(string path, System.IO.TextWriter writer, bool preserveForm);
        //void Execute(System.Web.IHttpHandler handler, System.IO.TextWriter writer, bool preserveForm);
        Exception GetLastError();
        string HtmlDecode(string s);
        void HtmlDecode(string s, System.IO.TextWriter output);
        string HtmlEncode(string s);
        void HtmlEncode(string s, System.IO.TextWriter output);
        string MachineName { get; }
        string MapPath(string path);
        int ScriptTimeout { get; set; }
        void Transfer(string path);
        void Transfer(string path, bool preserveForm);
        //void Transfer(System.Web.IHttpHandler handler, bool preserveForm);
        void TransferRequest(string path);
        void TransferRequest(string path, bool preserveForm);
        void TransferRequest(string path, bool preserveForm, string method, System.Collections.Specialized.NameValueCollection headers);
        string UrlDecode(string s);
        void UrlDecode(string s, System.IO.TextWriter output);
        string UrlEncode(string s);
        void UrlEncode(string s, System.IO.TextWriter output);
        string UrlPathEncode(string s);
        byte[] UrlTokenDecode(string input);
        string UrlTokenEncode(byte[] input);
    }
}
