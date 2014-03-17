using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using NLite.DynamicProxy;

namespace NLite.Domain
{
    /// <summary>
    /// 服务代理工厂
    /// </summary>
    public static class ServiceProxyFactory
    {
        class ServiceProxyInterceptor:NLite.DynamicProxy.IInterceptor
        {
            private Type InterfaceType;
            private string ServiceName;
            private const string ServiceNameSuffix = "service";
            private string ServiceDispatcherName;

            public ServiceProxyInterceptor(string serviceDispatcherName, Type interfaceType)
            {
                ServiceDispatcherName = serviceDispatcherName;
                InterfaceType = interfaceType;
                ServiceName = interfaceType.Name;
                ServiceName = ServiceName.TrimStart('I', 'i');
                
                if(ServiceName.ToLower().EndsWith(ServiceNameSuffix))
                    ServiceName = ServiceName.Substring(0, ServiceName.Length - 7).ToLower();
            }

            public object Intercept(DynamicProxy.InvocationInfo info)
            {
                var method = info.TargetMethod;

                var pararmeters = info.TargetMethod.GetParameters();
                var argCount = info.Arguments.Length;
                var args = new DictionaryWrapper();
                for (int i = 0; i < argCount; i++)
                    args[pararmeters[i].Name] = info.Arguments[i];


                var operationName = method.Name;

                var req = ServiceRequest.Create(ServiceName, operationName, args);
                if (ServiceDispatcherName.HasValue())
                    req.Arguments.Add(ServiceDispatcher.ServiceDispatcherParameterName, ServiceDispatcherName);

                var resp = ServiceDispatcher.Dispatch(req);

                if (resp.Success)
                {
                    return resp.Result;
                }
                else
                {
                    throw resp.Exception;
                }
            }
        }

        /// <summary>
        /// 创建领域服务代理对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>(string serviceDispatcherName = ServiceDispatcher.DefaultServiceDispatcherName)
        {
            var serviceType = typeof(T);
            if (!serviceType.IsInterface)
                throw new NotSupportedException("Not support class type,only support interface ");

            if (serviceDispatcherName.IsNullOrEmpty())
                serviceDispatcherName = ServiceDispatcher.DefaultServiceDispatcherName;

            //return (T)new ServiceProxy(serviceDispatcherName, serviceType).GetTransparentProxy();

            var proxy = ProxyFactory.CreateProxy<T>(new ServiceProxyInterceptor(serviceDispatcherName, serviceType), serviceType);

            return proxy;
        }

        [Obsolete]
        class ServiceProxy : RealProxy
        {
            private Type InterfaceType;
            private string ServiceName;
            private const string ServiceNameSuffix = "service";
            private string ServiceDispatcherName;
            public ServiceProxy(string serviceDispatcherName, Type interfaceType)
                : base(interfaceType)
            {
                ServiceDispatcherName = serviceDispatcherName;
                InterfaceType = interfaceType;
                ServiceName = interfaceType.Name;
                ServiceName = ServiceName.TrimStart('I', 'i');

                if (ServiceName.ToLower().EndsWith(ServiceNameSuffix))
                    ServiceName = ServiceName.Substring(0, ServiceName.Length - 7).ToLower();
            }

            public override IMessage Invoke(IMessage message)
            {
                var methodMessage = message as IMethodCallMessage;
                var method = methodMessage.MethodBase;

                var argCount = methodMessage.ArgCount;
                var args = new DictionaryWrapper();
                for (int i = 0; i < argCount; i++)
                    args[methodMessage.GetArgName(i)] = methodMessage.GetArg(i);


                var operationName = method.Name;

                var req = ServiceRequest.Create(ServiceName, operationName, args);
                if (ServiceDispatcherName.HasValue())
                    req.Arguments.Add(ServiceDispatcher.ServiceDispatcherParameterName, ServiceDispatcherName);
                try
                {
                    var resp = ServiceDispatcher.Dispatch(req);

                    if (resp.Success)
                    {
                        return new ReturnMessage(resp.Result, null, 0, methodMessage.LogicalCallContext, methodMessage);
                    }
                    else
                    {
                        return new ReturnMessage(resp.Exception, methodMessage);
                    }
                }
                catch (Exception ex)
                {
                    return new ReturnMessage(ex, methodMessage);
                }
            }
        }
    }
}
