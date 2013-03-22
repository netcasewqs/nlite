using System;
using System.Linq;
using NLite;
using NLite.Threading;
using System.Threading;

namespace NLite.Messaging.Internal
{
    #if !SILVERLIGHT
    [Serializable]
    #endif
    class Executor : IExecutor
    {
        public ISubject Subject { get; set; }
        public IMessageListenerManager ListnerManager { get; set; }
        public IDelegateInvoker DelegateInvoker { get; set; }

        public IMessageResponse Execute()
        {
            if (Subject.Queue.Count == 0)
                return MessageResult.Empty;

            var ctx = (MessageResult)Subject.Queue.Dequeue();
            
            var handlers = Subject.Subscriber.Observers;


            if (handlers.Count == 0)
                OnCompleted(ctx);
            else
            {
                var e = new MessageEventArgs(ctx);

                ListnerManager.OnSending(e);

                if (e.Context.Response.Canceled)
                {
                    ListnerManager.OnSent(e);
                    OnCompleted(ctx);
                    return ctx;
                }

                Dispatch(ctx);
            }
                

            return ctx;
        }

        private static void OnCompleted(MessageResult ctx)
       {
           ctx.IsCompleted = true;
       }

     

        void Dispatch(MessageResult ctx)
        {
            var handlers = Subject.Subscriber.Observers;

            var syncHandlers = handlers.Where(i => i.Value == SubscribeMode.Sync).Select(i => i.Key).ToArray();
            var asyncHandlers = handlers.Where(i => i.Value == SubscribeMode.Async).Select(i => i.Key).ToArray();

            var handlerCount = syncHandlers.Length + asyncHandlers.Length;
            var barrier = new AtomicInteger(handlerCount);

            foreach (var item in syncHandlers)
            {
                if (!DoExecute(ctx, item, barrier))
                    break;
            }


            Paraller.ForEach(asyncHandlers, handler => DoExecute(ctx, handler, barrier));
        }

        bool DoExecute(MessageResult ctx, IObserverHandler<IMessage> handler, AtomicInteger barrier)
        {
            var e = new MessageReceivingEventArgs (ctx, handler.Target);
            try
            {
                ListnerManager.OnReceiving(e);
                if (!e.Ignored)
                    ctx.results.Add(DelegateInvoker.Invoke<IMessage>(handler, ctx.Request.Sender, ctx.Request.ToMessage()));

                
                return true;
            }
            catch (Exception ex)
            {
                var re = new MessageExceptionEventArgs (ctx,ex);
                ListnerManager.OnReceivingException(re);
                ctx.InnerExceptions.Add(ex);
                return !re.Canceled; 
            }
            finally
            {
                barrier--;

                if (barrier == 0)
                {
                    ListnerManager.OnReceived(new MessageEventArgs(ctx));
                    ListnerManager.OnSent(e);
                    OnCompleted(ctx);
                }
                else
                    ListnerManager.OnReceived(new MessageEventArgs(ctx));
                
                e = null;
            }
        }

    }
}
