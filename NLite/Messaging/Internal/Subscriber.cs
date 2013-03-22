using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Collections;

namespace NLite.Messaging.Internal
{
    #if !SILVERLIGHT
    [Serializable]
    #endif
    partial class Subscriber : ISubscriber
    {
        //TODO:是否需要观察者顺序编号
        private readonly IDictionary<IObserverHandler<IMessage>,SubscribeMode> observers;
       
        public Subscriber()
        {
            this.observers = new Dictionary<IObserverHandler<IMessage>, SubscribeMode>();
        }

        public string Name { get; set; }
        public IMessageListenerManager ListnerManager { get; set; }

        /// <summary>
        /// 是否异步订阅
        /// </summary>
        public bool IsAsync { get; private set; }

        public IDisposable Subscribe(SubscribeMode mode, IObserverHandler<IMessage> handler)
        {
            if (handler == null
               || observers.ContainsKey(handler))
                return Disposable.Empty;

            if (mode == SubscribeMode.Async)
                IsAsync = true;

            var observer = handler.Target;

            lock (observers)
            {
                //ListnerManager.OnObserverAdding(observer, Name, Type);
                observers.Add(handler, mode);
                //ListnerManager.OnObserverAdded(observer, Name, Type);
            }

            return Disposable.Create(() => Unsubscribe(handler));
        }
 
        public void Unsubscribe(IObserverHandler<IMessage> handler)
        {
            if (handler == null
                || !observers.ContainsKey(handler))
                return;
            lock (observers)
            {
                //ListnerManager.OnObserverRemoving(handler.Target, Name, Type);
                observers.Remove(handler);
                //ListnerManager.OnObserverRemoved(handler.Target, Name, Type);
            }
        }

        IDictionary<IObserverHandler<IMessage>, SubscribeMode> ISubscriber.Observers
        {
            get
            {
                return new Dictionary<IObserverHandler<IMessage>,SubscribeMode>(observers);
            }
        }
       
        public void Clear()
        {
            if (observers.Count == 0)
                return;
            var items = observers.ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                var dis = items[i].Key as IDisposable;
                if (dis != null)
                    dis.Dispose();
                else
                {
                    var handler = items[i].Key.GetType().GetField("InnerHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    if (handler != null)
                        handler = null;
                }
            }

            observers.Clear();  
        }
    }
}
