using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite;
//using NLite.Interceptor.Mini;
using NLite.Threading;
using NLite.Mini;
using NLite.Mini.Resolving;

namespace NLite.Test.IoC
{
    public class TestBase
    {
        private LifecycleLogListner log = new LifecycleLogListner();

        class LifecycleLogListner : NLite.Mini.Listener.ComponentListenerAdapter
        {
            //public LifecycleLogListner() : base(ComponentListenStage.PostCreation | ComponentListenStage.PreDestroy) { }

            public override void OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
            {
                Console.WriteLine(string.Format("DI container created {0} component", ctx.Instance.GetType().Name));
            }

            public override void OnPreDestroy(IComponentInfo info, object instance)
            {
                Console.WriteLine(string.Format("DI container destroying {0} component", instance.GetType().Name));
            }


            public override void OnFetch(Mini.Context.IComponentContext ctx)
            {
                Console.WriteLine(string.Format("DI container Fetch {0} component", ctx.Instance.GetType().Name));

            }
          
            
        }
        protected Kernel kernel = null;
        
        [SetUp]
        public void Setup()
        {
            ReferenceManager.Instance.Enabled = true;
            LifestyleType.Default = LifestyleFlags.Singleton;
            kernel = new Kernel();
            kernel.ListenerManager.Register(new NLite.Mini.Listener.StartableListener());
            kernel.ListenerManager.Register(new NLite.Mini.Listener.DisposalListener());
            kernel.ListenerManager.Register(new NLite.Mini.Listener.InitializationListener());
            kernel.ListenerManager.Register(new NLite.Mini.Listener.SupportInitializeListener());
            kernel.ListenerManager.Register(new NLite.Mini.Listener.SubscribeListener());
            ServiceLocator.Current = kernel;
            ServiceRegistry.Current = kernel;

            //ServiceRegistry.Current.ListenerManager.Register(new LifecycleLogListner());
            //ServiceRegistry.Register<ProxyFactory>();
           

            Init();
        }

        protected virtual void Init() { }

        [TearDown]
        public void TearDown()
        {
            kernel.Dispose();
            ServiceLocator.Current = null;
            ServiceRegistry.Current = null;
        }
    }
}
