using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NLite.Domain.Mapping
{
    public class ActionResultTypes
    {
        public const string
            Content = "Content",
            View = "View",
            FileContent = "FileContent",
            FileStream = "FileStream",
            FilePath = "FilePath",
            JavaScript = "JavaScript",
            Json = "Json",
            Xml = "Xml",
            PartialView = "PartialView",
            Redirect = "Redirect",
            RedirectToAction = "RedirectToAction",
            RedirectToRoute = "RedirectToRoute",
            Status = "Status",
            Empty = "",
            Error = "Error"
            ;
    }

    public interface IViewResult : INavigationResult
    {
        string ViewName { get; }
        string MasterName { get; }
        object Model { get; }
    }

    public interface IContentResult : INavigationResult
    {
        string Content { get; }
        string ContentType { get; }
        Encoding ContentEncoding { get; }
    }

    public interface IFileResult : INavigationResult
    {
        string ContentType { get; }
        string FileDownloadName { get; }
    }
    public interface IFileContentResult : IFileResult
    {
        byte[] Content { get; }
    }
    public interface IFileStreamResult : IFileResult
    {
        Stream Stream { get; }
    }

    public interface IFilePathResult : IFileResult
    {
        string Path { get; }
    }

    public interface IJavaScriptResult : INavigationResult
    {
        string Script { get; }
    }

    public interface IJsonResult : INavigationResult
    {
        object Data { get; }
        string ContentType { get; }
        Encoding ContentEncoding { get; }
        bool AllowGet { get; }
    }

    public interface IPartialViewResult : INavigationResult
    {
        string ViewName { get; }
        object Model { get; }
    }

    public interface IStatusResult: INavigationResult
    {
        int StatusCode { get; }
        string Body { get; }
    }

    public interface IRedirectResult : INavigationResult
    {
        string Url { get; }
    }

    public interface IRedirectToActionResult : INavigationResult
    {
        string ActionName { get; }
        string ControllerName { get; }
        object RouteValues { get; }
    }

    public interface IRedirectToRouteResult : INavigationResult
    {
        string RouteName { get; }
        object RouteValues { get; }
    }

   
}
