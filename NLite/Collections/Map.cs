using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using NLite.Threading;
using System.Linq;
using NLite.Internal;
using NLite.Threading.Internal;
using System.Diagnostics;

namespace NLite.Collections
{
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TKey"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    //public interface IMap<TKey, TValue> : IDictionary<TKey, TValue>
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    TValue GetOrAdd(TKey key, TValue value);

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="creator"></param>
    //    /// <returns></returns>
    //    TValue GetOrAdd(TKey key, Func<TValue> creator);
    //}
 

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TKey"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    //[DebuggerTypeProxy(typeof(Dictionary<,>))]
    //#if !SILVERLIGHT
    //[Serializable]
    //#endif
    //public class Map<TKey, TValue> : IMap<TKey, TValue>
    //{
    //    private readonly Dictionary<TKey, TValue> Inner;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public Map()
    //    {
    //        Inner = new Dictionary<TKey, TValue>();
    //    }
       
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="capacity"></param>
    //    public Map(int capacity)
    //    {
    //        Inner = new Dictionary<TKey, TValue>(capacity);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="comparer"></param>
    //    public Map(IEqualityComparer<TKey> comparer)
    //    {
    //        Inner = new Dictionary<TKey, TValue>(comparer);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="capacity"></param>
    //    /// <param name="comparer"></param>
    //    public Map(int capacity, IEqualityComparer<TKey> comparer)
    //    {
    //        Inner = new Dictionary<TKey, TValue>(capacity, comparer);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public TValue this[TKey key]
    //    {
    //        get { return Get(key);}
    //        set{ Add(key,value);}
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ICollection<TKey> Keys { get { return Inner.Keys; } }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ICollection<TValue> Values { get { return Inner.Values; } }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public bool ContainsKey(TKey key)
    //    {
    //        return Inner.ContainsKey(key);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    public void Add(TKey key, TValue value)
    //    {
    //        if (key != null)
    //        {
    //            Inner.Add(key, value);
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public TValue Get(TKey key)
    //    {
    //        TValue result = default(TValue);
    //        if (key != null)
    //            Inner.TryGetValue(key, out result);
                   
    //        return result;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public bool TryGetValue(TKey key, out TValue value)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        if (Inner.TryGetValue(key, out value))
    //            return true;

    //        value = default(TValue);

    //        return false;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="creator"></param>
    //    /// <returns></returns>
    //    public TValue GetOrAdd(TKey key, Func<TValue> creator)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        TValue value = default(TValue);
    //        if (Inner.TryGetValue(key, out value))
    //            return value;

    //        if (creator != null)
    //        {
    //            value = creator();
    //            if (value == null)
    //                return value;

    //            Inner.Add(key, value);
    //            return value;
    //        }

    //        return default(TValue);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public TValue GetOrAdd(TKey key, TValue value)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        if (Inner.TryGetValue(key, out value))
    //            return value;
    //        Inner.Add(key, value);

    //        return value;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public bool Remove(TKey key)
    //    {
    //        if (Inner.ContainsKey(key))
    //            return Inner.Remove(key);
                    
    //        return false;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void Clear()
    //    {
    //        Inner.Clear();
               
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int Count
    //    {
    //        get
    //        {
    //            return Inner.Count;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    //    {
    //        return Inner.GetEnumerator();
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public bool IsReadOnly { get { return false; } }

    //    #region IEnumerable Members

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    #endregion

    //    //protected override void Dispose(bool disposing)
    //    //{
    //    //    Inner.Clear();
    //    //}


    //    #region ICollection<KeyValuePair<TKey,TValue>> Members
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="item"></param>

    //    public void Add(KeyValuePair<TKey, TValue> item)
    //    {
    //        Add(item.Key, item.Value);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="item"></param>
    //    /// <returns></returns>
    //    public bool Contains(KeyValuePair<TKey, TValue> item)
    //    {
    //        return ContainsKey(item.Key);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="array"></param>
    //    /// <param name="arrayIndex"></param>
    //    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    //    {
    //        (Inner as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="item"></param>
    //    /// <returns></returns>
    //    public bool Remove(KeyValuePair<TKey, TValue> item)
    //    {
    //        return Remove(item.Key);
    //    }

    //    #endregion
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TKey"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    // [DebuggerTypeProxy(typeof(Dictionary<,>))]
    //#if !SILVERLIGHT
    //[Serializable]
    //#endif
    //public class ConcurrentMap<TKey, TValue> : BooleanDisposable, IMap<TKey, TValue>
    //{
    //    private readonly Dictionary<TKey, TValue> Inner;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ConcurrentMap()
    //    {
    //        Inner = new Dictionary<TKey, TValue>();
    //    }
       
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="capacity"></param>
    //    public ConcurrentMap(int capacity)
    //    {
    //        Inner = new Dictionary<TKey, TValue>(capacity);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="comparer"></param>
    //    public ConcurrentMap(IEqualityComparer<TKey> comparer)
    //    {
    //        Inner = new Dictionary<TKey, TValue>(comparer);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="capacity"></param>
    //    /// <param name="comparer"></param>
    //    public ConcurrentMap(int capacity, IEqualityComparer<TKey> comparer)
    //    {
    //        Inner = new Dictionary<TKey, TValue>(capacity, comparer);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public bool IsReadOnly { get { return false; } }
    //    #if !SILVERLIGHT
    //    [NonSerialized]
    //    #endif
    //    private readonly object locker = new object();

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public bool ContainsKey(TKey key)
    //    {
    //        return Inner.ContainsKey(key);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public TValue this[TKey key]
    //    {
    //        get { return Get(key);}
    //        set{ Add(key,value);}
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ICollection<TKey> Keys { get { return Inner.Keys; } }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ICollection<TValue> Values { get { return Inner.Values; } }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    public void Add(TKey key, TValue value)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        lock(locker)//using (new WriteLock(Mutex))
    //        {
    //            Inner.Add(key, value);
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public TValue Get(TKey key)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        TValue result = default(TValue);
    //        lock (locker)
    //            Inner.TryGetValue(key, out result);
    //        return result;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public bool TryGetValue(TKey key, out TValue value)
    //    {
    //        CheckNotDisposed();

    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        lock (locker)
    //            if (Inner.TryGetValue(key, out value))
    //                return true;

    //        value = default(TValue);

    //        return false;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    public TValue GetOrAdd(TKey key, TValue value)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        lock (locker)//using (new ReadLock(Mutex))
    //        {
    //            if (!Inner.ContainsKey(key))
    //            {
    //                //using (new WriteLock(Mutex))
    //                    Inner.Add(key, value);
    //            }
    //            else
    //            {
    //                //using (new ReadLock(Mutex))
    //                    value = Inner[key];
    //            }
    //        }

    //        return value;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="creator"></param>
    //    /// <returns></returns>
    //    public TValue GetOrAdd(TKey key, Func<TValue> creator)
    //    {
    //        TValue value = default(TValue);
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        lock (locker) //using (new ReadLock(Mutex))
    //        {
    //            if (!Inner.ContainsKey(key))
    //            {
    //                //using (new WriteLock(Mutex))
    //                {
    //                    if (creator != null)
    //                        value = creator();
    //                    Inner.Add(key, value);
    //                }
    //            }
    //            else
    //            {
    //                //using (new ReadOnlyLock(Mutex))
    //                    value = Inner[key];
    //            }
    //        }

    //         return value;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <returns></returns>
    //    public bool Remove(TKey key)
    //    {
    //        if (key == null)
    //            throw new ArgumentNullException("key");

    //        if (Inner.ContainsKey(key))
    //            lock (locker)
    //                return Inner.Remove(key);
    //        return false;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void Clear()
    //    {
    //        if(Inner != null)
    //            lock (locker) 
    //                Inner.Clear();
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int Count
    //    {
    //        get
    //        {
    //            return Inner.Count;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    //    {
    //        return new Dictionary<TKey, TValue>(Inner).GetEnumerator();
    //    }

    //    #region ICollection<KeyValuePair<TKey,TValue>> Members
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="item"></param>
    //    public void Add(KeyValuePair<TKey, TValue> item)
    //    {
    //        Add(item.Key, item.Value);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="item"></param>
    //    /// <returns></returns>
    //    public bool Contains(KeyValuePair<TKey, TValue> item)
    //    {
    //        return ContainsKey(item.Key);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="array"></param>
    //    /// <param name="arrayIndex"></param>
    //    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    //    {
    //        (Inner as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="item"></param>
    //    /// <returns></returns>
    //    public bool Remove(KeyValuePair<TKey, TValue> item)
    //    {
    //        return Remove(item.Key);
    //    }

    //    #endregion

    //    #region IEnumerable Members
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    #endregion
       
    //}
}
