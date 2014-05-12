using System;
using NUnit.Framework;
using NLite.Interceptor;
using NLite.Interceptor.Metadata;
using NLite.Test.IoC.Contracts.Components;
using System.Collections.Generic;
using NLite.Mini;
using NLite.Mini.Proxy;
using NLite.Reflection;

namespace NLite.Test.IoC.Inteceptors
{
    [TestFixture]
    public class SimpleTest : TestBase
    {
        protected override void Init()
        {
            base.Init();
            kernel.ListenerManager.Register(new NLite.Mini.Listener.AopListener());
        }

        [TearDown]
        public void TearDown()
        {
            Aspect.Clear();
        }

        [Test]
        public void InterceptAllByIoC()
        {
            var aspect = Aspect.For<ComplexClass_Default_Constructure>();
            aspect
                .PointCut()
                .Method("*")
                .Deep(1)
                .Advice<LoggerInterceptor>();

            ServiceRegistry.Register<ComplexClass_Default_Constructure>();

            var cc = ServiceLocator.Get<ComplexClass_Default_Constructure>();
            Assert.IsNotNull(cc);


            WrapAndInvokeEverything(cc);
        }

        [Test]
        public void InterceptAllByDynamicProxy()
        {
            var aspect = Aspect.For<ComplexClass_Default_Constructure>();
            aspect
                .PointCut()
                .Method("*")
                .Deep(1)
                .Advice<LoggerInterceptor>();


            var cc = (ComplexClass_Default_Constructure)Proxy.NewProxyInstance(typeof(ComplexClass_Default_Constructure),
                null,
                new InterceptorInvocationHandr(typeof(ComplexClass_Default_Constructure),Type.EmptyTypes));
            Assert.IsNotNull(cc);

            WrapAndInvokeEverything(cc);
        }

        [Test]
        public void InterceptAllByDynamicProxyWithInterceptorAttribute()
        {

            var cc = (ComplexClass_Default_Constructure2)Proxy.NewProxyInstance(typeof(ComplexClass_Default_Constructure2),
                null,
                new InterceptorInvocationHandr(typeof(ComplexClass_Default_Constructure3), Type.EmptyTypes));
            Assert.IsNotNull(cc);

            WrapAndInvokeEverything(cc);
        }

        [Test]
        public void InterceptAllByDynamicProxyWithDemoInterceptorAttribute()
        {
            var cc = (ComplexClass_Default_Constructure3)Proxy.NewProxyInstance(typeof(ComplexClass_Default_Constructure3),
                null,
                new InterceptorInvocationHandr(typeof(ComplexClass_Default_Constructure3), Type.EmptyTypes));
            Assert.IsNotNull(cc);

            WrapAndInvokeEverything(cc);
        }

        [Test]
        public void InterceptAllByDynamicProxyWithDemoInterceptorAttribute2()
        {
            var cc = (ComplexClass_Default_Constructure4)Proxy.NewProxyInstance(typeof(ComplexClass_Default_Constructure4),
                null,
                new InterceptorInvocationHandr(typeof(ComplexClass_Default_Constructure4), Type.EmptyTypes));
            Assert.IsNotNull(cc);

            WrapAndInvokeEverything(cc);
        }

        private void WrapAndInvokeEverything(ComplexClass_Default_Constructure cc)
        {
            cc.DoNothing();
            cc.DoSomething();
            int arg = 1;

            cc.DoSomething(arg);
            cc.DoSomething(arg, "hiya");

            cc.Pong(ref arg);

            cc.Name = "John Johnson";
            Assert.AreEqual("John Johnson", cc.Name);
            cc.Started = true;
            Assert.IsTrue(cc.Started);
        }

        

        [Test]
        public void InterceptAllReadProperties()
        {
            var aspect = Aspect.For<ComplexClass_Default_Constructure>();
            aspect
                .PointCut(CutPointFlags.PropertyRead)
                .Advice<LoggerInterceptor>();

            ServiceRegistry.Register<ComplexClass_Default_Constructure>();

            var cc = ServiceLocator.Get<ComplexClass_Default_Constructure>();
            Assert.IsNotNull(cc);


            WrapAndInvokeEverything(cc);
        }

        [Test]
        public void InterceptAllWriteProperties()
        {
            var aspect = Aspect.For<ComplexClass_Default_Constructure>();
            aspect
                .PointCut(CutPointFlags.PropertyWrite)
                .Method("*")
                .Deep(1)
                .Advice<LoggerInterceptor>();

            ServiceRegistry.Register<ComplexClass_Default_Constructure>();

            var cc = ServiceLocator.Get<ComplexClass_Default_Constructure>();
            Assert.IsNotNull(cc);


            WrapAndInvokeEverything(cc);
        }
    }

    public class BaseClass
    {
        public virtual bool Started { get; set; }

        protected internal virtual void DoNothingCore() { }

        public void DoNothing()
        {
            DoNothingCore();
        }

        public virtual int DoSomething()
        {
            return 1;
        }

        public virtual int DoSomething(int value1)
        {
            return value1;
        }

        public virtual int DoSomething(int value1, String text)
        {
            return value1;
        }
    }

    [Serializable]
    public class ComplexClass_Default_Constructure : BaseClass
    {

        public virtual int Pong(ref int i)
        {
            return i;
        }

        public virtual int DoMethodExecutionCount { get; set; }

        [DemoInterceptor]
        public virtual String Name { get; set; }
    }

    [Interceptor(typeof(LoggerInterceptor))]
    public class ComplexClass_Default_Constructure2 : ComplexClass_Default_Constructure
    {
    }

    [DemoInterceptor]
    public class ComplexClass_Default_Constructure3 : ComplexClass_Default_Constructure
    {
    }


    public class ComplexClass_Default_Constructure4 : ComplexClass_Default_Constructure
    {
        
      
    }

    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method| AttributeTargets.Property| AttributeTargets.Event)]
    public class DemoInterceptor : Attribute, IInterceptor
    {
        public object Intercept(IInvocationContext invocationContext)
        {
            Console.WriteLine("call :" + invocationContext.Method.Name);
            return invocationContext.Proceed();
        }
    }
}
