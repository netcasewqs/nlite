using System;
using NLite;
using NLite.Mini;

namespace NLite
{
    /// <summary>
    /// 被标记为一个“组件”,该组件实现了一个或多个“契约”，如果没有实现任何“契约”，那么该组件也作为“契约”进行注册
    /// </summary>
    [AttributeUsage(AttributeTargets.Class 
        | AttributeTargets.Field
        | AttributeTargets.Property
        | AttributeTargets.Method
        , AllowMultiple=false, Inherited=true)]
    [MetadataAttributeAttribute]
    public class ComponentAttribute:Attribute
    {
        internal static readonly ComponentAttribute Default = new ComponentAttribute();
        /// <summary>
        /// 得到或设置组件的Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 得到或设置组件的生命周期
        /// </summary>
        public LifestyleFlags Lifestyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type Contract { get; set; }
    }

}
