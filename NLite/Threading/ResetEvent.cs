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
    public class ResetEvent
    {
        private volatile bool state;
        private readonly object lockObj = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialState"></param>
        public ResetEvent(bool initialState)
        {
            state = initialState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Wait()
        {
            return Wait(Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Wait(int millisecondsTimeout)
        {
            lock (lockObj)
            {
                if (state)
                    return true;
                return Monitor.Wait(lockObj,millisecondsTimeout);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Set()
        {
            lock (lockObj)
            {
                Monitor.PulseAll(lockObj);
                state = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            lock (lockObj)
                state = false;
        }
    }
}
