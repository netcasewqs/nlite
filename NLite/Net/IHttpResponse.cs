using System;
namespace NLite.Net
{
    public interface IHttpResponse
    {
        //void AddCacheDependency(params System.Web.Caching.CacheDependency[] dependencies);
        //void AddCacheItemDependencies(System.Collections.ArrayList cacheKeys);
        //void AddCacheItemDependencies(string[] cacheKeys);
        //void AddCacheItemDependency(string cacheKey);
        //void AddFileDependencies(System.Collections.ArrayList filenames);
        //void AddFileDependencies(string[] filenames);
        //void AddFileDependency(string filename);
        void AddHeader(string name, string value);
        void AppendCookie(IHttpCookie cookie);
        void AppendHeader(string name, string value);
        void AppendToLog(string param);
        string ApplyAppPathModifier(string virtualPath);
        void BinaryWrite(byte[] buffer);
        bool Buffer { get; set; }
        bool BufferOutput { get; set; }
        IHttpCachePolicy Cache { get; }
        string CacheControl { get; set; }
        string Charset { get; set; }
        void Clear();
        void ClearContent();
        void ClearHeaders();
        void Close();
        System.Text.Encoding ContentEncoding { get; set; }
        string ContentType { get; set; }
        IHttpCookieCollection Cookies { get; }
        void DisableKernelCache();
        void End();
        int Expires { get; set; }
        DateTime ExpiresAbsolute { get; set; }
        System.IO.Stream Filter { get; set; }
        void Flush();
        System.Text.Encoding HeaderEncoding { get; set; }
        System.Collections.Specialized.NameValueCollection Headers { get; }
        bool IsClientConnected { get; }
        bool IsRequestBeingRedirected { get; }
        System.IO.TextWriter Output { get; }
        System.IO.Stream OutputStream { get; }
        void Pics(string value);
        void Redirect(string url);
        void Redirect(string url, bool endResponse);
        string RedirectLocation { get; set; }
        void RemoveOutputCacheItem(string path);
        void SetCookie(IHttpCookie cookie);
        string Status { get; set; }
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        int SubStatusCode { get; set; }
        bool SuppressContent { get; set; }
        void TransmitFile(string filename);
        void TransmitFile(string filename, long offset, long length);
        bool TrySkipIisCustomErrors { get; set; }
        void Write(char ch);
        void Write(char[] buffer, int index, int count);
        void Write(object obj);
        void Write(string s);
        void WriteFile(IntPtr fileHandle, long offset, long size);
        void WriteFile(string filename);
        void WriteFile(string filename, bool readIntoMemory);
        void WriteFile(string filename, long offset, long size);
        //void WriteSubstitution(System.Web.HttpResponseSubstitutionCallback callback);
    }
}
