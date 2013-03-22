using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NLite.Collections;
using NLite.Reflection;
using NLite.Internal;
namespace NLite.Messaging.Internal
{
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    class MessageRepository : BooleanDisposable, IMessageRepository
    {
        private IDictionary<Key, ISubject> Table;
        private ISubjectBuilder Builder;
        public IDelegateInvoker DelegateInvoker;

        public IMessageListenerManager ListnerManager { get; private set; }

        
        public MessageRepository()
        {
            Table = new Dictionary<Key, ISubject>();
            Builder = new SubjectBuilder();
            ListnerManager = new MessageListnerManager();
            DelegateInvoker = new NLite.Messaging.Internal.DelegateInvoker();
        }
       
        public int Count
        {
            get { return Table.Count; }
        }

  
        public ISubject this[string topic, Type type]
        {
            get
            {

                var targetType = typeof(IMessage).IsAssignableFrom(type) ? GetDataType(type) : type;
                var key = string.IsNullOrEmpty(topic) ? Key.Make(targetType) : Key.Make(targetType, topic);

                ISubject subject = null;

                if (!Table.TryGetValue(key, out subject))
                {
                    lock (Table)
                    {
                        subject = Builder.Build(key.ToString());
                        Table[key] = subject;
                    }
                    subject.Subscriber.ListnerManager = ListnerManager;
                    subject.Executor.ListnerManager = ListnerManager;
                    subject.Executor.DelegateInvoker = DelegateInvoker;
                }
                return subject;
            }
        }

        static Type GetDataType(Type type)
        {
            var agrs = type.GetGenericArguments();
            if (agrs.Length == 0)
                return type.GetProperty("Data").PropertyType;

            return agrs[0];
        }
      

        public void Remove(string topic, Type type)
        {
            var key = Key.Make(type, topic);
            lock (Table)
            {
                if (Table.ContainsKey(key))
                {
                    var subject = Table[key];
                    Table.Remove(key);
                    subject.Dispose();
                }
            }
        }

        public void Clear()
        {
            if (Table.Count == 0)
                return;

            lock (Table)
            {
                var tmpItems = Table.Values.ToArray();
                if (tmpItems != null && tmpItems.Length > 0)
                    tmpItems.ForEach(item => item.Dispose());

                Table.Clear();
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Clear();
        }

      
       
    }
}
