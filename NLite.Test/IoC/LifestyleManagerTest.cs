using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Threading;
using System.Threading;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class LifestyleManagerTest:TestBase
    {

        [Test]
        public void SingletonLifestyleTest()
        {
            ServiceRegistry.Register<Person>();

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person2 != null);

            Assert.AreSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }

        [Test]
        public void TransientLifestyleTest()
        {
            ServiceRegistry.Current.Register<IPerson, Person>("person", LifestyleFlags.Transient);

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person2 != null);

            Assert.AreNotSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }

        [Test]
        public void ThreadLifestyleTest()
        {
            ServiceRegistry.Current.Register<IPerson, Person>("person", LifestyleFlags.Thread);

            IPerson person = null,
                person2 = null, person3 = null, person4 = null;

            PopulatePerThreadLifestyle(ref person, ref person2);

            var mre = new ResetEvent(false);
            ThreadPool.QueueUserWorkItem((s) =>
            {
                PopulatePerThreadLifestyle(ref person3, ref person4);
                mre.Set();
            });
            mre.Wait();

            Assert.AreSame(person, person2);
            Assert.AreSame(person3, person4);
            Assert.AreNotSame(person, person3);
        }


        [Test]
        public void GenericSingletonLifestyleTest()
        {
            ServiceRegistry.Current.Register(typeof(IList<>), typeof(List<>));

            var instance = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance);

            var instance2 = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance, instance2);
        }

        [Test]
        public void GenericTransientLifestyleTest()
        {
            ServiceRegistry.Current.Register(new ComponentInfo(null,typeof(IList<>),typeof(List<>), LifestyleFlags.Transient));

            var instance = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance);

            var instance2 = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance, instance2);
        }


        [Test]
        public void GenericThreadLifestyleTest()
        {
            ServiceRegistry.Current.Register(new ComponentInfo(null, typeof(IList<>), typeof(List<>), LifestyleFlags.Thread));

            IList<int> coll = null,
                coll2 = null, coll3 = null, coll4 = null;

            PopulatePerThreadLifestyle(ref coll, ref coll2);

            var mre = new ResetEvent(false);
            ThreadPool.QueueUserWorkItem((s) =>
            {
                PopulatePerThreadLifestyle(ref coll3, ref coll4);
                mre.Set();
            });
            mre.Wait();

            Assert.AreSame(coll, coll2);
            Assert.AreSame(coll3, coll4);
            Assert.AreNotSame(coll, coll3);
        }

        private void PopulatePerThreadLifestyle(ref IList<int> first, ref IList<int> second)
        {
            first = ServiceLocator.Get(typeof(IList<int>)) as IList<int>;
            Assert.IsTrue(first != null);

            second = ServiceLocator.Get(typeof(IList<int>)) as IList<int>;
            Assert.IsTrue(second != null);
        }

        private void PopulatePerThreadLifestyle(ref IPerson person, ref IPerson person2)
        {
            person = ServiceLocator.Get(typeof(IPerson)) as IPerson;
            Assert.IsTrue(person != null);

            person2 = ServiceLocator.Get(typeof(IPerson)) as Person;
            Assert.IsTrue(person2 != null);
        }

    }
}
