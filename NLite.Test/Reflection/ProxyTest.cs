using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Reflection
{
    [TestFixture]
    public class ProxyTest
    {
        public interface IPerson
        {
            bool Sex { get; set; }
            void Say(string message);
        }
        public class Person : IPerson
        {
            private string _name;
            public Person(string name)
            {
                this._name = name;
            }
            public bool Sex { get; set; }
            public virtual void Say(string message)
            {
                Console.WriteLine(string.Format("\"{0}\" 说：Hello {1}",  this._name,message));
            }
        }

        [Test]
        public void InterfaceProxyTest()
        {
            var h = new InterfaceInvocationHandler { Instance = new Person("张三") };
            var p = (IPerson)NLite.Reflection.Proxy.NewProxyInstance(
                new Type[] { typeof(IPerson) },
                h: h);

            p.Say("李四");

            p.Sex = true;

            Console.WriteLine(p.Sex);
        }

        [Test]
        public void ClassProxyTest()
        {
            var h = new ClassInvocationHandler();
            var p = (IPerson)NLite.Reflection.Proxy.NewProxyInstance(typeof(Person),
                new Type[] { typeof(IPerson) },
                h: h,
                arguments: new object[] { "张三" }
                );

            p.Say("李四");

            p.Sex = true;

            Console.WriteLine(p.Sex);
        }

        class InterfaceInvocationHandler : IInvocationHandler
        {
            public object Instance;

            public object Invoke(object target, System.Reflection.MethodInfo method, params object[] parameters)
            {
                Console.WriteLine("before call:" + method.Name);

                object result = method.Invoke(Instance, parameters);

                Console.WriteLine("after call:" + method.Name);

                return result;
            }
        }

        class ClassInvocationHandler : IInvocationHandler
        {
            public object Invoke(object target, System.Reflection.MethodInfo method, params object[] parameters)
            {
                Console.WriteLine("before call:"+method.Name);

                object result = method.Invoke(target , parameters);

                Console.WriteLine("after call:" + method.Name);

                return result;
            }
        }
    }
}
