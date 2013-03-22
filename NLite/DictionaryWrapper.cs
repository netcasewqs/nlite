using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace NLite
{

    [Serializable]
    class DictionaryWrapper : DictionaryWrapper<object>
    {
        public DictionaryWrapper()
        {
        }
        public DictionaryWrapper(IDictionary<string, object> dictionary):base(dictionary)
        {
           
        }
    }
    [Serializable]
    class DictionaryWrapper<T> : IDictionary<string, T>
    {

        private IDictionary<string, T> Inner;
      
        public DictionaryWrapper()
        {
            Inner = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        }
        public DictionaryWrapper(IDictionary<string, T> dictionary)
        {
            Inner = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
            if (dictionary != null && dictionary.Count > 0)
            {
                foreach (var key in dictionary.Keys)
                    Inner[key] = dictionary[key];
            }
        }

        public void Add(string key, T value)
        {
            Inner.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return Inner.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return Inner.Keys; }
        }

        public bool Remove(string key)
        {
            return Inner.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return Inner.TryGetValue(key, out value);
        }

        public ICollection<T> Values
        {
            get { return Inner.Values; }
        }

        public T this[string key]
        {
            get
            {
                T v = default(T);
                if (Inner.TryGetValue(key, out v))
                    return v;
                return v;
            }
            set
            {
                Inner[key] = value;
            }
        }

        void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item)
        {
            Inner.Add(item);
        }

        public void Clear()
        {
            Inner.Clear();
        }

        bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
        {
            return Inner.Contains(item);
        }

        void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Inner.Count; }
        }

        bool ICollection<KeyValuePair<string, T>>.IsReadOnly
        {
            get { return Inner.IsReadOnly; }
        }

        bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
        {
            return Inner.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

      
    }
}
