using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Reflection.Dynamic;
using NLite.Internal;
using System.Reflection;

namespace NLite.Reflection
{
    /// <summary>
    /// NLite 动态代理门面类
    /// </summary>
    public sealed class Proxy
    {
        private static NLite.Reflection.Dynamic.ProxyFactory proxyFactory;

        private static NLite.Reflection.Dynamic.ProxyFactory ProxyFactory
        {
            get
            {
                if (proxyFactory == null)
                    proxyFactory = new Dynamic.ProxyFactory();
                return proxyFactory;
            }
        }

        /// <summary>
        /// 根据declaringType得到Proxy 类型
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="interfaceTypes"></param>
        /// <returns></returns>
        public static Type GetProxyType(Type declaringType, params Type[] interfaceTypes)
        {
            Guard.NotNull(declaringType, "parentType");

            return ProxyFactory.GetProxyTemplate(declaringType, interfaceTypes).ImplementationType;
        }

        /// <summary>
        /// 检测一个类型是否是代理类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsProxyType(Type type)
        {
            Guard.NotNull(type, "type");

            return ProxyFactory.IsProxyClass(type);
        }

        /// <summary>
        /// 构造Class Proxy实例
        /// </summary>
        /// <param name="declaringType">代表declaringType</param>
        /// <param name="interfaceTypes">代表代理所实现的接口列表</param>
        /// <param name="h">调用处理器</param>
        /// <param name="mixins"></param>
        /// <param name="arguments">代理构造器参数</param>
        /// <returns></returns>
        public static object NewProxyInstance(Type declaringType,
            Type[] interfaceTypes, 
            IInvocationHandler h,
            object[] mixins = null ,
            params object[] arguments)
        {
            Guard.NotNull(declaringType, "parentType");
            Guard.NotNull(h, "h");

            if (mixins != null && mixins.Length != 0)
            {
                var mixinDictionary = GetMixinDictionary(mixins, interfaceTypes);

                h = new MixinInvocationHandler(mixinDictionary, h);
            }

            return ProxyFactory.CreateProxy(declaringType, interfaceTypes, h, arguments);
        }


        /// <summary>
        /// 构造Interface Proxy实例
        /// </summary>
        /// <param name="interfaceTypes">代表代理所实现的接口列表,不运行null</param>
        /// <param name="h"></param>
        /// <param name="mixins"></param>
        /// <returns></returns>
        public static object NewProxyInstance(Type[] interfaceTypes,
            IInvocationHandler h,
            object[] mixins = null)
        {
            Guard.NotNull(h, "h");

            if (interfaceTypes == null || interfaceTypes.Length == 0)
                throw new ArgumentNullException("interfaceTypes");

            if (mixins != null && mixins.Length != 0)
            {
                var mixinDictionary = GetMixinDictionary(mixins, interfaceTypes);

                h = new MixinInvocationHandler(mixinDictionary, h);
            }

            return ProxyFactory.CreateProxy(typeof(object),interfaceTypes, h);
        }
    

        [Serializable]
        sealed class MixinInvocationHandler : IInvocationHandler
        {
            private readonly IInvocationHandler _invocationHandler;

            private readonly Dictionary<Type, object> _mixins;

            public MixinInvocationHandler(Dictionary<Type, object> mixins, IInvocationHandler invocationHandler)
            {

                _invocationHandler = invocationHandler;

                _mixins = mixins;
            }

            /// <inheritdoc/>
            public object Invoke(object target, MethodInfo methodInfo, object[] parameters)
            {
                var declaringType = methodInfo.DeclaringType;
                object mixin;

                if (_mixins.TryGetValue(declaringType, out mixin))
                    return methodInfo.Invoke(mixin, parameters);

                if (_invocationHandler != null)
                    return _invocationHandler.Invoke(target, methodInfo, parameters);

                return methodInfo.Invoke(target, parameters);
            }

        }

        private static Dictionary<Type, object> GetMixinDictionary(object[] mixins, Type[] interfaceTypes)
        {
            Dictionary<Type, object> _mixins = new Dictionary<Type, object>();

            foreach (var mixin in mixins)
            {
                if (mixin == null)
                    throw new ArgumentNullException("mixin");

                var mixinType = mixin.GetType();

                foreach (var interfaceType in mixinType.GetInterfaces())
                {
                    if (interfaceTypes.Contains(interfaceType))
                    {
                        throw new InvalidOperationException(String.Format(NLite.Reflection.Dynamic.Internal.Resources.InterfaceTypeWasAlreadyAdded, interfaceType));

                    }

                    _mixins.Add(interfaceType, mixin);
                }
            }

            return _mixins;
        }
    }
}
