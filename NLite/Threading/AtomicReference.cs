using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLite.Internal;

namespace NLite.Threading
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class AtomicReference<T> where T : class
    {
        private T _value;

        /// <summary>
        /// 
        /// </summary>
        public AtomicReference()
            : this(default(T))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public AtomicReference(T value)
        {
            Guard.NotNull(value, "value");
            _value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Atomically sets the value to the given updated value if the current value == the expected value.
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool CompareAndSet(T expect, T update)
        {
            return Interlocked.CompareExchange(ref _value, update, expect) == expect;
        }

        /// <summary>
        /// Atomically sets to the given value and returns the old value.
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public T GetAndSet(T newValue)
        {
            for (; ; )
            {
                var current = _value;
                var next = newValue;
                if (CompareAndSet(current, next))
                    return current;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static implicit operator T(AtomicReference<T> a)
        {
            return a._value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static implicit operator AtomicReference<T>(T i)
        {
            return new AtomicReference<T>(i);
        }

    }
}
