using System;
using System.ComponentModel;
using System.Threading;

namespace NLite
{
    /// <summary>
    /// 拥有释放标价的释放基类
    /// </summary>
    [Serializable]
    public class BooleanDisposable : IBooleanDisposable
    {
        [NonSerialized]
        private int _isDisposed;
        const int DisposedFlag = 1;

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BooleanDisposable()
        {
            Dispose(false);
        }

        #region IDisposable Members
        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            var isDisposed = _isDisposed;
            Interlocked.CompareExchange(ref _isDisposed, DisposedFlag, isDisposed);

            if (isDisposed != 0)
            {
                return;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
        /// <summary>
        /// 判断当前对象是否释放
        /// </summary>
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                Thread.MemoryBarrier();
                return _isDisposed == DisposedFlag;
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }
        /// <summary>
        /// 检查对象是否还没有被释放
        /// </summary>
        /// <exception cref="ObjectDisposedException">如果对象已经释放则触发该异常</exception>
        protected virtual void CheckNotDisposed()
        {
            if (IsDisposed)
            {
                var ex = new ObjectDisposedException(GetType().Name);
                throw ex;
            }
        }
    }
}


