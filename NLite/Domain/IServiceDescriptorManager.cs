/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NLite.Internal;


namespace NLite.Domain
{
	/// <summary>
    /// 服务元数据管理器接口
    /// </summary>
    public interface IServiceDescriptorManager
    {
        /// <summary>
        /// 检查特定的类型是否是服务类型,如果是返回服务名称，否则返回null
        /// </summary>
        /// <returns></returns>
        Func<Type, string> PopulateServiceName { get; }
        /// <summary>
        /// 注册服务元数据
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        IServiceDescriptor[] Register(Type serviceType);

        /// <summary>
        /// 通过服务名称返回特定的服务元数据
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        IServiceDescriptor GetServiceDescriptor(string serviceName);

        /// <summary>
        /// 得到特定类型的所有元数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IServiceDescriptor[] GetServiceDescriptors<T>();

        /// <summary>
        /// 所有服务元数据
        /// </summary>
        IEnumerable<IServiceDescriptor> ServiceDescriptors { get; }
        /// <summary>
        /// 在服务元数据被解析出来时进行监听
        /// </summary>
        event Action<IServiceDescriptor> ServiceDescriptorResolved;
        /// <summary>
        /// 在操作元数据被解析时进行监听
        /// </summary>
        event Action<IOperationDescriptor> OperationDescriptorResolved;
    }


     /// <summary>
     /// 服务元数据扩展类
     /// </summary>
     public static class ServiceDescriptorManagerExtensions
     {
         /// <summary>
         /// 注册服务元数据
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="sm"></param>
         /// <returns></returns>
         public static IServiceDescriptor[] Register<T>(this IServiceDescriptorManager sm) 
         {
             Guard.NotNull(sm, "sm");
             return sm.Register(typeof(T));
         }
     }

}
