using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Binding
{
    /// <summary>
    /// 自定义模型绑定器注解
    /// </summary>
    [AttributeUsage(ValidTargets, AllowMultiple = false, Inherited = false)]
    public abstract class CustomModelBinderAttribute : Attribute
    {
        protected internal const AttributeTargets ValidTargets = AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Struct;
        
        /// <summary>
        /// 得到模型绑定器
        /// </summary>
        /// <returns></returns>
        public abstract IModelBinder GetBinder();
    }
}
