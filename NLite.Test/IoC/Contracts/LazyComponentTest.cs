using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Test.IoC.Contracts.Components;
using NUnit.Framework;

namespace NLite.Test.IoC.Contracts
{
    [TestFixture]
    public class LazyComponentTest:TestBase
    {
        [Inject]
        private Lazy<ISimpleContract> contract;

        [Inject]
        Lazy<SimpleComponent> component { get; set; }

        [InjectMany]
        IEnumerable<ISimpleContract> lazyComponents;

        [Inject]
        Lazy<ISimpleContract, IDictionary<string, object>> contract_metadata;


        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(contract);

            var components = lazyComponents.ToArray();

            Assert.IsTrue(components.Length == 2);
            Assert.IsTrue(components[0] is SimpleComponent);
            Assert.IsTrue(components[1] is SimpleComponentTwo);

            Assert.IsNotNull(component);

            Assert.AreSame(contract.Value, component.Value);
            Assert.AreSame(contract.Value, components[0]);

            var nonContract = ServiceLocator.Get<INonContractInterface>();
            Assert.IsNotNull(nonContract);

            Assert.IsNotNull(contract_metadata);
            Assert.IsNotNull(contract_metadata.Metadata != null);
            Assert.IsNotNull(contract_metadata.Value != null);

            Assert.AreEqual("SimpleComponent",contract_metadata.Metadata["TestClassName"]);
        }
    }
}
