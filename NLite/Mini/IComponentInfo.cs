using System;
using NLite.Collections;
using NLite.Internal;
using System.Collections.Generic;


namespace NLite
{
    /// <summary>
    /// 组件元数据信息接口
    /// </summary>
    public interface IComponentInfo : IEquatable<IComponentInfo>
    {
        /// <summary>
        /// 得到组件标帜Id
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 得到组件实现的所有契约
        /// </summary>
        Type[] Contracts { get; }

        /// <summary>
        /// 得到组件的具体类型
        /// </summary>
        Type Implementation { get; }

        /// <summary>
        /// 得到或设置组件的工厂类型（该属性常常作为组件的自定义工厂）
        /// </summary>
        string Activator { get; set; }

        /// <summary>
        /// 得到或设置组件的生命周期
        /// </summary>
        LifestyleFlags Lifestyle { get; set; }

        /// <summary>
        /// 得到组件的扩展属性
        /// </summary>
        IDictionary<string,object> ExtendedProperties { get; }

        /// <summary>
        /// 得到组件的工厂函数
        /// </summary>
        Func<object> Factory { get; }
    }


    static class ComponentInfoExtensions
    {
        public static bool IsGenericType(this IComponentInfo info)
        {
            return info.Lifestyle == LifestyleFlags.GenericSingleton
                || info.Lifestyle == LifestyleFlags.GenericThread
                || info.Lifestyle == LifestyleFlags.GenericTransient;
        }
    }

    
}
