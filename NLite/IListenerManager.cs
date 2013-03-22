using System;
using System.Collections.Generic;

namespace NLite
{
    /// <summary>
    /// 监听管理器接口
    /// </summary>
    /// <typeparam name="TListener">监听器类型</typeparam>
    /// <remarks>观察者模式中的抽象主题角色</remarks>
    public interface IListenerManager<TListener>:IDisposable
        where TListener:IListener
    {
        /// <summary>
        /// 监听管理器集合
        /// </summary>
        IEnumerable<TListener> Listeners { get; }
        /// <summary>
        /// 注册监听器
        /// </summary>
        /// <param name="listeners">监听器集合</param>
        void Register(params TListener[] listeners);
        /// <summary>
        /// 注销监听器
        /// </summary>
        /// <param name="listeners">监听器集合</param>
        void UnRegister(params TListener[] listeners);

        /// <summary>
        /// 监听器数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 清空所有监听管理器
        /// </summary>
        void Clear();
    }

  
  
}