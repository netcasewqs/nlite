using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Test.IoC.Contracts.Components;
using NLite;

namespace NLite.Test.IoC.Contracts
{


    [TestFixture]
    public class LazyManyEmptyArrayComponentTest:TestBase
    {
        [InjectMany]
        Lazy<ISimpleContract[]> array { get; set; }

        [Test]
        public void Test()
        {
            ServiceRegistry.Compose(this);
            Assert.IsNull(array);
        }
    }

    [TestFixture]
    public class LazyManyArrayComponentTest : TestBase
    {
        [InjectMany]
        Lazy<ISimpleContract[]> array { get; set; }

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(array);
            Assert.IsTrue(array.Value.Count() == 2);

        }
    }


    [TestFixture]
    public class LazyManyEnumerableComponentTest : TestBase
    {

        [InjectMany]
        Lazy<IEnumerable<ISimpleContract>> enumerable;
       
        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(enumerable);
            Assert.IsTrue(enumerable.Value.Count() == 2);

        }
    }

    [TestFixture]
    public class LazyManyEmptyEnumerableComponentTest : TestBase
    {

        [InjectMany]
        Lazy<IEnumerable<ISimpleContract>> enumerable;

        [Test]
        public void Test()
        {
            ServiceRegistry.Compose(this);
            Assert.IsNull(enumerable);
        }
    }

    [TestFixture]
    public class LazyManyListComponentTest : TestBase
    {

        [InjectMany]
        Lazy<List<ISimpleContract>> list;
       
        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Value.Count() == 2);

        }
    }

    [TestFixture]
    public class LazyManyIListComponentTest : TestBase
    {

        [InjectMany]
        Lazy<IList<ISimpleContract>> list;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Value.Count() == 2);

        }
    }

    [TestFixture]
    public class LazyManyICollectionComponentTest : TestBase
    {

        [InjectMany]
        Lazy<ICollection<ISimpleContract>> list;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Value.Count() == 2);

        }
    }
}
