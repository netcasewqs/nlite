using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using NLite.Internal;

namespace NLite.Messaging.Internal
{
   
    struct EventRaiser
    {

        public static readonly MethodInfo InvokeMethod = typeof(EventRaiser).GetMethod("Invoke");
        public static readonly MethodInfo Invoke1Method = typeof(EventRaiser).GetMethod("Invoke1");
        public static readonly MethodInfo Invoke2Method = typeof(EventRaiser).GetMethod("Invoke2");

        private IMessageBus MB;
        private string Topic;

        public EventRaiser(IMessageBus router, string topic)
        {
            this.MB = router;
            Topic = topic;
        }

        public void Invoke()
        {
            Guard.NotNullOrEmpty(Topic,"Topic");

            MB.Publish(null,Topic);
        }

        public void Invoke1<TEventArgs>(TEventArgs e)
        {
            MB.Publish(Topic,e);
        }

        public void Invoke2<TEventArgs>(object sender, TEventArgs e)
        {
            MB.Publish(Topic,sender, e);
        }
    }

  
    struct DefaultObserverHandler2<TData> : IObserverHandler<IMessage>,IEquatable<DefaultObserverHandler2<TData>>
    {
        internal Func<object,TData,object> InnerHandler;

        public object Target
        {
            get { return InnerHandler.Target; }
        }

        public object Invoke(object sender, IMessage message)
        {
            if(typeof(TData).IsInstanceOfType(message))
                return InnerHandler.Invoke(sender, (TData)message);
            return InnerHandler.Invoke(sender, (TData)message.Data);
        }

        public MethodInfo Method { get { return InnerHandler.Method; } }

        public bool Equals(DefaultObserverHandler2<TData> other)
        {
            return other.InnerHandler == InnerHandler;
        }

        public override bool Equals(object obj)
        {
            return Equals((DefaultObserverHandler2<TData>)obj);
        }

        public override int GetHashCode()
        {
            return InnerHandler.GetHashCode();
        }
              
    }

    struct DefaultObserverHandler1<TData> : IObserverHandler<IMessage>, IEquatable<DefaultObserverHandler1<TData>>
    {
        internal Func<TData,object> InnerHandler;

        public object Target
        {
            get { return InnerHandler.Target; }
        }

        public object Invoke(object sender, IMessage message)
        {
            if (typeof(TData).IsInstanceOfType(message))
                return InnerHandler.Invoke((TData)message);
            return InnerHandler.Invoke((TData)message.Data);
        }

        public MethodInfo Method { get { return InnerHandler.Method; } }

        public bool Equals(DefaultObserverHandler1<TData> other)
        {
            return other.InnerHandler == InnerHandler;
        }

        public override bool Equals(object obj)
        {
            return Equals((DefaultObserverHandler1<TData>)obj);
        }

        public override int GetHashCode()
        {
            return InnerHandler.GetHashCode();
        }

    }


    struct DefaultObserverHandler : IObserverHandler<IMessage>, IEquatable<DefaultObserverHandler>
    {
        internal Func<object> InnerHandler;

        public object Target
        {
            get { return InnerHandler.Target; }
        }

        public object Invoke(object sender, IMessage message)
        {
            return InnerHandler.Invoke();
        }

        public MethodInfo Method { get { return InnerHandler.Method; } }

        public bool Equals(DefaultObserverHandler other)
        {
            return other.InnerHandler == InnerHandler;
        }

        public override bool Equals(object obj)
        {
            return Equals((DefaultObserverHandler)obj);
        }

        public override int GetHashCode()
        {
            return InnerHandler.GetHashCode();
        }

    }
}
