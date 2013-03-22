using System;
using System.Collections.Generic;

namespace NLite.Collections.Internal
{
    #if !SILVERLIGHT
    [Serializable]
    #endif
    class SyncList<T> : ICollection<T>
    {
        protected readonly object InnerLocker = new object();
        private List<T> InnerList;
        public SyncList()
        {
            InnerList = new List<T>();
        }
        public SyncList(int capacity)
        {
            InnerList = new List<T>(capacity);
        }
        public SyncList(IEnumerable<T> collection)
        {
            InnerList = new List<T>(collection);
        }

        public bool Contains(T item)
        {
            return InnerList.Contains(item);
        }

        public void Clear()
        {
            lock (InnerLocker)
                InnerList.Clear();
        }

        public void Add(T item)
        {
            lock (InnerLocker)
                InnerList.Add(item);
        }

        public bool Remove(T item)
        {
            if (InnerList.Contains(item))
                lock (InnerLocker)
                    return InnerList.Remove(item);
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }


        public int Count
        {
            get { return InnerList.Count; }
        }


        public IEnumerator<T> GetEnumerator()
        {
           return InnerList.GetEnumerator();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

//#if !SILVERLIGHT
//    [Serializable]
//#endif
//    class DictionaryWrapper : IDictionary<string, object>
//    {

//        private IDictionary<string, object> Inner;

//        public DictionaryWrapper()
//        {
//            Inner = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
//        }
//        public DictionaryWrapper(IDictionary<string, object> dictionary)
//        {
//            Inner = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
//            if (dictionary != null && dictionary.Count > 0)
//            {
//                foreach (var key in dictionary.Keys)
//                    Inner[key] = dictionary[key];
//            }
//        }

//        public void Add(string key, object value)
//        {
//            Inner.Add(key, value);
//        }

//        public bool ContainsKey(string key)
//        {
//            return Inner.ContainsKey(key);
//        }

//        public ICollection<string> Keys
//        {
//            get { return Inner.Keys; }
//        }

//        public bool Remove(string key)
//        {
//            return Inner.Remove(key);
//        }

//        public bool TryGetValue(string key, out object value)
//        {
//            return Inner.TryGetValue(key, out value);
//        }

//        public ICollection<object> Values
//        {
//            get { return Inner.Values; }
//        }

//        public object this[string key]
//        {
//            get
//            {
//                object v;
//                if (Inner.TryGetValue(key, out v))
//                    return v;
//                return null;
//            }
//            set
//            {
//                Inner[key] = value;
//            }
//        }

//        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
//        {
//            Inner.Add(item);
//        }

//        public void Clear()
//        {
//            Inner.Clear();
//        }

//        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
//        {
//            return Inner.Contains(item);
//        }

//        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
//        {
//            Inner.CopyTo(array, arrayIndex);
//        }

//        public int Count
//        {
//            get { return Inner.Count; }
//        }

//        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
//        {
//            get { return Inner.IsReadOnly; }
//        }

//        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
//        {
//            return Inner.Remove(item);
//        }

//        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
//        {
//            return Inner.GetEnumerator();
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return Inner.GetEnumerator();
//        }


//    }
}
