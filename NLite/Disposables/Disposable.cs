using System;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// Disposable对象
    /// </summary>
    [Serializable]
    public struct Disposable
    {
        /// <summary> 
        /// 得到空Disposable对象
        /// </summary>
        public static readonly IDisposable Empty = new EmptyDisposeAction();

        /// <summary>
        /// 创建Disposable对象
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IDisposable Create(Action action)
        {
            Guard.NotNull(action, "action");
            return new ActionDisposable(action);
        }

        /// <summary>
        /// 创建Disposable对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IDisposable Create(object target, Action action)
        {
            Guard.NotNull(target, "target");
            Guard.NotNull(action, "action");
            return new ActionDisposable(target,action);
        }
    }
}
