using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NLite.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public class CountDownLatch
    {
        private object countLock = new object();
        private int currentCount;
        private bool isCanceled;
        private int count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public CountDownLatch(int count)
        {
            this.count = count;
            this.currentCount = count;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCanceled
        {
            get { return isCanceled; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentCount
        {
            get { return currentCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Await()
        {
            CheckCancellation();

            lock (countLock)
            {
                while (currentCount > 0)
                {
                    Monitor.Wait(countLock);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CountDown()
        {
            CheckCancellation();

            lock (countLock)
            {
                currentCount--;
                Monitor.PulseAll(countLock);
            }
        }

        private void CheckCancellation()
        {
            if (isCanceled)
                throw new OperationCanceledException("Semaphore has canceled");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            if (!isCanceled)
            {
                lock (countLock)
                {
                    if (!isCanceled)
                    {
                        isCanceled = true;
                        Monitor.PulseAll(countLock);
                    }
                }

            }
        }
    }
}
