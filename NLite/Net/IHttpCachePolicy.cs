using System;
namespace NLite.Net
{
    public interface IHttpCachePolicy
    {
        //void AddValidationCallback(System.Web.HttpCacheValidateHandler handler, object data);
        //void AppendCacheExtension(string extension);
        //void SetAllowResponseInBrowserHistory(bool allow);
        //void SetCacheability(System.Web.HttpCacheability cacheability);
        //void SetCacheability(System.Web.HttpCacheability cacheability, string field);
        void SetETag(string etag);
        void SetETagFromFileDependencies();
        void SetExpires(DateTime date);
        void SetLastModified(DateTime date);
        void SetLastModifiedFromFileDependencies();
        void SetMaxAge(TimeSpan delta);
        void SetNoServerCaching();
        void SetNoStore();
        void SetNoTransforms();
        void SetOmitVaryStar(bool omit);
        void SetProxyMaxAge(TimeSpan delta);
        //void SetRevalidation(System.Web.HttpCacheRevalidation revalidation);
        void SetSlidingExpiration(bool slide);
        void SetValidUntilExpires(bool validUntilExpires);
        void SetVaryByCustom(string custom);
        //System.Web.HttpCacheVaryByContentEncodings VaryByContentEncodings { get; }
        //System.Web.HttpCacheVaryByHeaders VaryByHeaders { get; }
        //System.Web.HttpCacheVaryByParams VaryByParams { get; }
    }
}
