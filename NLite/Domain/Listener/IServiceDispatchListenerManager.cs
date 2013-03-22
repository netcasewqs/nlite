using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Listener
{
    /// <summary>
    /// 服务分发监听管理器接口
    /// </summary>
    public interface IServiceDispatchListenerManager:IListenerManager<IServiceDispatchListener>,IServiceDispatchListener
    {
    }

    /// <summary>
    /// 服务分发监听管理器
    /// </summary>
    public class ServiceDispatchListenerManager : ListenerManager<IServiceDispatchListener>, IServiceDispatchListenerManager, IServiceDispatchListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listner"></param>
        protected override void OnAfterRegister(IServiceDispatchListener listner)
        {
            var methods = listner.GetType().GetMethods();
            var type = listner.GetType();
            foreach (var m in methods)
            {
                if (m.DeclaringType != type)
                    continue;
                switch (m.Name)
                {
                    case "OnDispatching":
                        Dispatching += listner.OnDispatching;
                        break;
                    case "OnDispatched":
                        Dispatched += listner.OnDispatched;
                        break;
                    case "OnServiceDescriptorFound":
                        ServiceDescriptorFound += listner.OnServiceDescriptorFound;
                        break;
                    case "OnOperationDescriptorFound":
                        OperationDescriptorFound += listner.OnOperationDescriptorFound;
                        break;
                    case "OnServiceResolved":
                        ServiceResolved += listner.OnServiceResolved;
                        break;
                    case "OnOperationExecuting":
                        OperationExecuting += listner.OnOperationExecuting;
                        break;
                    case "OnOperationExecuted":
                        OperationExecuted += listner.OnOperationExecuted;
                        break;
                    case "OnExceptionFired":
                        ExceptionFired += listner.OnExceptionFired;
                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listner"></param>
        protected override void OnAfterUnRegister(IServiceDispatchListener listner)
        {
            var methods = listner.GetType().GetMethods();
            var type = listner.GetType();
            foreach (var m in methods)
            {
                if (m.DeclaringType != type)
                    continue;
                switch (m.Name)
                {
                    case "OnDispatching":
                        Dispatching -= listner.OnDispatching;
                        break;
                    case "OnDispatched":
                        Dispatched -= listner.OnDispatched;
                        break;
                    case "OnServiceDescriptorFound":
                        ServiceDescriptorFound -= listner.OnServiceDescriptorFound;
                        break;
                    case "OnOperationDescriptorFound":
                        OperationDescriptorFound -= listner.OnOperationDescriptorFound;
                        break;
                    case "OnServiceResolved":
                        ServiceResolved -= listner.OnServiceResolved;
                        break;
                    case "OnOperationExecuting":
                        OperationExecuting -= listner.OnOperationExecuting;
                        break;
                    case "OnOperationExecuted":
                        OperationExecuted -= listner.OnOperationExecuted;
                        break;
                    case "OnExceptionFired":
                        ExceptionFired -= listner.OnExceptionFired;
                        break;
                }
            }
        }

        #region IServiceDispatchListener Member
        event Action<IServiceRequest> Dispatching;
        void IServiceDispatchListener.OnDispatching(IServiceRequest req)
        {
            var handler = Dispatching;
            if (handler != null)
                handler(req);
        }

        event Action<IServiceRequest> Dispatched;
        void IServiceDispatchListener.OnDispatched(IServiceRequest req)
        {
            var handler = Dispatched;
            if (handler != null)
                handler(req);
        }

        event Action<IServiceDescriptorFoundContext> ServiceDescriptorFound;
        void IServiceDispatchListener.OnServiceDescriptorFound(IServiceDescriptorFoundContext ctx)
        {
            var handler = ServiceDescriptorFound;
            if (handler != null)
                handler(ctx);
        }

        event Action<IOperationDescriptorFoundContext> OperationDescriptorFound;
        void IServiceDispatchListener.OnOperationDescriptorFound(IOperationDescriptorFoundContext ctx)
        {
            var handler = OperationDescriptorFound;
            if (handler != null)
                handler(ctx);
        }

        event Action<IServiceResolvedContext> ServiceResolved;
        void IServiceDispatchListener.OnServiceResolved(IServiceResolvedContext ctx)
        {
            var handler = ServiceResolved;
            if (handler != null)
                handler(ctx);
        }

        event Action<IOperationExecutingContext> OperationExecuting;
        void IServiceDispatchListener.OnOperationExecuting(IOperationExecutingContext ctx)
        {
            var handler = OperationExecuting;
            if (handler != null)
                handler(ctx);
        }

        event Action<IOperationExecutedContext> OperationExecuted;
        void IServiceDispatchListener.OnOperationExecuted(IOperationExecutedContext ctx)
        {
            var handler = OperationExecuted;
            if (handler != null)
                handler(ctx);
        }

        event Action<IServiceRequest, IServiceResponse,IOperationDescriptor> ExceptionFired;
        void IServiceDispatchListener.OnExceptionFired(IServiceRequest req, IServiceResponse resp, IOperationDescriptor operationDesc)
        {
            var handler = ExceptionFired;
            if (handler != null)
                handler(req, resp, operationDesc);
        }
        #endregion
    }

}
