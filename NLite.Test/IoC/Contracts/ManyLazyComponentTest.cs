using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Test.IoC.Contracts.Components;

namespace NLite.Test.IoC.Contracts
{
    [TestFixture]
    public class ManyLazyArrayComponentTest : TestBase
    {
        [InjectMany]
        Lazy<ISimpleContract>[] array { get; set; }

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
    public class ManyLazyEnumerableComponentTest : TestBase
    {

        [InjectMany]
        IEnumerable<Lazy<ISimpleContract>> enumerable;

        [Test]
        public void Test()
        {
            ServiceRegistry
               .Register<SimpleComponent>()
               .Register<SimpleComponentTwo>()
               .Compose(this);

            Assert.IsNotNull(enumerable);
            Assert.IsTrue(enumerable.Count() == 2);

        }
    }

    [TestFixture]
    public class ManyLazyListComponentTest : TestBase
    {

        [InjectMany]
        List<Lazy<ISimpleContract>> list;

        [Test]
        public void Test()
        {
            ServiceRegistry
               .Register<SimpleComponent>()
               .Register<SimpleComponentTwo>()
               .Compose(this);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count() == 2);

        }
    }

    [TestFixture]
    public class ManyLazyIListComponentTest : TestBase
    {

        [InjectMany]
        IList<Lazy<ISimpleContract>> list;

        [Test]
        public void Test()
        {
            ServiceRegistry
               .Register<SimpleComponent>()
               .Register<SimpleComponentTwo>()
               .Compose(this);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count() == 2);

        }
    }

    [TestFixture]
    public class ManyLazyICollectionComponentTest : TestBase
    {

        [InjectMany]
        ICollection<Lazy<ISimpleContract>> list;

        [Test]
        public void Test()
        {
            ServiceRegistry
               .Register<SimpleComponent>()
               .Register<SimpleComponentTwo>()
               .Compose(this);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count() == 2);

        }
    }
}
