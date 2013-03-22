using System;
using System.Collections.Generic;
using NLite.Collections;
using NLite.Internal;
using NLite.Collections.Internal;

namespace NLite.Messaging.Internal
{

    class SubjectBuilder : ISubjectBuilder
    {
      

        public ISubject Build(string name)
        {
            var subject = CreateSubject(name);

            subject.Queue = CreateQueue();
            subject.Subscriber = CreateSubscriber();
            subject.Executor = CreateExecutor();

            subject.Subscriber.Name = subject.Name;
            subject.Executor.Subject = subject;

           

            return subject;
        }

        private ISubject CreateSubject(string name)
        {
            return new Subject { Name = name };
        }

        private IQueue<IMessageContext> CreateQueue()
        {
            return new SyncQueue<IMessageContext>();
        }

        private ISubscriber CreateSubscriber()
        {
            return new Subscriber();
        }

        private IExecutor CreateExecutor()
        {
            return new Executor();
        }
    }
}
