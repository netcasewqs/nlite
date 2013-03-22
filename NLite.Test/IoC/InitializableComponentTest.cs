using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class InitializableComponentTest:TestBase
    {
        public class A : NLite.IInitializable
        {
            public bool HasInit { get; private set; }

            public void Init()
            {
                HasInit = true;
            }
        }

        class LogListner : NLite.Mini.Listener.ComponentListenerAdapter
        {
            

            public override void OnMetadataRegistered(IComponentInfo info)
            {
                Console.WriteLine("OnMetadataRegistered:" + info.Implementation.Name);
            }

            public override void OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
            {
                Console.WriteLine("OnPostCreation:" + ctx.Component.Implementation.Name);
            }

            public override void OnInitialization(NLite.Mini.Context.IComponentContext ctx)
            {
                Console.WriteLine("OnInitialization:" + ctx.Component.Implementation.Name);
            }

            public override void OnPostInitialization(NLite.Mini.Context.IComponentContext ctx)
            {
                Console.WriteLine("OnPostInitialization:" + ctx.Component.Implementation.Name);
            }

            public override void OnPreCreation(NLite.Mini.Context.IComponentContext ctx)
            {
                Console.WriteLine("OnPreCreation:" + ctx.Component.Implementation.Name);
            }

            public override void OnPreDestroy(IComponentInfo info, object instance)
            {
                if (info.Implementation != null)
                    Console.WriteLine("OnPreDestroy:" + info.Implementation.Name);
                else
                    Console.WriteLine("OnPreDestroy:" + info.Id);
            }

            public override void OnPostDestroy(IComponentInfo info)
            {
                Console.WriteLine("OnPostDestroy:" + info.Implementation.Name);
            }
        }

        [Test]
        public void Test()
        {
            Console.WriteLine("Register Listner...");
            (ServiceRegistry.Current as IKernel).ListenerManager.Register(new LogListner());

            ServiceRegistry.Register<A>();

            Console.WriteLine("begin create...");
            Assert.IsTrue(ServiceLocator.Get<A>().HasInit);
            Console.WriteLine("end create...");
        }
    }
}
