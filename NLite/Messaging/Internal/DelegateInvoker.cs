using System;

namespace NLite.Messaging.Internal
{
    class DelegateInvoker : IDelegateInvoker
    {
        public DelegateInvoker() { }

        public object Invoke<TMessage>(IObserverHandler<TMessage> handler, object sender, TMessage msg) where TMessage : IMessage
        {
            return handler.Invoke(sender, msg);
        }

    }
}
