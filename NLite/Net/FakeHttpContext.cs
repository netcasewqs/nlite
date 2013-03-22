using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Configuration;
using System.IO;
using System.Collections;
using NLite.Collections;
using System.Diagnostics;
using NLite.Cache;

namespace NLite.Net
{
    public class FakeHttpRequest : IHttpRequest
    {
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;
        private readonly IHttpCookieCollection _cookies;
        private Uri _uri;
        private NameValueCollection _headers = new NameValueCollection();

        public FakeHttpRequest(string url, NameValueCollection formParams, NameValueCollection queryStringParams, IHttpCookieCollection cookies)
        {
            _uri = new Uri(url);
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
        }

        public  NameValueCollection Form
        {
            get
            {
                return _formParams;
            }
        }

        public  NameValueCollection QueryString
        {
            get
            {
                return _queryStringParams;
            }
        }

        public  NameValueCollection Headers
        {
            get
            {
                return _headers;
            }
        }

        public  IHttpCookieCollection Cookies
        {
            get
            {
                return _cookies;
            }
        }

        public  string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return "~" + this.Url.AbsolutePath;
            }
        }

        public  string PathInfo
        {
            get
            {
                return "";
            }
        }

        public  string ApplicationPath
        {
            get
            {
                return "/";
            }
        }

        public  Uri Url
        {
            get
            {
                return _uri;
            }
        }

        public void SetUri(string url)
        {
            this._uri = new Uri(url);
        }

        public  string UserHostAddress
        {
            get
            {
                return "127.0.0.1";
            }
        }

        public  string HttpMethod
        {
            get
            {
                return "GET";
            }
        }


        public string[] AcceptTypes
        {
            get { throw new NotImplementedException(); }
        }

        public string AnonymousID
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] BinaryRead(int count)
        {
            throw new NotImplementedException();
        }

        public IHttpBrowserCapabilities Browser
        {
            get { throw new NotImplementedException(); }
        }

        public Encoding ContentEncoding
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int ContentLength
        {
            get { throw new NotImplementedException(); }
        }

        public string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentExecutionFilePath
        {
            get { throw new NotImplementedException(); }
        }

        public string FilePath
        {
            get { throw new NotImplementedException(); }
        }

        public IHttpFileCollection Files
        {
            get { throw new NotImplementedException(); }
        }

        public Stream Filter
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Stream InputStream
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAuthenticated
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsLocal
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSecureConnection
        {
            get { throw new NotImplementedException(); }
        }

        public WindowsIdentity LogonUserIdentity
        {
            get { throw new NotImplementedException(); }
        }

        public int[] MapImageCoordinates(string imageFieldName)
        {
            throw new NotImplementedException();
        }

        public string MapPath(string virtualPath)
        {
            throw new NotImplementedException();
        }

        public string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            throw new NotImplementedException();
        }

        public NameValueCollection Params
        {
            get { throw new NotImplementedException(); }
        }

        public string Path
        {
            get { throw new NotImplementedException(); }
        }

        public string PhysicalApplicationPath
        {
            get { throw new NotImplementedException(); }
        }

        public string PhysicalPath
        {
            get { throw new NotImplementedException(); }
        }

        public string RawUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string RequestType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SaveAs(string filename, bool includeHeaders)
        {
            throw new NotImplementedException();
        }

        public NameValueCollection ServerVariables
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        public int TotalBytes
        {
            get { throw new NotImplementedException(); }
        }

        public Uri UrlReferrer
        {
            get { throw new NotImplementedException(); }
        }

        public string UserAgent
        {
            get { throw new NotImplementedException(); }
        }

        public string UserHostName
        {
            get { throw new NotImplementedException(); }
        }

        public string[] UserLanguages
        {
            get { throw new NotImplementedException(); }
        }

        public void ValidateInput()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeHttpResponse : IHttpResponse
    {
        private StringBuilder sb;
        private StringWriter sw;

        public FakeHttpResponse()
        {
            sb = new StringBuilder(1000);
            sw = new StringWriter(sb);
        }
        public  void End()
        {

        }

        public  int StatusCode
        {
            get;
            set;
        }

        public  void Clear()
        {
            sb.Length = 0;
        }

        public  void AddHeader(string name, string value)
        {
            this.Headers[name] = value;
        }

        public  string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }

        private NameValueCollection _headers = new NameValueCollection();
        public  NameValueCollection Headers
        {
            get
            {
                return _headers;
            }
        }

        private IHttpCookieCollection _cookies = new FakeHttpCookieCollection();
        public  IHttpCookieCollection Cookies
        {
            get
            {
                return _cookies;
            }
        }

        public  IHttpCachePolicy Cache
        {
            get
            {
                return new FakeHttpCachePolicy();
            }
        }

        public  string ContentType
        {
            get
            {
                return "text/dummy";
            }
            set
            {

            }
        }


        public void AppendCookie(IHttpCookie cookie)
        {
            _cookies.Add(cookie);
        }

        public void AppendHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public void AppendToLog(string param)
        {
        }

        public void BinaryWrite(byte[] buffer)
        {
        }

        public bool Buffer { get; set; }

        public bool BufferOutput { get; set; }

        public string CacheControl { get; set; }

        public string Charset { get; set; }

        public void ClearContent()
        {
        }

        public void ClearHeaders()
        {
        }

        public void Close()
        {
        }

        public Encoding ContentEncoding { get; set; }

        public void DisableKernelCache()
        {
        }

        public int Expires { get; set; }

        public DateTime ExpiresAbsolute { get; set; }

        public Stream Filter { get; set; }

        public void Flush()
        {
        }

        public Encoding HeaderEncoding{get;set;}

        public bool IsClientConnected{get;set;}

        public bool IsRequestBeingRedirected { get; set; }

        public TextWriter Output
        {
            get { return sw; }
        }

        public Stream OutputStream
        {
            get { throw new NotImplementedException(); }
        }

        public void Pics(string value)
        {
            throw new NotImplementedException();
        }

        public void Redirect(string url)
        {
        }

        public void Redirect(string url, bool endResponse)
        {
        }

        public string RedirectLocation { get; set; }

        public void RemoveOutputCacheItem(string path)
        {
        }

        public void SetCookie(IHttpCookie cookie)
        {
            _cookies.Set(cookie);
        }

        public string Status { get; set; }

        public string StatusDescription { get; set; }

        public int SubStatusCode { get; set; }

        public bool SuppressContent { get; set; }

        public void TransmitFile(string filename)
        {
        }

        public void TransmitFile(string filename, long offset, long length)
        {
        }

        public bool TrySkipIisCustomErrors { get; set; }

        public void Write(char ch)
        {
            sb.Append(ch);
        }

        public void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public void Write(object obj)
        {
            throw new NotImplementedException();
        }

        public void Write(string s)
        {
           sb.Append(s);
        }

        public void WriteFile(IntPtr fileHandle, long offset, long size)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string filename)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string filename, bool readIntoMemory)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string filename, long offset, long size)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeHttpCachePolicy : IHttpCachePolicy
    {
        public FakeHttpCachePolicy()
            : base()
        {

        }

        public  void SetMaxAge(TimeSpan delta)
        {

        }

        public  void SetProxyMaxAge(TimeSpan delta)
        {

        }

        public void SetETag(string etag)
        {
        }

        public void SetETagFromFileDependencies()
        {
        }

        public void SetExpires(DateTime date)
        {
        }

        public void SetLastModified(DateTime date)
        {
        }

        public void SetLastModifiedFromFileDependencies()
        {
        }

        public void SetNoServerCaching()
        {
        }

        public void SetNoStore()
        {
        }

        public void SetNoTransforms()
        {
        }

        public void SetOmitVaryStar(bool omit)
        {
        }

        public void SetSlidingExpiration(bool slide)
        {
        }

        public void SetValidUntilExpires(bool validUntilExpires)
        {
        }

        public void SetVaryByCustom(string custom)
        {
        }
    }

    public class FakeHttpServerUtility : IHttpServerUtility
    {
        private string _applicationRootConfigurationPath;

        public FakeHttpServerUtility()
            : base()
        {

        }

        public  string MachineName
        {
            get
            {
                return "Fake-MachineName";
            }
        }

        public  int ScriptTimeout
        {
            get
            {
                return 1000;
            }
            set
            {

            }
        }


        //Assign a base path at appSettings (key=FakeApplicationRoot) of the app.config file
        public  string MapPath(string path)
        {
            if (_applicationRootConfigurationPath == null)
            {
                string localConfig = ConfigurationManager.AppSettings["FakeApplicationRoot"];
                localConfig = localConfig == null ? "" : localConfig;
                System.Configuration.Configuration applicationConfiguration = ConfigurationManager.OpenExeConfiguration("");
                _applicationRootConfigurationPath = Path.Combine(Path.GetDirectoryName(applicationConfiguration.FilePath), localConfig);
            }
            if (path.StartsWith("~/"))
            {
                path = path.Substring(2);
            }
            else if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            path = path.Replace("/", "\\");

            return Path.Combine(_applicationRootConfigurationPath, path);
        }

        public void ClearError()
        {
            throw new NotImplementedException();
        }

        public object CreateObject(string progID)
        {
            throw new NotImplementedException();
        }

        public object CreateObject(Type type)
        {
            throw new NotImplementedException();
        }

        public object CreateObjectFromClsid(string clsid)
        {
            throw new NotImplementedException();
        }

        public void Execute(string path)
        {
            throw new NotImplementedException();
        }

        public void Execute(string path, bool preserveForm)
        {
            throw new NotImplementedException();
        }

        public void Execute(string path, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Execute(string path, TextWriter writer, bool preserveForm)
        {
            throw new NotImplementedException();
        }

        public Exception GetLastError()
        {
            throw new NotImplementedException();
        }

        public string HtmlDecode(string s)
        {
            throw new NotImplementedException();
        }

        public void HtmlDecode(string s, TextWriter output)
        {
            throw new NotImplementedException();
        }

        public string HtmlEncode(string s)
        {
            throw new NotImplementedException();
        }

        public void HtmlEncode(string s, TextWriter output)
        {
            throw new NotImplementedException();
        }

        public void Transfer(string path)
        {
            throw new NotImplementedException();
        }

        public void Transfer(string path, bool preserveForm)
        {
            throw new NotImplementedException();
        }

        public void TransferRequest(string path)
        {
            throw new NotImplementedException();
        }

        public void TransferRequest(string path, bool preserveForm)
        {
            throw new NotImplementedException();
        }

        public void TransferRequest(string path, bool preserveForm, string method, NameValueCollection headers)
        {
            throw new NotImplementedException();
        }

        public string UrlDecode(string s)
        {
            throw new NotImplementedException();
        }

        public void UrlDecode(string s, TextWriter output)
        {
            throw new NotImplementedException();
        }

        public string UrlEncode(string s)
        {
            throw new NotImplementedException();
        }

        public void UrlEncode(string s, TextWriter output)
        {
            throw new NotImplementedException();
        }

        public string UrlPathEncode(string s)
        {
            throw new NotImplementedException();
        }

        public byte[] UrlTokenDecode(string input)
        {
            throw new NotImplementedException();
        }

        public string UrlTokenEncode(byte[] input)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeHttpSessionState : IHttpSessionState
    {
        private readonly Dictionary<string,object> _sessionItems;

        public FakeHttpSessionState(Dictionary<string, object> sessionItems)
        {
            _sessionItems = sessionItems;
        }

        public  void Add(string name, object value)
        {
            _sessionItems[name] = value;
        }

        public  int Count
        {
            get
            {
                return _sessionItems.Count;
            }
        }

        public  IEnumerator GetEnumerator()
        {
            return _sessionItems.GetEnumerator();
        }

        public  IEnumerable<string> Keys
        {
            get
            {
                return _sessionItems.Keys;
            }
        }

        public  object this[string name]
        {
            get
            {
                return _sessionItems[name];
            }
            set
            {
                _sessionItems[name] = value;
            }
        }


        public  void Remove(string name)
        {
            _sessionItems.Remove(name);
        }

        public void Abandon()
        {
        }

        public void Clear()
        {
            _sessionItems.Clear();
        }

        public void RemoveAll()
        {
            _sessionItems.Clear();
        }

        public string SessionID
        {
            get { return ""; }
        }

        public int Timeout
        {
            get
            {
               return 0;
            }
            set
            {
                ;
            }
        }

        public void CopyTo(Array array, int index)
        {
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class FakeIdentity : IIdentity
    {
        private readonly string _name;

        public FakeIdentity(string userName)
        {
            _name = userName;

        }

        public string AuthenticationType
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsAuthenticated
        {
            get { return !String.IsNullOrEmpty(_name); }
        }

        public string Name
        {
            get { return _name; }
        }

    }

    public class FakePrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        private readonly string[] _roles;

        public FakePrincipal(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = roles;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            if (_roles == null)
                return false;
            return _roles.Contains(role);
        }
    }

    public class FakeHttpContext : IHttpContext
    {
        private readonly FakePrincipal _principal;
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;
        private readonly IHttpCookieCollection _cookies;
        private readonly string _url;
        private FakeHttpRequest _request;

        public FakeHttpContext(string url, FakePrincipal principal, NameValueCollection formParams, NameValueCollection queryStringParams, IHttpCookieCollection cookies, Dictionary<string,object> sessionItems)
        {
            _url = url;
            _principal = principal;
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
            _session = new FakeHttpSessionState(sessionItems);
            _request = new FakeHttpRequest(_url, _formParams, _queryStringParams, _cookies);
            _server = new FakeHttpServerUtility();
            _items = new Dictionary<string, object>();
        }

        public FakeHttpContext(string url)
            : this(url, null, new NameValueCollection(), new NameValueCollection(), new FakeHttpCookieCollection(), new Dictionary<string, object>())
        {

        }

        public  IHttpRequest Request
        {
            get
            {
                return _request;
            }
        }

        public  IPrincipal User
        {
            get
            {
                return _principal;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        private IHttpSessionState _session;
        public  IHttpSessionState Session
        {
            get
            {
                return _session;
            }
        }

        private FakeHttpResponse _response = new FakeHttpResponse();
        public  IHttpResponse Response
        {
            get
            {
                return _response;
            }
        }

        public  ICache Cache
        {
            get
            {
                return NLiteEnvironment.Cache;
            }
        }

        private FakeHttpServerUtility _server;
        public  IHttpServerUtility Server
        {
            get
            {
                return _server;
            }
        }

        private System.Collections.IDictionary _items;
        public  System.Collections.IDictionary Items
        {
            get
            {
                return _items;
            }
        }

        public void AddError(Exception errorInfo)
        {
            throw new NotImplementedException();
        }

        public Exception[] AllErrors
        {
            get { throw new NotImplementedException(); }
        }

       
        public void ClearError()
        {
            throw new NotImplementedException();
        }

        public Exception Error
        {
            get { throw new NotImplementedException(); }
        }

        public object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            throw new NotImplementedException();
        }

        public object GetGlobalResourceObject(string classKey, string resourceKey, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object GetLocalResourceObject(string virtualPath, string resourceKey)
        {
            throw new NotImplementedException();
        }

        public object GetLocalResourceObject(string virtualPath, string resourceKey, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object GetSection(string sectionName)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomErrorEnabled
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDebuggingEnabled
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPostNotification
        {
            get { throw new NotImplementedException(); }
        }

        public void RewritePath(string filePath, string pathInfo, string queryString)
        {
            throw new NotImplementedException();
        }

        public void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        {
            throw new NotImplementedException();
        }

        public void RewritePath(string path)
        {
            throw new NotImplementedException();
        }

        public void RewritePath(string path, bool rebaseClientPath)
        {
            throw new NotImplementedException();
        }

      

        public bool SkipAuthorization
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime Timestamp
        {
            get { throw new NotImplementedException(); }
        }


        public IHttpApplicationState Application
        {
            get { throw new NotImplementedException(); }
        }
    }

    class FakeHttpCookie : IHttpCookie
    {
        public FakeHttpCookie()
        {
            Values = new NameValueCollection();
        }
        public string Domain { get; set; }

        public DateTime Expires { get; set; }

        public bool HasKeys
        {
            get { throw new NotImplementedException(); }
        }

        public bool HttpOnly { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public bool Secure { get; set; }

        public string Value { get; set; }

        public NameValueCollection Values { get; private set; }

        public string this[string key]
        {
            get
            {
                return Values[key];
            }
            set
            {
                Values[key]=value;
            }
        }
    }

    class FakeHttpCookieCollection : IHttpCookieCollection
    {
        private List<IHttpCookie> Items = new List<IHttpCookie>();

        public string[] AllKeys
        {
            get { return Items.Where(p=>!string.IsNullOrEmpty(p.Name)).Select(p=>p.Name).ToArray(); }
        }

        public IHttpCookie this[string name]
        {
            get { return Items.FirstOrDefault(p=>p.Name == name); }
        }

        public void Add(IHttpCookie cookie)
        {
            if (cookie != null)
                Items.Add(cookie);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public IHttpCookie Get(string name)
        {
            return this[name];
        }

        public void Remove(string name)
        {
            var item = this[name];
            if (item != null)
                Items.Remove(item);
        }

        public void Set(IHttpCookie cookie)
        {
            if (cookie == null)
                return;
            Remove(cookie.Name);
            Add(cookie);
        }

        public IEnumerator<IHttpCookie> GetEnumerator()
        {
           return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
