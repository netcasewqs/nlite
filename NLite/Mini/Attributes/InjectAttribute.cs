using System;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// 标记某个字段，属性，参数可以被DI容器自动注入进来，
    /// 也可以标记某个方法或构造函数让DI容器通过该方法或构造函数进行
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
	                | AttributeTargets.Method
	                | AttributeTargets.Field
	                | AttributeTargets.Constructor
                    | AttributeTargets.Parameter
	                , AllowMultiple = false
	                , Inherited = true)]
    public class InjectAttribute:Attribute
    {
        /// <summary>
        /// 得到或设置组件依赖Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 是否支持重新注入
        /// </summary>
        public bool Reinjection { get; set; }
    }

    /// <summary>
    /// 注入回调方法注册注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method
        , AllowMultiple = false, Inherited = true)]
    [MetadataAttributeAttribute]
    public class InjectedNotificationAttribute : Attribute
    {
    }
}
