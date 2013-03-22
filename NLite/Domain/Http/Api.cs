
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Net;
using System.Data.Common;
using System.Net;
using NLite.Domain;
using System.IO;
using NLite.Serialization;
using NLite.Data;

namespace NLite
{
    /// <summary>
    /// 服务分发异常解析器
    /// </summary>
    public class ServiceDispatcherExceptionResolver : ExceptionResolver
    {
        /// <summary>
        /// 构造服务分发异常解析器
        /// </summary>
        public ServiceDispatcherExceptionResolver()
        {
            Order = 1;
        }

     
        /// <summary>
        /// 解析异常
        /// </summary>
        /// <param name="ex"></param>
        protected override void OnResolve(Exception ex)
        {
            var serviceDispatcherException = ex as ServiceDispatcherException;
            var code = (int)serviceDispatcherException.ExceptionCode + ExceptionCode.ServiceDispatcherExceptionStart;
            RenderException(code, ex);
        }

        /// <summary>
        /// 判定特定的异常是否是服务分发器异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool HasSupport(Exception ex)
        {
            return ex is ServiceDispatcherException;
        }
    }

    /// <summary>
    /// 领域异常解析器
    /// </summary>
    public class DomainExceptionResolver : ExceptionResolver
    {
        /// <summary>
        /// 构造领域异常解析器
        /// </summary>
        public DomainExceptionResolver()
        {
            Order = 2;
        }

        /// <summary>
        /// 判定特定的异常是否是领域异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool HasSupport(Exception ex)
        {
            return ex is DomainException;
        }

        /// <summary>
        /// 解析异常
        /// </summary>
        /// <param name="ex"></param>
        protected override void OnResolve(Exception ex)
        {
            var domainException = ex as DomainException;
            var code = domainException.ExceptionId + ExceptionCode.DomainExceptionStart;
            RenderException(code, ex);
        }
    }

    /// <summary>
    /// 数据库异常解析器类
    /// </summary>
    public class DbExceptionResolver : ExceptionResolver
    {
        /// <summary>
        /// 构造数据库异常解析器
        /// </summary>
        public DbExceptionResolver()
        {
            Order = 3;
        }

        /// <summary>
        /// 判定特定的异常是否是数据库异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool HasSupport(Exception ex)
        {
            return ex is DbException;
        }

        /// <summary>
        /// 解析异常
        /// </summary>
        /// <param name="ex"></param>
        protected override void OnResolve(Exception ex)
        {
            var queryException = ex as QueryException;
            if (queryException != null)
            {
                RenderException(ExceptionCode.QueryException, ex);
                return;
            }

            var insertException = ex as InsertException;
            if (insertException != null)
            {
                RenderException(ExceptionCode.InsertException, ex);
                return;
            }

            var deleteException = ex as DeleteException;
            if (deleteException != null)
            {
                RenderException(ExceptionCode.DeleteException, ex);
                return;
            }

            var updateException = ex as UpdateException;
            if (updateException != null)
            {
                RenderException(ExceptionCode.UpdateException, ex);
                return;
            }

            var persistenceException = ex as PersistenceException;
            if (persistenceException != null)
            {
                RenderException(ExceptionCode.PersistenceException, ex);
                return;
            }

            var dbException = ex as System.Data.Common.DbException;
            if (dbException != null)
            {
                RenderException(ExceptionCode.DbExceptionStart, ex);
                return;
            }
        }
    }
}


namespace NLite.Domain.Http
{

    /// <summary>
    /// Http 响应结果处理器接口
    /// </summary>
    public interface IHttpResponseResult
    {
        /// <summary>
        /// 输出服务响应结果
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceResponse"></param>
        void Execute(IHttpContext httpContext, IServiceResponse serviceResponse);
    }

    /// <summary>
    /// 服务响应解析器
    /// </summary>
    public interface IServiceResponseResolver
    {
        /// <summary>
        /// 解析服务响应结果
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceResponse"></param>
        /// <returns></returns>
        object Resolve(IHttpContext httpContext, IServiceResponse serviceResponse);
    }


    
    /// <summary>
    /// Http 响应结果处理器
    /// </summary>
    public class HttpResponseResult : IHttpResponseResult
    {
        /// <summary>
        /// 服务响应解析器
        /// </summary>
        public IServiceResponseResolver ServiceResponseResolver
        {
            get;
            private set;
        }

        /// <summary>
        /// 构造Http 响应结果处理器
        /// </summary>
        public HttpResponseResult() : this(new ServiceResponseResolver()) { }
        /// <summary>
        ///  构造Http 响应结果处理器
        /// </summary>
        /// <param name="serviceResponseResolver"></param>
        public HttpResponseResult( IServiceResponseResolver serviceResponseResolver)
        {
            if (serviceResponseResolver == null)
                throw new ArgumentNullException("serviceResponseResolver");

            ServiceResponseResolver = serviceResponseResolver;
        }

        /// <summary>
        /// 输出服务响应结果
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceResponse"></param>
        public void Execute(IHttpContext httpContext, IServiceResponse serviceResponse)
        {
            if (!serviceResponse.Success && serviceResponse.Exception != null)
                return; //throw serviceResponse.Exception;


            var response = httpContext.Response;

            var contentType = httpContext.Request.AcceptTypes != null && httpContext.Request.AcceptTypes.Length > 0 
                ? httpContext.Request.AcceptTypes[0]
                :"json";
            response.ContentType = contentType;
            
            try
            {
                var responseResult = ServiceResponseResolver.Resolve(httpContext, serviceResponse);
                var serializer = ContentHandlerTable.GetHandler(contentType);
                var strContent = serializer.Serialize(responseResult);
                response.Write(strContent);
                //response.Flush();
            }
            catch (Exception ex)
            {
                throw new ServiceDispatcherException(ServiceDispatcherExceptionCode.ResponseSerializerException, ex);
            }
        }


    }

    /// <summary>
    /// 服务响应解析器
    /// </summary>
    public class ServiceResponseResolver : IServiceResponseResolver
    {
        class SuccessResponse
        {
            public int code;
            public object data;
            public SuccessResponse() { }
            public SuccessResponse(object data)
            {
                code = 1;
                this.data = data;
            }
        }

        class DotNetSuccessResponse : SuccessResponse
        {
            public string dataType;
            public DotNetSuccessResponse() { }
            public DotNetSuccessResponse(object data)
                : base(data)
            {
                if (data != null)
                    this.dataType = data.GetType().FullName;
            }
        }

        /// <summary>
        /// 解析服务响应结果
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceResponse"></param>
        /// <returns></returns>
        public object Resolve(IHttpContext httpContext, IServiceResponse serviceResponse)
        {
            var jsonResponse = httpContext.Request["_cs_"] != null ?
                new DotNetSuccessResponse(serviceResponse.Result)
                : new SuccessResponse(serviceResponse.Result);
            return jsonResponse;
        }
    }

}


