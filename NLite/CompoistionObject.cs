using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{

    /// <summary>
    /// 可组合的可扩展对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CompoistionObject<T> : ExtensibleObject<T>, ICompositionObject<T> where T : CompoistionObject<T>
    {
        /// <summary>
        /// 用当前扩展点来扩展可扩展对象
        /// </summary>
        /// <param name="cfg"></param>
        public virtual void Attach(T owner)
        {
        }

        /// <summary>
        /// 从可扩展对象中移除当前扩展点
        /// </summary>
        /// <param name="cfg"></param>
        public virtual void Detach(T owner)
        {
            Clear();
        }
    }
}
