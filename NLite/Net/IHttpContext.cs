using System;
using NLite.Collections;
using NLite.Cache;
namespace NLite.Net
{
    public interface IHttpContext 
    {
        void AddError(Exception errorInfo);
        Exception[] AllErrors { get; }
        IHttpApplicationState Application { get; }
        ICache Cache { get; }
        void ClearError();
        Exception Error { get; }
        object GetGlobalResourceObject(string classKey, string resourceKey);
        object GetGlobalResourceObject(string classKey, string resourceKey, System.Globalization.CultureInfo culture);
        object GetLocalResourceObject(string virtualPath, string resourceKey);
        object GetLocalResourceObject(string virtualPath, string resourceKey, System.Globalization.CultureInfo culture);
        object GetSection(string sectionName);
        bool IsCustomErrorEnabled { get; }
        bool IsDebuggingEnabled { get; }
        bool IsPostNotification { get; }
        System.Collections.IDictionary Items { get; }
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }
        void RewritePath(string filePath, string pathInfo, string queryString);
        void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath);
        void RewritePath(string path);
        void RewritePath(string path, bool rebaseClientPath);
        IHttpServerUtility Server { get; }
        IHttpSessionState Session { get; }
        bool SkipAuthorization { get; set; }
        DateTime Timestamp { get; }
        System.Security.Principal.IPrincipal User { get; set; }
    }
}
