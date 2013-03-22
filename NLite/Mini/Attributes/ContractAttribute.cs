using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 指定某个接口，抽象类，或类标记为契约
    /// </summary>
    [Obsolete("该标记已经过时了，新版本将根据组件类型以及基类和实现的接口是否是系统Assembly中的类型，如果不是那么将作为契约类型，否则不是")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ContractAttribute : Attribute
    {
    }
}
