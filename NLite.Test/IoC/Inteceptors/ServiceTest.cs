using NLite.Interceptor;
using NLite.Mini.Proxy;
using NLite.Test.IoC.Contract;
using NLite.Test.IoC.Core;
using NLite.Test.IoC.Core.System;
using NUnit.Framework;

namespace NLite.Test.IoC.Inteceptors
{
    [TestFixture]
    public class ServiceTest : TestBase
    {
        protected override void Init()
        {
            base.Init();
            ServiceRegistry.Current
                .ListenerManager.Register(new NLite.Mini.Listener.AopListener()); ;
        }

        [Test]
        public void Test()
        {
            var aspect = Aspect.For(typeof(BusinessListProxy<,>))
                .PointCut()
                .Method("*")
                .Deep(2)
                .Advice<LoggerInterceptor>();

            ServiceRegistry.Current.Register(typeof(IBusinessList<,>), typeof(BusinessListProxy<,>));

            var items = ServiceLocator.Get<IBusinessList<ButtonList, Button>>();
            Assert.IsNotNull(items);

            // 这里跟进不了切片
            var ds = items.Fetch(new ReflectCriteria<ButtonList>("ButtonList", "Search", null));
            Assert.IsNotNull(ds);
        }
    }
}
