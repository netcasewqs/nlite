using System;
using System.Reflection;
using NLite.Reflection.Internal;
using NLite.Collections;
using System.Collections.Generic;

namespace NLite.Reflection
{
    
    /// <summary>
    /// 构造函数扩展类
    /// </summary>
    public static class CounstructorExtensions
    {
        private static readonly Dictionary<ConstructorInfo, ConstructorHandler> factoryMethodCache = new Dictionary<ConstructorInfo, ConstructorHandler>();

        /// <summary>
        /// 快速构造函数方法调用
        /// </summary>
        /// <param name="constructor">构造函数</param>
        /// <param name="args">参数</param>
        /// <returns>返回创建的对象</returns>
        public static object FastInvoke(this ConstructorInfo constructor, params object[] args)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            if (args == null)
                args = new object[0];

            ConstructorHandler handler;
            if (!factoryMethodCache.TryGetValue(constructor, out handler))
                factoryMethodCache[constructor] = handler = constructor.GetCreator();

            return handler(args);
        }
    }

}
