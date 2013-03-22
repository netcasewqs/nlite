using System;
using System.Collections.Generic;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// 服务定位器门面类
    /// </summary>
    public static partial class ServiceLocator
    {
        const string key = "NLite_ServiceLocator";
        static IServiceLocator _current;

        /// <summary>
        /// 得到当前活动的服务定位器
        /// </summary>
        public static IServiceLocator Current
        {
            get
            {
                var current = NLiteEnvironment.IsWeb ? NLiteEnvironment.Application[key] as IServiceLocator : _current;
                return current;
            }
            set
            {
                  _current = value;
                if (NLiteEnvironment.IsWeb)
                    NLiteEnvironment.Application[key] = _current;
            }
        }

        /// <summary>
        /// 通过契约类型得到组件实例
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public static TContract Get<TContract>()
        {
            return Current.Get<TContract>();
        }

        /// <summary>
        /// 通过组件Id得到指定的组件
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TContract Get<TContract>(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            return Current.Get<TContract>(id);
        }

        /// <summary>
        /// 通过契约类型得到指定的组件实例
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static TComponent Get<TContract, TComponent>() where TComponent : TContract
        {
            return Current.Get<TContract, TComponent>();
        }

        /// <summary>
        /// 得到实现指定契约的所有组件
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TContract> GetAll<TContract>()
        {
            return Current.GetAll<TContract>();
        }

        /// <summary>
        /// 通过契约类型得到组件实例
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static object Get(Type contract)
        {
            Guard.NotNull(contract, "contract");
            return Current.Get(contract);
        }

        /// <summary>
        /// 通过组件Id得到指定的组件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static object Get(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            return Current.Get(id);
        }
    }
}
