using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 
        /// </summary>
        /// <modelExp name="key"></modelExp>
        /// <returns></returns>
        object this[object key] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <modelExp name="key"></modelExp>
        /// <returns></returns>
        bool ContainsKey(object key);

        /// <summary>
        /// 
        /// </summary>
        /// <modelExp name="key"></modelExp>
        void Remove(object key);

        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        void Clear();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDependency"></param>
        void Insert(string key, object value, object cacheDependency);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDependency"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        void Insert(string key, object value, object cacheDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        void Insert(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
    }

    /// <summary>
    /// 缓存管理器
    /// </summary>
    public static class CacheManager
    {
        /// <summary>
        /// 缺省缓存名称
        /// </summary>
        public const string DefaultCacheName = "_NLite_Default_Cache_";
        /// <summary>
        /// 缓存容器集合
        /// </summary>
        public static readonly Dictionary<string, ICache> Caches = new Dictionary<string, ICache>();
       
        /// <summary>
        /// 缺省缓存容器
        /// </summary>
        public static ICache Cache
        {
            get
            {
                ICache cache;
                if (!Caches.TryGetValue(DefaultCacheName, out cache))
                    lock (Caches)
                        Caches[DefaultCacheName] = cache = NLiteEnvironment.IsWeb
                                              ? NLite.Internal.HttpContextWrapper.CacheState
                                              : new NLite.Threading.Internal.LocalCacheState();
                return cache;
            }
        }
    }
}
