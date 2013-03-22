using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite;

namespace NLite.Test.IoC.Contracts
{
    [TestFixture]
    public class ComponentMemberRegisterTest:TestBase
    {
        class ComponentA
        {
            public ComponentA()
            {
                Name = "ComponentA.Name";
            }

            [Component(Id ="ComponentA_Name")]
            public string Name { get; set; }

            [Component(Id = "ComponentA_Hello", Contract=typeof(Func<string>))]
            public string Hello()
            {
                return "Hello Component A;";
            }

            [Component(Id = "ComponentA_Hello2", Contract = typeof(Func<string,string>))]
            public string Hello2(string s)
            {
                return "Hello " + s;
            }

            [Component(Id = "ComponentA_Hello3", Contract = typeof(Action<string>))]
            public void Hello3(string s)
            {
                Console.WriteLine( "Hello " + s);
            }


            [Component(Id = "ComponentA_Hello4")]
            public string Hello4()
            {
                return "Hello Component A;";
            }

            [Component(Id = "ComponentA_Hello5")]
            public string Hello5(string s)
            {
                return "Hello " + s;
            }

            [Component(Id = "ComponentA_Hello6")]
            public void Hello6(string s)
            {
                Console.WriteLine("Hello " + s);
            }
        }

        class ComponentB
        {
            [Inject(Id = "ComponentA_Name")]
            private string ComponentA_Name;

            [Inject(Id = "ComponentA_Hello")]
            public Func<string> Hello;

             [Inject(Id = "ComponentA_Hello2")]
             public Func<string,string> Hello2;

             [Inject(Id = "ComponentA_Hello3")]
             public Action<string> Hello3;

            [Inject(Id = "ComponentA_Hello4")]
             public Func<string> Hello4;

            [Inject(Id = "ComponentA_Hello5")]
            public Func<string,string> Hello5;
            [Inject(Id = "ComponentA_Hello6")]
            public Action<string> Hello6;


            public void Test()
            {
                Assert.AreEqual("ComponentA.Name", ComponentA_Name);
                Assert.IsNotNull(Hello);

                Assert.IsNotNull(Hello2);
                Assert.IsNotNull(Hello3);
                Assert.IsNotNull(Hello4);
                Assert.IsNotNull(Hello5);
                Assert.IsNotNull(Hello6);
            }
        }

       
        [Test]
        public void Test()
        {
            ServiceRegistry.Register<ComponentA>().Register<ComponentB>();

            var a = ServiceLocator.Get<ComponentA>();
            var b = ServiceLocator.Get<ComponentB>();

            b.Test();

            Assert.AreEqual(a.Hello(), b.Hello());
            Assert.AreEqual(a.Hello2("1"), b.Hello2("1"));
            b.Hello3("sfas");

        }


        [Setting("NIROI")]
        private IEnumerable<NIROIType> NIROI;

        protected override void Init()
        {
            base.kernel.ListenerManager.Register(new NLite.Mini.Listener.MemberExportListener());
            base.Init();
            kernel.ListenerManager.Register(new NLite.Mini.Listener.AppSettingInjectionListener());
        }

        [Test]
        public void SettingTest()
        {
            ServiceRegistry.Compose(this);

            foreach (var item in NIROI)
                Console.WriteLine(item);

            PropertyManager.Items["NIROI"] = new int[]{1};

            foreach (var item in NIROI)
                Console.WriteLine(item);
        }
    }
}
