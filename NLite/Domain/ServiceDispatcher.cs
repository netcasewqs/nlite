/*
 * Created by SharpDevelop.
 * User: netcasewqs
 * Date: 2011-1-12
 * Time: 12:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NLite.Domain.Cfg;
using NLite.Internal;

namespace NLite.Domain
{
    /// <summary>
    /// 服务分发器
    /// </summary>
    public static class ServiceDispatcher
    {
        /// <summary>
        /// 缺省服务分发器名称：_NLite_Default_ServiceDispatcher_
        /// </summary>
        public const string DefaultServiceDispatcherName="_NLite_Default_ServiceDispatcher_";
        /// <summary>
        /// 请求的服务分发器的参数名称：RouteName
        /// </summary>
        public const string ServiceDispatcherParameterName = "RouteName";


        /// <summary>
        ///  服务分发
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <remarks>
        /// 可以在请求参数中指定服务分发器的名称，如果没有指定则
        /// </remarks>
        public static IServiceResponse Dispatch(IServiceRequest req)
        {
            if (req == null)
                throw new ArgumentNullException("req");
           
            //设置缺省服务分发器名称
            string serviceDispatcherName = DefaultServiceDispatcherName;

            //是否指定了服务分发器
            if (req.Arguments.ContainsKey(ServiceDispatcherParameterName))
            {
               var tmpServiceDispatcherName = req.Arguments[ServiceDispatcherParameterName] as string;
               if (!string.IsNullOrEmpty(tmpServiceDispatcherName))
                   serviceDispatcherName = tmpServiceDispatcherName;
            }

            //通过服务分发器名称得到服务分发器配置节点
            IServiceDispatcherConfiguationItem config = ServiceLocator.Get<IServiceDispatcherConfiguationItem>(serviceDispatcherName);
            if (config == null)
                throw new ApplicationException("No config service dispatcher for name:" + serviceDispatcherName);

            return config.ServiceDispatcherCreator().Dispatch(req);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public static TResult Dispatch<TResult>(string serviceName, string operationName)
        {
            return ServiceDispatcher.Dispatch<TResult>(serviceName, operationName, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="operationName"></param>
        /// <param name="requestArgs"></param>
        /// <returns></returns>
        public static TResult Dispatch<TResult>(string serviceName, string operationName, object requestArgs)
        {
            var req = ServiceRequest.Create(serviceName, operationName, requestArgs);
            var resp = ServiceDispatcher.Dispatch(req);

            if (resp.Exception != null)
                throw resp.Exception;
            return (TResult)resp.Result;
        }

        static readonly string serviceSuffix = "Service";
        static readonly int serviceStringLength = serviceSuffix.Length;
        /// <summary>
        /// 以默认的方式得到服务名称
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetServiceNameByDefault(Type t)
        {
            Guard.NotNull(t, "t");
            string serviceName = t.Name;
            if (!serviceName.EndsWith(serviceSuffix))
                return null;
            return serviceName.Substring(0, serviceName.Length - serviceStringLength).ToLower();
        }
    }
}
