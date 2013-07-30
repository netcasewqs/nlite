using System;
using System.Collections.Generic;
using NLite.Internal;
using System.Reflection;

namespace NLite
{
    /// <summary>
    /// 服务定位器接口
    /// </summary>
    public interface IServiceLocator : IServiceProvider,IDisposable
    {
        
        /// <summary>
        /// 得到服务
        /// </summary>
        /// <param name="id">服务Id</param>
        /// <returns>返回服务实例</returns>
        object Get(string id);

        /// <summary>
        /// 得到服务
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>返回服务实例</returns>
        object Get(Type serviceType);

        /// <summary>
        /// 得到服务
        /// </summary>
        /// <param name="id">服务Id</param>
        /// <returns>返回服务实例</returns>
        object Get(string id, IDictionary<string, object> args);

        /// <summary>
        /// 得到服务
        /// </summary>
        /// <param name="service">服务类型</param>
        /// <returns>返回服务实例</returns>
        object Get(Type service, IDictionary<string, object> args);

        /// <summary>
        /// 得到服务
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object Get(string id, params object[] args);


        /// <summary>
        /// 得到服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object Get(Type serviceType, params object[] args);

        /// <summary>
        /// 得到所有服务
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        IEnumerable<object> GetAll(Type service);

        /// <summary>
        /// 得到所有服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll<T>();
    }


    /// <summary>
    /// 服务重组通知接口
    /// </summary>
    public interface IServiceReinjectedNotification
    {
        /// <summary>
        /// 
        /// </summary>
        void OnReinjected(string[] memberNames);
    }
 
    /// <summary>
    /// 服务定位器扩展类
    /// </summary>
    public static class ServiceLocatorExtensions
    {

        /// <summary>
        /// 通过组件Id得到指定的组件
        /// </summary>
        /// <typeparam name="TComponent">组件类型</typeparam>
        /// <param name="id">组件Id</param>
        /// <returns>返回组件实例</returns>
        public static TComponent Get<TComponent>(this IServiceLocator locator, string id)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            Guard.NotNullOrEmpty(id, "id");
            return (TComponent)locator.Get(id);
        }
       
    }
      
}

namespace NLite
{
    /// <summary>
    /// 服务提供者扩展类
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// 通过契约类型得到组件实例
        /// </summary>
        /// <typeparam name="TContract">契约类型</typeparam>
        /// <returns>返回组件实例</returns>
        public static TContract Get<TContract>(this IServiceLocator locator)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            return (TContract)locator.Get(typeof(TContract));
        }
        
        /// <summary>
        /// 通过契约类型得到指定的组件实例
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static TComponent Get<TContract, TComponent>(this IServiceLocator locator) where TComponent : TContract
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            return (TComponent)locator.GetService(typeof(TContract));
        }

    }

}
