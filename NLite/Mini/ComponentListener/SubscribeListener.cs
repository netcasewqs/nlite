using System;
using System.Linq;
using System.Reflection;
using NLite.Messaging;
using NLite.Reflection;
using NLite.Mini.Resolving;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscribeListener:ComponentListenerAdapter
    {
        static readonly Type MessageBusType = typeof(IMessageBus);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
        {
            var instance = ctx.Instance;
            if (instance == null || SubscribeListener.MessageBusType.IsAssignableFrom(instance.GetType()))
                return;

            var bus = Kernel.Get<IMessageBus>();
            if (bus == null)
                return;
            if (!ctx.Component.ExtendedProperties.ContainsKey("subscribeProviders"))
                return;

            var subscribers = ctx.Component.ExtendedProperties["subscribeProviders"] as ISubscribeInfoFactoryProvider[];

            if (subscribers == null || subscribers.Length == 0)
                return;
           
            var disCollector = instance as IDisposeCollector;
            var compositeDis = instance as ICompositeDisposable;
            foreach(var subscriber in subscribers)
            {

                var unsubscriber = bus.Subscribe(subscriber.Factory(instance));
                if (compositeDis != null)
                    compositeDis.AddDisposable(unsubscriber);
                else if (disCollector != null)
                    disCollector.Disposes.AddDisposable(unsubscriber);
            }

        }
    }
}
