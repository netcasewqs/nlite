using System;
using System.Collections.Generic;
using NLite.Internal;

namespace NLite.Messaging.Internal
{
    interface IMessageRepository : IDisposable
    {
        IMessageListenerManager ListnerManager { get; }
        ISubject this[string topic, Type type] { get; }
        void Remove(string topic, Type type);

        int Count { get; }
        void Clear();

    }
}
