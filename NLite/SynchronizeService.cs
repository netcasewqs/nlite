#if !SILVERLIGHT
using System;
using System.ComponentModel;
using System.Threading;

namespace NLite
{
    /// <summary>
    /// 同步服务
    /// </summary>
    public static class SynchronizeService
    {
        /// <summary>
        /// 得到或设置同步对象
        /// </summary>
        public static ISynchronizeInvoke SynchronizeObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="args"></param>
        public static void Send(this Delegate handler, params object[] args)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (SynchronizeObject != null && SynchronizeObject.InvokeRequired)
                SynchronizeObject.Invoke(handler, args);
            else
            {
                var sc = SynchronizationContext.Current;
                if (sc != null)
                    sc.Send(state => handler.DynamicInvoke(args), null);
                else
                    handler.DynamicInvoke(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="args"></param>
        public static void Post(this Delegate handler, params object[] args)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (SynchronizeObject != null && SynchronizeObject.InvokeRequired)
                SynchronizeObject.BeginInvoke(handler, args);
            else
            {
                var sc = SynchronizationContext.Current;
                if (sc != null)
                    sc.Post(state => handler.DynamicInvoke(args), null);
                else
                    ThreadPool.QueueUserWorkItem(state => handler.DynamicInvoke(args));
            }
        }
    }
}
#endif