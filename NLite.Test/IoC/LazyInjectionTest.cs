using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Mini.Resolving;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class LazyInjectionTest:TestBase
    {
        [Contract]
        interface ISimpleContract
        {
        }

        class SimpleComponent : ISimpleContract
        {
        }

        class HostComponent
        {
            [Inject]
            public Lazy<ISimpleContract> contract;
        }

        [Test]
        public void Test()
        {
            ReferenceManager.Instance.Enabled = true;
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<HostComponent>();
            

            var host = ServiceLocator.Get<HostComponent>();
            Assert.IsNotNull(host.contract != null);
            Assert.IsNotNull(host.contract.Value != null);
            ReferenceManager.Instance.Enabled = false;

        }
    }
}
