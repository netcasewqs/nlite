//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using NLite.Collections;

//namespace NLite
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="TListener"></typeparam>
//    /// <typeparam name="TEnum"></typeparam>
//    public class ListenerManager<TListener, TEnum> : BooleanDisposable, IListenerManager<TListener, TEnum>
//            where TListener : IListener<TEnum>
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        protected IMap<TEnum, ICollection<TListener>> listeners { get; private set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ListenerManager() : this( new List<TListener>()) { }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="listnersCreator"></param>
//        public ListenerManager(ICollection<TListener> listnersCreator)
//        {
//            Init(listnersCreator);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="listnersCreator"></param>
//        private void Init(ICollection<TListener> listnersCreator)
//        {
//            listeners = new Map<TEnum, ICollection<TListener>>(NumberOfEnumValues());
//            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
//            foreach (var item in values)
//                listeners[item] = listnersCreator;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="listner"></param>
//        protected virtual void OnAfterRegister(TListener listner)
//        {

//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="listner"></param>
//        protected virtual void OnAfterUnRegister(TListener listner)
//        {

//        }

//        /// <summary>
//        /// 注册监听器
//        /// </summary>
//        /// <param name="listeners">监听器集合</param>
//        public void Register(params TListener[] listeners)
//        {
//            if (listeners != null && listeners.Length > 0)
//            {
//                int m = 0;
//                int n = 0;
//                foreach (var key in listeners.Keys)
//                {
//                    m = key.GetHashCode();
//                    foreach (var item in listeners)
//                    {
//                        n = item.Type.GetHashCode();
//                        if ((m & n) == m)
//                        {
//                            listeners[key].Add(item);
//                            OnAfterRegister(item);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 注销监听器
//        /// </summary>
//        /// <param name="listner">监听器集合</param>
//        public void UnRegister(params TListener[] listeners)
//        {
//            int m = 0;
//            int n = 0;
//            foreach (var key in listeners.Keys)
//            {
//                m = key.GetHashCode();
//                foreach (var item in listeners)
//                {
//                    n = item.Type.GetHashCode();
//                    if ((m & n) == m)
//                    {
//                        listeners[key].Remove(item);
//                        OnAfterUnRegister(item);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="action"></param>
//        public void ForEach(Action<TListener> action)
//        {
//            if (action == null)
//                return;

//            int m = 0;
//            int n = 0;
//            foreach (var key in listeners.Keys)
//            {
//                //m = key.GetHashCode();
//                foreach (var item in listeners[key])
//                {
//                    if (item != null)
//                    {
//                        //n = item.Type.GetHashCode();
//                        //if ((m & n) == m)
//                            action(item);
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// 监听器数量
//        /// </summary>
//        public int Count
//        {
//            get
//            {
//                var set = new HashSet<TListener>();
//                foreach (var item in listeners.Values.ToArray())
//                    foreach (var i in item)
//                        set.Add(i);
//                int count = set.Count;
//                set.Clear();
//                return count;
//            }
//        }



//        private static int NumberOfEnumValues()
//        {
//            return typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static).Length;
//        }

//        /// <summary>
//        /// 得到迭代子对象
//        /// </summary>
//        public IEnumerator<TListener> GetEnumerator()
//        {
//            var set = new HashSet<TListener>();
//            foreach (var item in listeners.Values.ToArray())
//                foreach (var i in item)
//                    set.Add(i);
//            return set.GetEnumerator();
//        }

//        /// <summary>
//        /// 得到迭代子对象
//        /// </summary>
//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void Clear()
//        {
//            foreach (var item in this)
//            {
//                var dis = item as IDisposable;
//                if (dis != null)
//                    dis.Dispose();
//            }

//            listeners.Clear();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="disposing"></param>
//        protected override void Dispose(bool disposing)
//        {
//            Clear();
//        }
//    }
//}
