using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using NLite.Reflection;
using System.Collections.Specialized;
using NLite.Collections;
using NLite.Cache;

namespace NLite.Internal
{
    class HttpContextWrapper
    {
        public static Type HttpContextType;
        public static Type ApplicationType;
        public static Type SessionType;
        public static Type CacheType;

        public static Func CurrentProperty;
        public static Func ItemsProperty;
        public static Func ApplicationProperty;
        public static Func SessionProperty;
        public static Func CacheProperty;

        public static object RealContext
        {
            get
            {
                if (CurrentProperty == null)
                    return null;

                try
                {
                    return CurrentProperty(null);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static IDictionary Items
        {
            get { return ItemsProperty(RealContext) as IDictionary; }
        }

        class ApplicationWrapper:IApplication
        {

            static Func AddMethod;
            static Func RemoveMethod;
            static Func GetMethod;
            static Func GetCountMethod;
            static Func GetAllKeysMethod;
            static Func RemoveAllMethod;

            static ApplicationWrapper()
            {
                var applicationStateType = HttpContextWrapper.ApplicationType;
                AddMethod = applicationStateType.GetMethod("Set", new Type[] { typeof(string), typeof(object) }).GetFunc();
                GetMethod = applicationStateType.GetMethod("Get", new Type[] { typeof(string) }).GetFunc();
                RemoveMethod = applicationStateType.GetMethod("Remove", new Type[] { typeof(string) }).GetFunc();
                RemoveAllMethod = applicationStateType.GetMethod("RemoveAll").GetFunc();
                GetCountMethod = typeof(ICollection).GetMethod("get_Count").GetFunc();
                GetAllKeysMethod = applicationStateType.GetMethod("get_AllKeys").GetFunc();
            }

            private object Real
            {
            		get{ return HttpContextWrapper.RealApplicationState;}
            }
             
            #region IDataCollection Members

            public object this[object key]
            {
                get
                {
                    Guard.NotNull(key,"key");
                    var strKey = key.ToString();
                    Guard.NotNullOrEmpty(strKey, "strKey");

                    return GetMethod(HttpContextWrapper.RealApplicationState, strKey);
                }
                set
                {
                    Guard.NotNull(key, "key");
                    var strKey = key.ToString();
                    Guard.NotNullOrEmpty(strKey, "strKey");


                    AddMethod(HttpContextWrapper.RealApplicationState, strKey, value);
                }
            }

            public bool ContainsKey(object key)
            {
                Guard.NotNull(key, "key");
                var strKey = key.ToString();
                Guard.NotNullOrEmpty(strKey,"strKey");

                var keys = GetAllKeysMethod(HttpContextWrapper.RealApplicationState) as string[];
                if (keys == null || keys.Length == 0)
                    return false;

                return keys.FirstOrDefault(item => item == strKey) != null;
            }

            public void Remove(object key)
            {
                Guard.NotNull(key, "key");
                var strKey = key.ToString();
                Guard.NotNullOrEmpty(strKey, "strKey");

                RemoveMethod(HttpContextWrapper.RealApplicationState, strKey);
            }

            public int Count
            {
                get { return (int) GetCountMethod(HttpContextWrapper.RealApplicationState); }
            }

            public void Clear()
            {
                RemoveAllMethod(HttpContextWrapper.RealApplicationState);
            }

            #endregion
        }

        class CacheWrapper : ICache
        {
            public static Func SetMethod;
            public static Func GetMethod;
            public static Func RemoveMethod;
            public static Func GetDictionaryEnumeratorMethod;
            public static Func GetCountMethod;
            public static Func Insert3Method;
            public static Func Insert5Method;

            static Type dependencyType;

            static CacheWrapper()
            {
                var cacheType = HttpContextWrapper.CacheType;
                dependencyType = cacheType.Assembly.GetType("System.Web.Caching.CacheDependency");

                SetMethod = cacheType.GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }).GetFunc();
                GetMethod = cacheType.GetMethod("get_Item", new Type[] { typeof(string) }).GetFunc();
                RemoveMethod = cacheType.GetMethod("Remove", new Type[] { typeof(string) }).GetFunc();
                GetDictionaryEnumeratorMethod = cacheType.GetMethod("GetEnumerator").GetFunc();
                GetCountMethod = cacheType.GetMethod("get_Count").GetFunc();

                Insert3Method = cacheType.GetMethod("Insert", new Type[]
                {
                    Types.String
                    ,Types.Object
                    ,dependencyType
                }).GetFunc();

                Insert5Method = cacheType.GetMethod("Insert", new Type[]
                {
                    Types.String
                    ,Types.Object
                    ,dependencyType
                    ,Types.DateTime
                    ,Types.TimeSpan
                }).GetFunc();
            }

            static IDictionaryEnumerator GetDictionaryEnumerator()
            {
                return GetDictionaryEnumeratorMethod(HttpContextWrapper.RealCacheState) as IDictionaryEnumerator;
            }

            private object Real
            {
            		get{ return HttpContextWrapper.RealCacheState;}
            }
            
            #region IDataCollection Members

            public object this[object key]
            {
                get
                {
                    Guard.NotNull(key,"key");
                    var strKey = key.ToString();
                    Guard.NotNullOrEmpty(strKey, "strKey");

                    return GetMethod(HttpContextWrapper.RealCacheState, strKey);
                }
                set
                {
                    Guard.NotNull(key, "key");
                    var strKey = key.ToString();
                    Guard.NotNullOrEmpty(strKey, "strKey");


                    SetMethod(HttpContextWrapper.RealCacheState, strKey, value);
                }
            }

            public bool ContainsKey(object key)
            {
                Guard.NotNull(key, "key");
                var strKey = key.ToString();
                Guard.NotNullOrEmpty(strKey, "strKey");

                var it = GetDictionaryEnumerator();
                while (it.MoveNext())
                {
                    if (object.Equals( it.Key,key)) return true;
                }

                return false;
            }

            public void Remove(object key)
            {
                Guard.NotNull(key, "key");
                var strKey = key.ToString();
                Guard.NotNullOrEmpty(strKey, "strKey");
                RemoveMethod(HttpContextWrapper.RealCacheState, strKey);
            }

            public int Count
            {
                get { return (int)GetCountMethod(HttpContextWrapper.RealCacheState); }
            }

            public void Clear()
            {
                var it = GetDictionaryEnumerator();
                while (it.MoveNext())
                    Remove(it.Key);
            }

            #endregion

            public void Insert(string key, object value, object cacheDependency)
            {
                Insert3Method(HttpContextWrapper.RealCacheState, key, value, cacheDependency);
            }

            public void Insert(string key, object value, object cacheDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            {
                Insert5Method(HttpContextWrapper.RealCacheState, key, value, cacheDependency, absoluteExpiration, slidingExpiration);
            }

            public void Insert(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            {
                Insert5Method(HttpContextWrapper.RealCacheState, key, value, null, absoluteExpiration, slidingExpiration);
            }
        }

        class SessionWrapper : ISession
        {
            public static Func SetMethod;
            public static Func GetMethod;
            public static Func RemoveMethod;
            public static Func RemoveAllMethod;
            public static Func GetCountMethod;

            static SessionWrapper()
            {
                var sessionType = HttpContextWrapper.SessionType;

                SetMethod = sessionType.GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }).GetFunc();
                GetMethod = sessionType.GetMethod("get_Item", new Type[] { typeof(string) }).GetFunc();
                RemoveMethod = sessionType.GetMethod("Remove", new Type[] { typeof(string) }).GetFunc();
                RemoveAllMethod = sessionType.GetMethod("RemoveAll").GetFunc();
                GetCountMethod = sessionType.GetMethod("get_Count").GetFunc();
            }


            private ICollection Real
            {
            		get{ return HttpContextWrapper.RealSessionState;}
            }
             
            #region IDataCollection Members

            public object this[object key]
            {
                get
                {
                    Guard.NotNull(key,"key");
                    var strKey = key.ToString();
                    Guard.NotNullOrEmpty(strKey, "strKey");

                    return GetMethod(HttpContextWrapper.RealSessionState, strKey);
                }
                set
                {
                    Guard.NotNull(key, "key");
                    var strKey = key.ToString();
                    Guard.NotNullOrEmpty(strKey, "strKey");


                    SetMethod(HttpContextWrapper.RealSessionState, strKey, value);
                }
            }

            public bool ContainsKey(object key)
            {
                Guard.NotNull(key, "key");
                var strKey = key.ToString();
                Guard.NotNullOrEmpty(strKey, "strKey");

                var it = HttpContextWrapper.RealSessionState.GetProperty<NameObjectCollectionBase.KeysCollection>("Keys").GetEnumerator();
                while (it.MoveNext())
                {
                	if (object.Equals(it.Current , key)) return true;
                }

                return false;
            }

            public void Remove(object key)
            {
                Guard.NotNull(key, "key");
                var strKey = key.ToString();
                Guard.NotNullOrEmpty(strKey, "strKey");

                RemoveMethod(HttpContextWrapper.RealSessionState, strKey);
            }

            public int Count
            {
                get { return (int)GetCountMethod(HttpContextWrapper.RealSessionState); }
            }

            public void Clear()
            {
                RemoveAllMethod(HttpContextWrapper.RealSessionState);
            }

            #endregion
        }

        static NameObjectCollectionBase RealApplicationState
        {
            get { return ApplicationProperty(RealContext) as NameObjectCollectionBase; }
        }

        public static IApplication ApplicationState
        {
            get { return new ApplicationWrapper(); }
        }

        static object RealCacheState
        {
            get { return CacheProperty(RealContext); }
        }

        public static ICache CacheState
        {
            get { return new CacheWrapper(); }
        }

        static ICollection RealSessionState
        {
            get { return (ICollection)SessionProperty(RealContext) ; }
        }

        public static ISession SessionState
        {
            get { return new SessionWrapper(); }
        }
    }
    #if !SILVERLIGHT
    sealed class HashtableDataCollection : Hashtable, IDataCollection { }
#else
    sealed class HashtableDataCollection : Dictionary<object,object>, IDataCollection { }
#endif
    }
