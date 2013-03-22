using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 标记某个字段,属性或参数的值来源是通过注入多个实现了该契约的组件过来的
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
                   | AttributeTargets.Field
                   | AttributeTargets.Parameter
                   , AllowMultiple = false
                   , Inherited = true)]
    public class InjectManyAttribute : Attribute
    {
        /// <summary>
        /// 是否支持重新注入,缺省为true
        /// </summary>
        public bool Reinjection = true;
        
    }
}
