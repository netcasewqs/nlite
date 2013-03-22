using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLite.Collections;
using NLite.Internal;
using NLite.Collections.Internal;
using System.Threading;

namespace NLite.Messaging.Internal
{
    class Subject :BooleanDisposable, ISubject
    {
        
        public string Name { get; set; }
        public ISubscriber Subscriber { get; set; }
        public IExecutor Executor { get; set; }
        public IQueue<IMessageContext> Queue { get; set; }

        public IMessageResponse Publish(IMessageRequest req)
        {
            CheckNotDisposed();

            var ctx = new MessageResult(req);
            Queue.Enqueue(ctx);
            if (req.IsAsync)
            {
                ThreadPool.QueueUserWorkItem(s => Executor.Execute());
                return ctx;
            }
            return Executor.Execute();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Queue.Dispose();
                Subscriber.Clear();
            }
        }

       
    }
  
}
