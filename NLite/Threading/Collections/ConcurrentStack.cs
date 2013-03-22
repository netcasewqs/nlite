using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLite.Threading;
using NLite.Internal;
using NLite.Threading.Collections.Internal;

namespace NLite.Threading.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class ConcurrentStack<T> : IEnumerable<T>
    {
        private AtomicReference<Node<T>> top;
        /// <summary>
        /// 
        /// </summary>
        public ConcurrentStack()
        {
            top = new AtomicReference<Node<T>>(new Node<T>());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            Node<T> node = new Node<T>();
            node.Value = item;
            bool sucessful = false;
            while (!sucessful)
            {
                Node<T> oldTop = top.Value;
                Node<T> newTop = new Node<T> { Value = item, Next = oldTop };
                sucessful = top.CompareAndSet(oldTop, newTop);
            }; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            bool sucessful = false;
            Node<T> newTop = null;
            Node<T> oldTop = null;
            while (!sucessful)
            {
                oldTop = top.Value;
                newTop = oldTop.Next;
                sucessful = top.CompareAndSet(oldTop, newTop);
            }
            return oldTop.Value; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return top.Value.Value;
        }

        #region IEnumerable<T> Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            Node<T> header = top.Value;

            while (header.Next != null)
            {
                yield return header.Value;
                header = header.Next;
            }
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    namespace Internal
    {
        #if !SILVERLIGHT
    [Serializable]
    #endif
        class Node<V>
        {
            public Node<V> Next;
            public V Value;
        }
    }
}
