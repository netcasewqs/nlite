using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite;
using NLite.Interceptor;
using NLite.Mini.Proxy;

namespace NLite.Test.IoC.Inteceptors
{
    [TestFixture]
    public class GenericProxyTest : TestBase
    {


        [Contract]
        interface IMyList<T> : IList<T>
        {
            void Test<T2>(T2 o);
            void Test2<T2>(T t,T2 o);
        }
        sealed class MyList<T> : List<T>, IMyList<T>
        {
            public void Test<T2>(T2 o)
            {
                Console.WriteLine(o);
            }

             public void Test2<T2>(T t, T2 o)
            {
                  Console.WriteLine(t);
                Console.WriteLine(o);
            }
        }

        protected override void Init()
        {
            base.Init();
            kernel.ListenerManager.Register(new NLite.Mini.Listener.AopListener());
            ServiceRegistry.Current.RegisterInstance(ProxyFactory.Default);
        }
        [Test]
        public void Test()
        {
            var aspect = Aspect.For(typeof(MyList<>))
                .PointCut()
                .Method("*")
                .Deep(2)
                .Advice<LoggerInterceptor>();

            ServiceRegistry.Current.Register(typeof(IMyList<>), typeof(MyList<>));

            var list = ServiceLocator.Get<IMyList<int>>();
            Assert.IsNotNull(list);

            list.Add(1);

            list.Test<string>("hello");
            list.Test2<string>(5, "word");
        }
    }
}
