using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 扩展点接口，用来扩展可扩展对象的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExtension<T> where T : IExtensibleObject<T>
    {
        /// <summary>
        /// 用当前扩展点来扩展可扩展对象
        /// </summary>
        /// <param name="owner"></param>
        void Attach(T owner);

        /// <summary>
        /// 从可扩展对象中移除当前扩展点
        /// </summary>
        /// <param name="owner"></param>
        void Detach(T owner);
    }
}
