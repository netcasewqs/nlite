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



        [Test]
        public void InterceptAll()
        {
            var aspect = Aspect.For<ComplexClass_Default_Constructure>();
            aspect
                .PointCut()
                .Method("*")
                .Deep(1)
                .Advice<LoggerInterceptor>();

            WrapAndInvokeEverything();
        }

   

        private void WrapAndInvokeEverything()
        {

            ServiceRegistry.Register<ComplexClass_Default_Constructure>();

            var cc = ServiceLocator.Get<ComplexClass_Default_Constructure>();
            Assert.IsNotNull(cc);

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

            WrapAndInvokeEverything();
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

            WrapAndInvokeEverything();
        }
    }

    public class BaseClass:MarshalByRefObject
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

        public virtual String Name { get; set; }
    }

    //[Serializable]
    //public abstract class ComplexClass : ComplexClass_Default_Constructure
    //{
    //    private ISimpleContract simpleContract;
    //    public ComplexClass(ISimpleContract simpleContract)
    //    {
    //        this.simpleContract = simpleContract;
    //    }


    //}




}
