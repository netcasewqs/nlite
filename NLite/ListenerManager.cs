using System;
using System.Linq;
using System.Collections.Generic;
using NLite.Collections;
using NLite.Internal;
using NLite.Collections.Internal;
    
namespace NLite
{
    /// <summary>
    /// 监听管理器
    /// </summary>
    /// <typeparam name="TListener">监听器类型</typeparam>
    /// <remarks>观察者模式中的抽象主题角色</remarks>
    public class ListenerManager<TListener> : BooleanDisposable, IListenerManager<TListener>
        where TListener : IListener
    {
        private readonly object Mutext = new object();
        private ICollection<TListener> listeners;

        /// <summary>
        /// 监听管理器集合
        /// </summary>
        public IEnumerable<TListener> Listeners
        {
            get { return listeners.ToArray(); }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public ListenerManager()
        {
            listeners = new List<TListener>();
        }

        /// <summary>
        /// 在监听管理器注册后触发
        /// </summary>
        /// <param name="listner"></param>
        protected virtual void OnAfterRegister(TListener listner)
        {

        }

        /// <summary>
        /// 在监听管理器卸载后触发
        /// </summary>
        /// <param name="listner"></param>
        protected virtual void OnAfterUnRegister(TListener listner)
        {

        }

        /// <summary>
        /// 注册监听器
        /// </summary>
        /// <param name="listeners">监听器集合</param>
        public void Register(params TListener[] listeners)
        {
            if (listeners == null || listeners.Length == 0)
                throw new ArgumentNullException("listeners");

            lock (Mutext)
            {
                foreach (var item in listeners)
                    if (!this.listeners.Contains(item))
                    {
                        this.listeners.Add(item);
                        OnAfterRegister(item);
                    }
            }
        }

        /// <summary>
        /// 注销监听器
        /// </summary>
        /// <param name="listeners">监听器集合</param>
        public void UnRegister(params TListener[] listeners)
        {
            if (listeners == null || listeners.Length == 0)
                throw new ArgumentNullException("listeners");

            lock (Mutext)
            {
                foreach (var item in listeners)
                    if (this.listeners.Contains(item))
                    {
                        this.listeners.Remove(item);
                        OnAfterUnRegister(item);
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<TListener> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (var item in listeners)
                if (item != null)
                    action(item);
        }
        /// <summary>
        /// 监听器数量
        /// </summary>
        public int Count
        {
            get { return listeners.Count; }
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Clear();
        }

        /// <summary>
        /// 清空所有监听管理器
        /// </summary>
        public void Clear()
        {
            if (listeners.Count == 0)
                return;

            var items = listeners.ToArray();
            UnRegister(items);
            foreach (var item in listeners.ToArray())
            {
                var dis = item as IDisposable;
                if (dis != null)
                {
                    try
                    {
                        dis.Dispose();
                    }
                    finally { }
                }
            }

            listeners.Clear();
        }

       
    }
}
