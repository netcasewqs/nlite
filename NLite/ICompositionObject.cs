using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 可组合的可扩展对象接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICompositionObject<T> : IExtensibleObject<T>, IExtension<T> where T : ICompositionObject<T>
    {
    }
}
