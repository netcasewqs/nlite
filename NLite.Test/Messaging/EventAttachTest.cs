using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Messaging;
//using System.Linq.Expressions;
//using NLite.Reflection;

namespace NLite.Test.Messaging
{
    [TestFixture]
    public class EventAttachTest:NLite.CompositeDisposable
    {
        IMessageBus bus;
        int Result;

        [SetUp]
        public void Init()
        {
            NLiteEnvironment.Refresh();
            var kerel = ServiceLocator.Current as IKernel;
            kerel.ListenerManager.Register(new NLite.Mini.Listener.SubscribeListener());
            ServiceRegistry.Register<SimpleBus>();
            bus = ServiceLocator.Get<IMessageBus>();

           
            ServiceRegistry.Compose(this);
            bus.Attach(this, s => s.Calculate1);
            bus.Attach("sub", this, s => s.Calculate2);
        }

       

        [TearDown]
        public void TearDown()
        {
            ServiceRegistry.Current.UnRegister(typeof(IMessageBus));
        }

        struct Tuple<T1, T2>
        {
            public Tuple(T1 first, T2 second)
            {
                First = first;
                Second = second;
            }

            public T1 First;
            public T2 Second;
        }

        event Action<object, Tuple<int, int>> Calculate2;
        event Action<Tuple<int, int>> Calculate1;


        [Subscribe]
        void OnCalculate(Tuple<int, int> e)
        {
            Result = e.First + e.Second;
        }

        [Subscribe(Topic="sub")]
        void OnCalculate2(Tuple<int, int> e)
        {
            Result = e.First - e.Second;
        }

        [Test]
        public void Test()
        {
            Calculate1(new Tuple<int, int>(3, 2));
            Assert.AreEqual(5, Result);
        }

        [Test]
        public void Test2()
        {
            Calculate2(this, new Tuple<int,int>(3,2));
            Assert.AreEqual(1, Result);
        }

       
        
    }

   
}
