using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Listener
{
    /// <summary>
    /// 服务分发监听器接口
    /// </summary>
    public interface IServiceDispatchListener:IListener
    {
        /// <summary>
        /// 在服务分发前进行监听
        /// </summary>
        /// <param name="req"></param>
        void OnDispatching(IServiceRequest req);
        /// <summary>
        /// 在服务元数据被发现时进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnServiceDescriptorFound(IServiceDescriptorFoundContext ctx);


        /// <summary>
        /// 在操作元数据被发现时进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnOperationDescriptorFound(IOperationDescriptorFoundContext ctx);

        /// <summary>
        /// 在服务对象被解析时进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnServiceResolved(IServiceResolvedContext ctx);

       
        /// <summary>
        /// Called before an operation method executes.
        /// </summary>
        /// <param name="ctx"></param>
        void OnOperationExecuting(IOperationExecutingContext ctx);

        /// <summary>
        /// Called after an operation method executes.
        /// </summary>
        /// <param name="ctx"></param>
        void OnOperationExecuted(IOperationExecutedContext ctx);

        /// <summary>
        /// 在异常发生时进行监听
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="operationDesc"></param>
        void OnExceptionFired(IServiceRequest req, IServiceResponse resp, IOperationDescriptor operationDesc);

        /// <summary>
        /// 在服务分发后进行监听
        /// </summary>
        /// <param name="req"></param>
        void OnDispatched(IServiceRequest req);
    }

    /// <summary>
    /// 确实服务分发监听器
    /// </summary>
    public class ServiceDispatcherListener : IServiceDispatchListener
    {
      
        /// <summary>
        /// 在服务元数据被发现时进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnServiceDescriptorFound(IServiceDescriptorFoundContext ctx)
        {
        }

        /// <summary>
        /// 在操作元数据被发现时进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnOperationDescriptorFound(IOperationDescriptorFoundContext ctx)
        {
        }

        /// <summary>
        /// 在服务对象被解析时进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnServiceResolved(IServiceResolvedContext ctx)
        {
        }

        /// <summary>
        /// Called after an operation method executes.
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnOperationExecuted(IOperationExecutedContext ctx)
        {
        }

        /// <summary>
        /// Called before an operation method executes.
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnOperationExecuting(IOperationExecutingContext ctx)
        {
        }

        /// <summary>
        /// 在异常发生时进行监听
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="operationDesc"></param>
        public virtual void OnExceptionFired(IServiceRequest req, IServiceResponse resp, IOperationDescriptor operationDesc)
        {
        }

        /// <summary>
        /// 在服务分发前进行监听
        /// </summary>
        /// <param name="req"></param>
        public virtual  void OnDispatching(IServiceRequest req)
        {
        }

        /// <summary>
        /// 在服务分发后进行监听
        /// </summary>
        /// <param name="req"></param>
        public virtual void OnDispatched(IServiceRequest req)
        {
        }
    }
}
