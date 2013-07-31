using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Messaging;

namespace NLite.Test.Messaging
{
    [TestFixture]
    public class SubscribeListnerTest
    {
        IMessageBus bus;
        int Result;
        bool HasAction;

        [SetUp]
        public void Init()
        {
           
            bus = new SimpleBus();

            bus.Subscribe(this);
        }

        [TearDown]
        public void TearDown()
        {
            bus.Shutdown();
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

        [Subscribe(Topic = "testTopic", Mode = SubscribeMode.Async)]
        void OnTest(object sender, Tuple<int, int> e)
        {
            Result = e.First + e.Second;
        }


        [Subscribe(Mode = SubscribeMode.Async)]
        void OnCalculate2(Tuple<int, int> e)
        {
            Result = e.First - e.Second;
        }

        [Subscribe(Topic = "ActionTopic")]
        void OnAction()
        {
            HasAction = true;
        }


        [Test]
        public void Test()
        {
            bus.Publish("testTopic", this, new Tuple<int, int>(3, 2));
            Assert.AreEqual(5, Result);
        }

        [Test]
        public void Test2()
        {
            bus.Publish(this, new Tuple<int, int>(3, 2));
            Assert.AreEqual(1, Result);
        }

        [Test]
        public void Test3()
        {
            bus.Publish("ActionTopic");
            Assert.IsTrue(HasAction);
        }
    }

    [TestFixture]
    public class SubscribeListnerTest3
    {
        static IMessageBus bus;
        static int Result;
        static bool HasAction;

        [SetUp]
        public void Init()
        {

            bus = new SimpleBus();

            bus.Subscribe(this);

            Result = 0;
            HasAction = false;
        }

        [TearDown]
        public void TearDown()
        {
            bus.Shutdown();
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

        [Subscribe(Topic = "testTopic", Mode = SubscribeMode.Async)]
        static void OnTest(object sender, Tuple<int, int> e)
        {
            Result = e.First + e.Second;
        }


        [Subscribe(Mode = SubscribeMode.Async)]
        static void OnCalculate2(Tuple<int, int> e)
        {
            Result = e.First - e.Second;
        }

        [Subscribe(Topic = "ActionTopic")]
        static void OnAction()
        {
            HasAction = true;
        }


        [Test]
        public void Test()
        {
            bus.Publish("testTopic", this, new Tuple<int, int>(3, 2));
            Assert.AreEqual(5, Result);
        }

        [Test]
        public void Test2()
        {
            bus.Publish(this, new Tuple<int, int>(3, 2));
            Assert.AreEqual(1, Result);
        }

        [Test]
        public void Test3()
        {
            bus.Publish("ActionTopic");
            Assert.IsTrue(HasAction);
        }
    }


    [TestFixture]
    public class SubscribeListnerTest2
    {
        IMessageBus bus;
        int Result;
        bool HasAction;

        [SetUp]
        public void Init()
        {
            NLiteEnvironment.Refresh();
            var kerel = ServiceLocator.Current as IKernel;
            kerel.ListenerManager.Register(new NLite.Mini.Listener.SubscribeListener());
            ServiceRegistry.Register<SimpleBus>();
            bus = ServiceLocator.Get<IMessageBus>();

            ServiceRegistry.Compose(this);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceRegistry.Current.UnRegister(typeof(IMessageBus));
        }


        struct Tuple<T1,T2>
        {
            public Tuple(T1 first, T2 second)
            {
                First = first;
                Second = second;
            }

            public T1 First;
            public T2 Second;
        }

        [Subscribe(Topic = "testTopic", Mode = SubscribeMode.Async)]
        void OnTest(object sender, Tuple<int, int> e)
        {
            Result = e.First + e.Second;
        }


        [Subscribe(Mode = SubscribeMode.Async)]
        void OnCalculate2(Tuple<int, int> e)
        {
            Result = e.First - e.Second;
        }

        [Subscribe(Topic = "ActionTopic")]
        void OnAction()
        {
            HasAction = true;
        }


        [Test]
        public void Test()
        {
            bus.Publish("testTopic",this,new Tuple<int,int>(3,2));
            Assert.AreEqual(5, Result);
        }

        [Test]
        public void Test2()
        {
            bus.Publish(this, new Tuple<int, int>(3, 2));
            Assert.AreEqual(1, Result);
        }

        [Test]
        public void Test3()
        {
            bus.Publish("ActionTopic");
            Assert.IsTrue(HasAction);
        }
    }
}
