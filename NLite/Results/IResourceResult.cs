using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NLite.Net;
using NLite.Domain;
using NLite.Domain.Mapping;
using System.Net.Mime;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NLite.Results
{
    public interface IResourceResultAdapter
    {
        void Execute(INavigationResult nav, IHttpContext context);
    }

    //public class ResourceResult
    //{
    //    private static IDictionary<string, IResourceResultAdapter> Adapters = new DictionaryWrapper<IResourceResultAdapter>();
    //    public static Func<object,string> JsonSerialize { get; set; }

    //    static ResourceResult()
    //    {
    //        Adapters[ActionResultTypes.Content] = new ContentResultAdapter();
    //        Adapters[ActionResultTypes.FileContent] = new FileContentResultAdapter();
    //        Adapters[ActionResultTypes.FilePath] = new FilePathResultAdapter();
    //        Adapters[ActionResultTypes.FileStream] = new FileStreamResultAdapter();
    //        Adapters[ActionResultTypes.JavaScript] = new JavaScriptResultAdapter();
    //        Adapters[ActionResultTypes.Json] = new JsonResultAdapter();
    //        Adapters[ActionResultTypes.Xml] = new XmlResultAdapter();
    //        //Adapters[ActionResultTypes.PartialView] = new PartialViewResultAdapter();
    //        Adapters[ActionResultTypes.Redirect] = new RedirectResultAdapter();
    //        Adapters[ActionResultTypes.Status] = new StatusResultAdapter();
    //        Adapters[ActionResultTypes.Empty] = new EmptyResultAdapter();
    //        //Adapters[ActionResultTypes.RedirectToAction] = new RedirectToActionResultAdapter();
    //        //Adapters[ActionResultTypes.RedirectToRoute] = new RedirectToRouteResultAdapter();
    //        //Adapters[ActionResultTypes.View] = new ViewResultAdapter();
    //    }

    //    public static void Register(string type, IResourceResultAdapter adapter)
    //    {
    //        Adapters[type] = adapter;
    //    }

    //    public static IResourceResultAdapter Get(string type)
    //    {
    //        return Adapters[type];
    //    }

    //    public static void Execute(INavigationResult nav, IHttpContext context)
    //    {
    //        if (nav == null)
    //            return;
    //        if (Adapters.ContainsKey(nav.Type))
    //            Adapters[nav.Type].Execute(nav, context);
    //        else
    //            throw new NotSupportedException(string.Format("Not support {0} type resource result", nav.Type)); 
    //    }


           
    //    class ContentResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            var info = nav as ContentResult;
    //            var response = context.Response;
    //            if (!string.IsNullOrEmpty(info.ContentType))
    //            {
    //                response.ContentType = info.ContentType;
    //            }
    //            if (info.ContentEncoding != null)
    //            {
    //                response.ContentEncoding = info.ContentEncoding;
    //            }
    //            if (info.Content != null)
    //            {
    //                response.Write(info.Content);
    //            }
    //        }
    //    }

    //    class FileContentResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            var info = nav as FileContentResult;
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var response = context.Response;
    //            response.ContentType = info.ContentType;
    //            if (!String.IsNullOrEmpty(info.FileDownloadName))
    //            {
    //                ContentDisposition disposition = new ContentDisposition() { FileName = info.FileDownloadName };
    //                string headerValue = disposition.ToString();
    //                response.AddHeader("Content-Disposition", headerValue);
    //            }

    //            response.OutputStream.Write(info.Content, 0, info.Content.Length);
    //        }

    //    }

    //    class FileStreamResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            var info = nav as FileStreamResult;
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var response = context.Response;
    //            response.ContentType = info.ContentType;
    //            if (!String.IsNullOrEmpty(info.FileDownloadName))
    //            {
    //                ContentDisposition disposition = new ContentDisposition() { FileName = info.FileDownloadName };
    //                string headerValue = disposition.ToString();
    //                response.AddHeader("Content-Disposition", headerValue);
    //            }

    //            var outputStream = response.OutputStream;
    //            using (info.Stream)
    //            {
    //                byte[] buffer = new byte[4096];
    //                while (true)
    //                {
    //                    int num = info.Stream.Read(buffer, 0, 4096);
    //                    if (num == 0)
    //                    {
    //                        break;
    //                    }
    //                    outputStream.Write(buffer, 0, num);
    //                }
    //            }
    //        }

    //    }

    //    class FilePathResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            var info = nav as FilePathResult;
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var response = context.Response;
    //            response.ContentType = info.ContentType;
    //            if (!String.IsNullOrEmpty(info.FileDownloadName))
    //            {
    //                ContentDisposition disposition = new ContentDisposition() { FileName = info.FileDownloadName };
    //                string headerValue = disposition.ToString();
    //                response.AddHeader("Content-Disposition", headerValue);
    //            }

    //            response.WriteFile(info.Path, true);
    //        }

    //    }

    //    class JavaScriptResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var info = nav as JavaScriptResult;
    //            var response = context.Response;
    //            response.ContentType = "application/x-javascript";
    //            if (info.Script != null)
    //                response.Write(info.Script);
    //        }
    //    }

    //    class JsonResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            if (context == null)
    //            {
    //                throw new ArgumentNullException("context");
    //            }

    //            var info = nav as JsonResult;
    //            if (!info.AllowGet && string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
    //            {
    //                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
    //            }
    //            var response = context.Response;
    //            if (!string.IsNullOrEmpty(info.ContentType))
    //                response.ContentType = info.ContentType;
    //            else
    //                response.ContentType = "application/json";
    //            if (info.ContentEncoding != null)
    //                response.ContentEncoding = info.ContentEncoding;

    //            if (JsonSerialize != null)
    //                response.Write(JsonSerialize(info.Data));
    //        }
    //    }

    //    class XmlResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            if (context == null)
    //            {
    //                throw new ArgumentNullException("context");
    //            }

    //            var info = nav as JsonResult;
    //            if (!info.AllowGet && string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
    //            {
    //                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
    //            }
    //            var response = context.Response;
    //            if (!string.IsNullOrEmpty(info.ContentType))
    //                response.ContentType = info.ContentType;
    //            else
    //                response.ContentType = "application/xml";
    //            if (info.ContentEncoding != null)
    //                response.ContentEncoding = info.ContentEncoding;

    //            if (info.Data != null)
    //            {
    //                if (info.Data is XmlNode)
    //                    response.Write(((XmlNode)info.Data).OuterXml);
    //                else if (info.Data is XNode)
    //                    response.Write(((XNode)info.Data).ToString());
    //                else
    //                {
    //                    var dataType = info.Data.GetType();
    //                    if (dataType.GetCustomAttributes(typeof(DataContractAttribute), true).FirstOrDefault() != null)
    //                    {
    //                        var dSer = new DataContractSerializer(dataType);
    //                        dSer.WriteObject(response.OutputStream, info.Data);
    //                    }
    //                    else
    //                    {
    //                        var xSer = new XmlSerializer(dataType);
    //                        xSer.Serialize(response.OutputStream, info.Data);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    class RedirectResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var info = nav as RedirectResult;
    //            context.Response.Redirect(info.Url, true);
    //        }
    //    }
        
    //    class StatusResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var info = nav as StatusResult;
    //            if (!string.IsNullOrEmpty(info.Body))
    //                context.Response.Write(info.Body);
    //            context.Response.StatusCode = info.StatusCode;
    //        }
    //    }

    //    class EmptyResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //        }
    //    }

    //    class ViewResultAdapter : IResourceResultAdapter
    //    {
    //        public void Execute(INavigationResult nav, IHttpContext context)
    //        {
    //            if (context == null)
    //                throw new ArgumentNullException("context");

    //            var info = nav as ViewResult;
    //            var view = ViewEngines.ViewEngine.Current.Get(info.ViewName);
    //            view.Render(context.Response.Output, info.Model);
    //        }
    //    }

        
    //}

   
}
