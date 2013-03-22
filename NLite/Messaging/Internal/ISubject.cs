using System.Collections.Generic;
using NLite.Collections;
using NLite.Internal;
using NLite.Collections.Internal;

namespace NLite.Messaging.Internal
{
    /// <summary>
    /// 
    /// </summary>
    interface ISubject:IPublisher
    {
        string Name { get; set; }
        ISubscriber Subscriber { get; set; }
        IQueue<IMessageContext> Queue { get; set; }
        IExecutor Executor { get; set; }
    }

   
}
