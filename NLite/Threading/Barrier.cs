using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NLite.Threading
{
    /// <summary>
    /// 轻量级栅栏
    /// </summary>
    public class Barrier
    {
        private readonly object lockObj = new object();
        private readonly int originalCount;
        private int currentCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public Barrier(int count)
        {
            if (count < 0)
                throw new ArgumentException("count", "count must be great 0");
            originalCount = count;
            currentCount = count;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentCount
        {
            get { return currentCount; }
        }



        /// <summary>
        /// 自动等待，当栅栏全部移除那么就不用等待，否则移除一个栅栏，然后再判断栅栏数量，
        /// 如果栅栏数量不为0那么阻塞当前线程，否则继续执行
        /// </summary>
        public void Await()
        {
            if (currentCount > 0)
            {
                lock (lockObj)
                {
                    if (Interlocked.Decrement(ref currentCount) == 0)
                    {
                        Monitor.PulseAll(lockObj);
                        currentCount = originalCount;
                    }
                    else
                        Monitor.Wait(lockObj);
                }

            }
        }
    }
}
