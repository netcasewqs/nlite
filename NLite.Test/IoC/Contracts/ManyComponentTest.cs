using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Test.IoC.Contracts.Components;
using NUnit.Framework;

namespace NLite.Test.IoC.Contracts
{
    [TestFixture]
    public class ManyComponentTest:TestBase
    {
        [InjectMany]
        ISimpleContract[] array;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(array);
            Assert.IsTrue(array.Count() == 2);

        }
    }

    [TestFixture]
    public class IEnumerableManyComponentTest : TestBase
    {
        [InjectMany]
        IEnumerable<ISimpleContract> array;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(array);
            Assert.IsTrue(array.Count() == 2);

        }
    }

    [TestFixture]
    public class ICollectionManyComponentTest : TestBase
    {
        [InjectMany]
        ICollection<ISimpleContract> array;

        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(array);
            Assert.IsTrue(array.Count() == 2);

        }
    }

    [TestFixture]
    public class IListManyComponentTest : TestBase
    {
        [InjectMany]
        IList<ISimpleContract> array;

        [Test]
        public void Test()
        {
            ServiceRegistry
                 .Register<SimpleComponent>()
                 .Register<SimpleComponentTwo>()
                 .Compose(this);

            Assert.IsNotNull(array);
            Assert.IsTrue(array.Count() == 2);

        }
    }

    [TestFixture]
    public class ListManyComponentTest : TestBase
    {
        [InjectMany]
        List<ISimpleContract> array;

        [Test]
        public void Test()
        {
            ServiceRegistry
               .Register<SimpleComponent>()
               .Register<SimpleComponentTwo>()
               .Compose(this);

            Assert.IsNotNull(array);
            Assert.IsTrue(array.Count() == 2);
        }
    }
}
