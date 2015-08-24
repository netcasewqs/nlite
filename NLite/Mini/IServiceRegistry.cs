using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using NLite.Reflection;
using System.IO;
using NLite.Mini.Fluent;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// 服务注册表接口
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// 通过组件元数据注册组件到容器中
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        IServiceRegistry Register(IComponentInfo info);

        /// <summary>
        /// 将组件实例所依赖的其它组件都通过容器自动注入进来
        /// </summary>
        /// <param name="componentInstance"></param>
        /// <returns></returns>
        IServiceRegistry Compose(object componentInstance);

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="id">实例Id</param>
        /// <param name="instance">实例</param>
        IServiceRegistry RegisterInstance(string id, object instance);

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="id">实例Id</param>
        /// <param name="contract">服务</param>
        /// <param name="instance">实例</param>
        IServiceRegistry RegisterInstance(string id, Type contract, object instance);

        /// <summary>
        /// 是否注册了实现给定契约接口的组件
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        bool HasRegister(Type contract);

        /// <summary>
        /// 是否注册了含有组件Id的组件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool HasRegister(string id);

        /// <summary>
        /// 通过组件Id注销组件
        /// </summary>
        /// <param name="id">组件Id</param>
        void UnRegister(string id);

        /// <summary>
        ///通过契约类型注销相应的组件
        /// </summary>
        void UnRegister(Type contract);

        /// <summary>
        /// 得到监听管理器
        /// </summary>
        IComponentListenerManager ListenerManager { get; }
    }

    /// <summary>
    /// 服务注册表门面
    /// </summary>
    public static class ServiceRegistry
    {
        const string key = "NLite_ServiceLocator";
        static IServiceRegistry _current;
       
        /// <summary>
        /// 返回当前活动的服务注册表
        /// </summary>
        public static IServiceRegistry Current
        {
            get
            {
                var current = NLiteEnvironment.IsWeb ? NLiteEnvironment.Application[key] as IServiceRegistry : _current;
                return current;
            }
            set
            {
                  _current = value;
                if (NLiteEnvironment.IsWeb)
                    NLiteEnvironment.Application[key] = value;
            }
        }

        /// <summary>
        /// 是否注册了实现给定契约接口的组件
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public static bool HasRegister<TContract>()
        {
            return Current.HasRegister(typeof(TContract));
        }

        /// <summary>
        /// 是否注册了实现给定契约接口的组件
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool HasRegister(Type service)
        {
            Guard.NotNull(service, "service");
            return Current.HasRegister(service);
        }

        /// <summary>
        /// 是否注册了含有组件Id的组件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool HasRegister(string id)
        {
            Guard.NotNullOrEmpty(id, "id");

            return Current.HasRegister(id);
        }

        /// <summary>
        /// 通过组件类型注册组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static IServiceRegistry Register<TComponent>()
        {
            return Current.Register<TComponent>();
        }

        /// <summary>
        /// 通过组件类型和和组件Id来注册组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IServiceRegistry Register<TComponent>(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            return Current.Register<TComponent>(id);
        }

        /// <summary>
        /// 通过流畅Api接口来注册组件
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IServiceRegistry Register(Action<IComponentExpression> handler)
        {
            Guard.NotNull(handler, "handler");
            return Current.Register(handler);
        }

        ///// <summary>
        ///// 通过流畅Api接口来注册组件
        ///// </summary>
        ///// <param name="handlers"></param>
        ///// <returns></returns>
        //public static IServiceRegistry Register(params Action<IComponentExpression>[] handlers)
        //{
        //    return Current.Register(handlers);
        //}

        /// <summary>
        /// 注册指定类型所在Assembly下的所有标记为"ComponentAttribue"的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceRegistry RegisteryFromAssemblyOf<T>()
        {
            return Current.RegisterFromAssemblyOf<T>();
        }

        /// <summary>
        /// 将组件实例所依赖的其它组件都通过容器自动注入进来
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IServiceRegistry Compose(object component)
        {
            Guard.NotNull(component, "component");
            return Current.Compose(component);
        }
    }
}
