using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLite.Threading;
using NLite.Collections;


namespace NLite.Collections.Internal
{
    class SyncQueue<T> : BooleanDisposable, IQueue<T>
    {
        private Queue<T> InnerQueue = new Queue<T>();
        private readonly object sysLock = new object();

        #region IQueue<TTarget> Members
        public object SyncRoot { get { return sysLock; } }

        public int Count
        {
            get
            {
                return InnerQueue.Count;
            }
        }

        public T Dequeue()
        {
            lock (sysLock)
            {

                var result = default(T);
                if (InnerQueue.Count > 0)
                    result = InnerQueue.Dequeue();
                Monitor.PulseAll(sysLock);
                return result;
            }
        }


        public void Enqueue(T item)
        {
            lock (sysLock)
            {
                InnerQueue.Enqueue(item);
                Monitor.PulseAll(sysLock);
            }
        }

      

        #endregion

        #region IEnumerable<TTarget> Members

        public IEnumerator<T> GetEnumerator()
        {
            lock (sysLock)
                return InnerQueue.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (sysLock)
                return InnerQueue.GetEnumerator();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            InnerQueue.Clear();
            base.Dispose(disposing);
        }

    }
}
