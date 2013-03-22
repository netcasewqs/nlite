using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 扩展点基类
    /// </summary>
    /// <typeparam name="T">可扩展对象</typeparam>
    public abstract class Extension<T> : IExtension<T> where T : IExtensibleObject<T>
    {
        /// <summary>
        /// 安装扩展点
        /// </summary>
        /// <param name="cfg"></param>
        public abstract void Attach(T cfg);

        /// <summary>
        /// 卸载扩展点
        /// </summary>
        /// <param name="cfg"></param>
        public virtual void Detach(T cfg)
        {
        }
    }
}
