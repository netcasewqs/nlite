using System;
namespace NLite.Net
{
    public interface IHttpRequest
    {
        string[] AcceptTypes { get; }
        string AnonymousID { get; }
        string ApplicationPath { get; }
        string AppRelativeCurrentExecutionFilePath { get; }
        byte[] BinaryRead(int count);
        IHttpBrowserCapabilities Browser { get; }
        //System.Web.HttpClientCertificate ClientCertificate { get; }
        System.Text.Encoding ContentEncoding { get; set; }
        int ContentLength { get; }
        string ContentType { get; set; }
        IHttpCookieCollection Cookies { get; }
        string CurrentExecutionFilePath { get; }
        string FilePath { get; }
        IHttpFileCollection Files { get; }
        System.IO.Stream Filter { get; set; }
        System.Collections.Specialized.NameValueCollection Form { get; }
        System.Collections.Specialized.NameValueCollection Headers { get; }
        string HttpMethod { get; }
        System.IO.Stream InputStream { get; }
        bool IsAuthenticated { get; }
        bool IsLocal { get; }
        bool IsSecureConnection { get; }
        System.Security.Principal.WindowsIdentity LogonUserIdentity { get; }
        int[] MapImageCoordinates(string imageFieldName);
        string MapPath(string virtualPath);
        string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping);
        System.Collections.Specialized.NameValueCollection Params { get; }
        string Path { get; }
        string PathInfo { get; }
        string PhysicalApplicationPath { get; }
        string PhysicalPath { get; }
        System.Collections.Specialized.NameValueCollection QueryString { get; }
        string RawUrl { get; }
        string RequestType { get; set; }
        void SaveAs(string filename, bool includeHeaders);
        System.Collections.Specialized.NameValueCollection ServerVariables { get; }
        string this[string key] { get; }
        int TotalBytes { get; }
        Uri Url { get; }
        Uri UrlReferrer { get; }
        string UserAgent { get; }
        string UserHostAddress { get; }
        string UserHostName { get; }
        string[] UserLanguages { get; }
        void ValidateInput();
    }
}
