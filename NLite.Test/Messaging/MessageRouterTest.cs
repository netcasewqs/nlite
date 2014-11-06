using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;
using NLite;
using NLite.Messaging;
using NLite.Threading;
using NLite.Collections;

namespace NLite.Test.Messaging
{
    [TestFixture]
    public class MessageRouterTest
    {
        [Test]
        public void SimpleTest()
        {
            using (Bus.Subscribe(new SubscribeInfo<int>(msg =>
            {
                Console.WriteLine("Subscriber id:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                if (msg == -1)
                {
                    Bus.Remove<int>();
                }
                Console.WriteLine(msg.ToString());
            }) { Mode = SubscribeMode.Async }))
            using (Bus.Subscribe(new SubscribeInfo<int>(msg =>
            {
                Console.WriteLine("Subscriber id:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                if (msg == -1)
                {
                    Bus.Remove<int>();
                }
                Console.WriteLine(msg.ToString());
            }) { Mode = SubscribeMode.Async }))
            {
                Console.WriteLine("Publisher id:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                for (int i = 0; i < 2; i++)
                {

                    Bus.Publish(i);
                }

                Bus.Publish(-1);
            }
        }

        [Test]
        public void RepeatSubscribeTest()
        {
            Action<int> subscriber = msg =>
                {
                    if (msg == -1)
                    {
                        Bus.Remove<int>();
                    }
                    Console.WriteLine(msg.ToString());
                };

            using(Bus.Subscribe(new SubscribeInfo<int>( subscriber)))
            using (Bus.Subscribe(new SubscribeInfo<int>(subscriber)))
            {
                for (int i = 0; i < 2; i++)
                {

                    Bus.Publish(i);
                }

                Bus.Publish(-1);
            }
        }

        [Test]
        public void TopicTest()
        {
            const string topic = "TopicTest";

            using (Bus.Subscribe<int>(topic, msg =>
            {
                if (msg == -1)
                {
                    Bus.Remove<int>(topic);
                }
                Console.WriteLine(msg.ToString() + " for " + topic + "!");

            }))
            {

                for (int i = 0; i < 100; i++)
                {
                    Bus.Publish(topic, i);
                }

                Bus.Publish(topic, -1);
            }
        }

        [Test]
        public void Topic2Test()
        {
            const string topic = "Topic2";

            Action handler = () => Console.WriteLine("Has Received");

            Bus.Subscribe(topic, handler);
            Bus.Subscribe(topic, handler);
            Bus.Subscribe(topic, handler);

            Bus.Publish(topic);
        }

        //class CallbackMessage : Callback
        //{
        //    public int Result { get; set; }
        //    public CallbackMessage(Action<int> callback, int a)
        //        : base(callback, a)
        //    {
        //    }
        //}

        //[Test]
        //public void CallbackTest()
        //{
        //    const string topic = "CallbackTest";

        //    Action<CallbackMessage> handler = p => Console.WriteLine("Has Received");
        //    Action<CallbackMessage> handler2 = p => Console.WriteLine("Has Received 2");
        //    Action<CallbackMessage> handler3 = p => Console.WriteLine("Has Received 3");

        //    Bus.Subscribe(topic, handler);
        //    Bus.Subscribe(topic, handler2);
        //    Bus.Subscribe(topic, handler3);

        //    var result = 0;
        //    var message = new CallbackMessage(p => result += 1, 1);
        //    Bus.Publish(topic, message);

        //    Console.WriteLine(result);
        //}

        //[Test]
        //public void AsynCallbackTest()
        //{
        //    const string topic = "AsynCallbackTest";

        //    var result = 0;
        //    var message = new CallbackMessage(p => result += 1, 1);

        //    Action<CallbackMessage> handler = p => Console.WriteLine("Has Received");
        //    Action<CallbackMessage> handler2 = p => Console.WriteLine("Has Received 2");
        //    Action<CallbackMessage> handler3 = p => Console.WriteLine("Has Received 3");

        //    using (Bus.Subscribe(topic, SubscribeMode.Async, handler))
        //    using (Bus.Subscribe(topic, SubscribeMode.Async, handler2))
        //    using (Bus.Subscribe(topic, handler3))
        //        Bus.Publish(topic, message);

        //    Thread.Sleep(1000);

        //    Console.WriteLine(result);
        //}

        [Test]
        public void ReturnResultTest()
        {
            const string topic = "ReturnResultTest";

            Func<int, int> handler = p => 1;
            Action<int> handler2 = p => { };
            Func<int, int> handler3 = p => 3;

            Bus.Subscribe(topic, SubscribeMode.Async, handler);
            Bus.Subscribe(topic, SubscribeMode.Async, handler2);
            Bus.Subscribe(topic, handler3);

            //var message = MessageFactory.Make(topic, 0);
            //var result = Bus.Publish(message);

            //Thread.Sleep(1000);

            //var results = result.Results.OfType<int>();
            //foreach (var item in results)
            //    Console.WriteLine(item);
        }

        [Test]
        public void ReturnResultTest2()
        {
            const string topic = "ReturnResultTest2";

            Func<IMessage<int>, int> handler = p => 1;
            Func<int, int> handler2 = p => { Thread.Sleep(100); return 2; };
            Func<int, int> handler3 = p => 3;

            Bus.Subscribe(topic, handler);
            Bus.Subscribe(topic, handler2);
            Bus.Subscribe(topic, handler3);

            var req = new MessageRequest<int>
            {
                Topic = topic,
                Data = 0,
               
            };
            var resp = Bus.Publish(req);

            var results = resp.Results.OfType<int>();
            foreach (var item in results)
                Console.WriteLine(item);
        }


        [Test]
        public void ReturnResultTest3()
        {
            const string topic = "ReturnResultTest3";

            Func<int, int> handler = p => 1;
            Action<int> handler2 = p => { Thread.Sleep(1000); };
            Func<int, int> handler3 = p => 3;

            Bus.Subscribe(topic, SubscribeMode.Async, handler);
            Bus.Subscribe(topic, SubscribeMode.Async, handler2);
            Bus.Subscribe(topic, handler3);

            var req = new MessageRequest<int>
            {
                Topic = topic,
                Data = 0,
            };
            var resp = Bus.Publish(req);

            var results = resp.Results.OfType<int>();
            foreach (var item in results)
                Console.WriteLine(item);


        }

        [Test]
        public void ReturnResultTest4()
        {
            const string topic = "ReturnResultTest4";

            Func<int, int> handler = p => 1;
            Action<int> handler2 = p => { Thread.Sleep(1000); };
            Func<int, int> handler3 = p => 3;

            using (Bus.Subscribe(topic, SubscribeMode.Async, handler))//避免内存泄露
            using (Bus.Subscribe(topic, SubscribeMode.Async, handler2))//避免内存泄露
            using (Bus.Subscribe(topic, handler3))//避免内存泄露
            {
                var req = new MessageRequest<int> { Topic = topic, Data = 0,  };
                var resp = Bus.Publish(req);
                resp.Results.OfType<int>().ForEach(item => Console.WriteLine(item));
            }

        }

        public static IMessageBus Bus { get; protected set; }
        [SetUp]
        public virtual void Setup()
        {
          
            Bus = new SimpleBus();
            Bus.RegisterListner(new LogListener());
        }

        [TearDown]
        public void TearDown()
        {
            Bus.Dispose();
        }
    }


    class LogListener : MessageListner
    {
        public LogListener()
            : base(MessageListnerType.All)
        {
        }

        public override void OnSending(MessageEventArgs e)
        {
        }

        public override void OnReceiving(MessageReceivingEventArgs e)
        {
        }

        public override void OnReceivingException(MessageExceptionEventArgs e)
        {
        }

        public override void OnSent(MessageEventArgs e)
        {
        }

        public override void OnReceived(MessageEventArgs e)
        {
        }
    }

}
