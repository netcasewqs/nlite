using System;
using System.Collections;
using NLite.Collections;
using NLite.Internal;
using NLite.Threading.Internal;
using System.Collections.Generic;
using NLite.Cache;

namespace NLite.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public static class Local
    {
        
        internal static readonly IDataContext _dataContext = new LocalDataContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(object key)
        {
            Guard.NotNull(key, "key");
            return _dataContext.Data[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(object key, object value)
        {
            Guard.NotNull(key, "key");
            _dataContext.Data[key] = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(object key)
        {
            Guard.NotNull(key, "key");
            _dataContext.Data.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(object key)
        {
            Guard.NotNull(key, "key");
            return _dataContext.Data.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            var items = _dataContext.Data;
            var count = _dataContext.Data.Count;
            for (int i = 0; i < count; i++)
            {
                var item = items[0];
                if (item == null)
                    continue;

                var dis = item as IDisposable;
                if (dis != null)
                {
                    try
                    {
                        dis.Dispose();
                    }
                    catch { }
                }

                item = null;
            }

            items.Clear();
        }
       
    }

    namespace Internal
    {
        class LocalDataContext : IDataContext
        {
            [ThreadStatic]
            private static HashtableDataCollection _localData;
            private static readonly object LocalDataHashtableKey = new object();

            public IDataCollection Data
            {
                get
                {
                    if (!NLiteEnvironment.IsWeb)
                    {
                        if (_localData == null)
                            _localData = new HashtableDataCollection();
                        return _localData;
                    }
                    else
                    {
                        var web_hashtable = HttpContextWrapper.Items[LocalDataHashtableKey] as HashtableDataCollection;
                        if (web_hashtable == null)
                        {
                            web_hashtable = new HashtableDataCollection();
                            HttpContextWrapper.Items[LocalDataHashtableKey] = web_hashtable;
                        }
                        return web_hashtable;
                    }
                }
            }
        }
        #if !SILVERLIGHT
        class LocalState : Hashtable, IApplication, ISession { }

        class LocalCacheState : Hashtable, ICache
        {
            public void Insert(string key, object value, object cacheDependency)
            {
                this[key] = value;
            }

            public void Insert(string key, object value, object cacheDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            {
                this[key] = value;
            }

            public void Insert(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            {
                this[key] = value;
            }
        }
#else
        class LocalState : Dictionary<object,object>, ICache, IApplication, ISession { }

#endif
        }
}
