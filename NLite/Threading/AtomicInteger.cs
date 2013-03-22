using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace NLite.Threading
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Value={value}")]
    public class AtomicInteger:IEquatable<AtomicInteger>
    {
        private int value;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialValue"></param>
        public AtomicInteger(int initialValue)
        {
            value = initialValue;
        }
        /// <summary>
        /// 
        /// </summary>
        public AtomicInteger():this(0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public int Value { get { return value; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public int GetAndSet(int newValue)
        {
            for (; ; )
            {
                int current = value;
                if (CompareAndSet(current, newValue))
                    return current;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool CompareAndSet(int expect, int update)
        {
            return Interlocked.CompareExchange(ref value, update, expect) == expect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetAndIncrement()
        {
            for (; ; )
            {
                int current = value;
                int next = current + 1;
                if (CompareAndSet(current, next))
                    return current;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetAndDecrement()
        {
            for (; ; )
            {
                int current = value;
                int next = current - 1;
                if (CompareAndSet(current, next))
                    return current;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public int GetAndAdd(int delta)
        {
            for (; ; )
            {
                int current =value;
                int next = current + delta;
                if (CompareAndSet(current, next))
                    return current;
            }
        }

        private int IncrementAndGet()
        {
            for (; ; )
            {
                int current = value;
                int next = current + 1;
                if (CompareAndSet(current, next))
                    return next;
            }
        }

        private int DecrementAndGet()
        {
            for (; ; )
            {
                int current = value;
                int next = current - 1;
                if (CompareAndSet(current, next))
                    return next;
            }
        }

        private int AddAndGet(int delta)
        {
            for (; ; )
            {
                int current = value;
                int next = current + delta;
                if (CompareAndSet(current, next))
                    return next;
            }
        }

        private int MinusAndGet(int delta)
        {
            for (; ; )
            {
                int current = value;
                int next = current - delta;
                if (CompareAndSet(current, next))
                    return next;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return Convert.ToString(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static implicit operator int(AtomicInteger a)
        {
            return a.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static implicit operator AtomicInteger(int i)
        {
            return new AtomicInteger(i);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static AtomicInteger operator ++(AtomicInteger l)
        {
            l.IncrementAndGet();
            return l;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static AtomicInteger operator --(AtomicInteger l)
        {
            l.DecrementAndGet();
            return l;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static AtomicInteger operator +(AtomicInteger l, AtomicInteger r)
        {
            l.AddAndGet(r.value);
            return l;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static AtomicInteger operator +(AtomicInteger l, int r)
        {
            l.AddAndGet(r);
            return l;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static AtomicInteger operator -(AtomicInteger l, AtomicInteger r)
        {
            l.MinusAndGet(r.value);
            return l;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static AtomicInteger operator -(AtomicInteger l, int r)
        {
            l.MinusAndGet(r);
            return l;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="l"></param>
       /// <param name="r"></param>
       /// <returns></returns>
        public static bool operator ==(AtomicInteger l, AtomicInteger r)
        {
            return l.value == r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator ==(AtomicInteger l, int r)
        {
            return l.value == r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator ==(int l, AtomicInteger r)
        {
            return l == r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator !=(AtomicInteger l, AtomicInteger r)
        {
            return l.value != r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator !=(AtomicInteger l, int r)
        {
            return l.value != r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator !=(int l, AtomicInteger r)
        {
            return l != r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >(AtomicInteger l, AtomicInteger r)
        {
            return l.value > r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >(AtomicInteger l, int r)
        {
            return l.value > r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >(int l, AtomicInteger r)
        {
            return l > r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >=(AtomicInteger l, AtomicInteger r)
        {
            return l.value >= r.value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >=(AtomicInteger l, int r)
        {
            return l.value >= r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >=(int l, AtomicInteger r)
        {
            return l >= r.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator <=(AtomicInteger l, AtomicInteger r)
        {
            return l.value <= r.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator <=(AtomicInteger l, int r)
        {
            return l.value <= r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator <=(int l, AtomicInteger r)
        {
            return l <= r.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator <(AtomicInteger l, AtomicInteger r)
        {
            return l.value < r.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator <(AtomicInteger l, int r)
        {
            return l.value < r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator <(int l, AtomicInteger r)
        {
            return l < r.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AtomicInteger other)
        {
            if (other == null)
                return false;
            return value == other.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AtomicInteger);
        }
    }
}
