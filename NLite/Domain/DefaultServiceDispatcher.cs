using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Cfg;
using NLite.Domain.Cfg;
using NLite.Domain.Listener;
using NLite.Log;
using NLite.Data;
using NLite.Threading;
using NLite.Validation;
using System.Data.Common;
using NLite.Internal;

namespace NLite.Domain
{
    /// <summary>
    /// 缺省服务分发器
    /// </summary>
    public class DefaultServiceDispatcher : IServiceDispatcher
    {
        static readonly Dictionary<string, object> EmptyArgs = new Dictionary<string, object>(0);

        private IServiceDescriptorManager DescriptorManager;
        private IServiceDispatchListenerManager ListenManager;
        private Func<string, object> ServiceLocator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceLocator"></param>
        /// <param name="listenManager"></param>
        /// <param name="descriptorManager"></param>
        public DefaultServiceDispatcher(Func<string, object> serviceLocator, IServiceDispatchListenerManager listenManager, IServiceDescriptorManager descriptorManager)
        {
            Guard.NotNull(serviceLocator, "serviceLocator");
            Guard.NotNull(descriptorManager, "descriptorManager");
            Guard.NotNull(listenManager, "listenManager");

            ServiceLocator = serviceLocator;
            DescriptorManager = descriptorManager;
            ListenManager = listenManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceLocator"></param>
        public DefaultServiceDispatcher(Func<string, object> serviceLocator)
            : this(serviceLocator, 
            new ServiceDispatchListenerManager(),
            new DefaultServiceDescriptorManager(ServiceDispatcher.GetServiceNameByDefault))
        {
        }

        private void OnAfterAction(IOperationExecutedContext ctx)
        {
            ListenManager.OnOperationExecuted(ctx);
            foreach (var filter in ctx.OperationDescriptor.Filters)
                filter.OnOperationExecuted(ctx);
        }

        private void OnBeforeAction(object service, IOperationExecutingContext ctx)
        {
            ListenManager.OnOperationExecuting(ctx);
            if (ctx.Cancelled)
                return;

            foreach (var filter in ctx.OperationDescriptor.Filters)
            {
                filter.OnOperationExecuting(ctx);
                if (ctx.Cancelled)
                    return;
            }
        }

        /// <summary>
        /// 得到操作元数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public virtual IOperationDescriptor GetOperationDescriptor(IServiceRequest req)
        {
            Guard.NotNull(req, "req");
            var serviceDescriptor = DescriptorManager.GetServiceDescriptor(req.ServiceName);
            if (serviceDescriptor == null)
                throw new ServiceDispatcherException(
                        ServiceDispatcherExceptionCode.ServiceNotFound)
                {
                    ServiceName = req.ServiceName
                    ,
                    OperationName = req.OperationName
                };

            //得到领域Action元数据
            var operationDesc = serviceDescriptor[req.OperationName];
            if (operationDesc == null)
                throw new ServiceDispatcherException(
                        ServiceDispatcherExceptionCode.OperationNotFound)
                {
                    ServiceName = req.ServiceName
                    ,
                    OperationName = req.OperationName
                };

            return operationDesc;
        }

        /// <summary>
        /// 分发服务调用
        /// </summary>
        /// <param name="req">请求</param>
        /// <returns>返回响应信息</returns>
        public IServiceResponse Dispatch(IServiceRequest req)
        {
            Guard.NotNull(req, "req");
            try
            {
                ListenManager.OnDispatching(req);
                return OnExecute(req);
            }
            finally
            {
                try
                {
                    ListenManager.OnDispatched(req);
                }
                catch (Exception ex)
                {
                    ExceptionManager.Handle(ex);
                }
            }
        }

        private IServiceResponse OnExecute(IServiceRequest req)
        {
           
            IServiceResponse resp = null;
            IOperationDescriptor operationDesc = null;
            try
            {
                operationDesc = GetOperationDescriptor(req);
                resp = OnExecute(req, operationDesc);
            }
            catch (Exception ex)
            {
                resp = new ServiceResponse(ex);
            }

            if (req.Context.ContainsKey("RawArguments"))
                req.Context.Remove("RawArguments");

            if (req.Arguments.ContainsKey("AutoCloseServiceContext"))
                ServiceContext.Current = null;
            if (resp.Exception != null)
            {
                try
                {
                    ListenManager.OnExceptionFired(req, resp, operationDesc);
                }
                finally { }
            }

            return resp;
        }

        /// <summary>
        /// 执行服务分发
        /// </summary>
        /// <param name="req"></param>
        /// <param name="operationDesc"></param>
        /// <returns></returns>
        public IServiceResponse Execute(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            Guard.NotNull(req, "req");
            Guard.NotNull(operationDesc, "operationDesc");
          
            var resp = OnExecute(req, operationDesc);

            if (req.Context.ContainsKey("RawArguments"))
                req.Context.Remove("RawArguments");

            if (req.Arguments.ContainsKey("AutoCloseServiceContext"))
                ServiceContext.Current = null;
            if (resp.Exception != null)
            {
                try
                {
                    ListenManager.OnExceptionFired(req, resp, operationDesc);
                }
                finally { }
            }

            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="operationDesc"></param>
        /// <returns></returns>
        protected virtual IServiceResponse OnExecute(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            var resp = new ServiceResponse();
            //得到领域服务元数据
            var serviceDesc = operationDesc.ServiceDescriptor;
            var svrResolvedContext = new ServiceResolvedContext(serviceDesc, req, resp);
            ServiceContext.Current = new ServiceContext { Request = req, Response = resp };

            try
            {
                ListenManager.OnServiceDescriptorFound(svrResolvedContext);
                if (svrResolvedContext.Cancelled)
                    return resp;

                svrResolvedContext.OperationDescriptor = operationDesc;
                ListenManager.OnOperationDescriptorFound(svrResolvedContext);
                if (svrResolvedContext.Cancelled)
                    return resp;
            }
            catch (Exception ex)
            {
                resp.AddException(ex);
                return resp;
            }

            object service = null;
            try
            {
                service = ServiceLocator(serviceDesc.Id);
            }
            catch (Exception ex)
            {
                resp.AddException(new ServiceDispatcherException(ServiceDispatcherExceptionCode.CreateServiceException, ex)
                {
                    ServiceName = req.ServiceName,
                    OperationName = req.OperationName
                });
                return resp;
            }

            svrResolvedContext.Service = service;
            ListenManager.OnServiceResolved(svrResolvedContext);
            if (svrResolvedContext.Cancelled)
                return resp;

            try
            {
                req = PopulateModelBinding(operationDesc, req, resp);
            }
            catch (Exception ex)
            {
                resp.AddException(new ServiceDispatcherException(ServiceDispatcherExceptionCode.ParameterBindException, ex)
                {
                    ServiceName = req.ServiceName,
                    OperationName = req.OperationName
                });
                return resp;
            }

            try
            {
                if (!ValidateParamters(operationDesc, req, resp))
                {
                    resp.AddException(new ServiceDispatcherException(ServiceDispatcherExceptionCode.ModelValidationException)
                    {
                        ServiceName = req.ServiceName,
                        OperationName = req.OperationName
                    });
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.AddException(new ServiceDispatcherException(ServiceDispatcherExceptionCode.ModelValidationException, ex)
                {
                    ServiceName = req.ServiceName,
                    OperationName = req.OperationName
                });
                return resp;
            }

            //创建操作上下文对象
            var ctx = new OperationExecutedContext(req, operationDesc) { Response = resp };

            try
            {
                //执行前置过滤器
                OnBeforeAction(service, ctx);

                //如果过滤器进行了必要的拦截则返回
                if ((ctx as IOperationExecutingContext).Cancelled)
                    return resp;

                //调用领域服务方法
                resp.Result = operationDesc.Invoke(service, ctx.Arguments.Values.ToArray());

                ctx.Service = service;

                //执行后置过滤器
                OnAfterAction(ctx);
                resp.Success = true;
            }
            catch (Exception ex)
            {
                resp.AddException(ex);
            }

            return resp;
        }

        bool ValidateParamters(IOperationDescriptor operationDesc, IServiceRequest req, ServiceResponse resp)
        {
            if (!req.ValidateRequest
               || req.Arguments == null
               || req.Arguments.Count == 0)
                return true;

            foreach (var key in req.Arguments.Keys)
            {
                var errorState = Validator.Validate(req.Arguments[key]);
                if (errorState.Count > 0)
                {
                    resp.ErrorState.AddRange(errorState);
                    return false;
                }
            }

            return true;
        }


        IServiceRequest PopulateModelBinding(IOperationDescriptor operationDesc, IServiceRequest req, IServiceResponse resp)
        {
            if (req.Arguments == null)
                return req;

            //备份原始请求参数
            req.Context["RawArguments"] = req.Arguments;

            var data = operationDesc.GetParameterValues(req.Arguments);
            var tmpReq = ServiceRequest.Create(req.ServiceName, req.OperationName, data);
            tmpReq.ValidateRequest = req.ValidateRequest;

            if (tmpReq.Arguments.ContainsKey("AutoCloseServiceContext"))
                tmpReq.Arguments.Remove("AutoCloseServiceContext");
            tmpReq.Context = req.Context;

            return tmpReq;
        }

    }
}
