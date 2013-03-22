using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite;
using NLite.Test.IoC.Contracts.Components;

namespace NLite.Test.IoC.Contracts
{
    [TestFixture]
    public class ComponentTest:TestBase
    {
        [Inject]
        private ISimpleContract contract;


        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<SimpleComponentTwo>()
                .Compose(this);

            Assert.IsNotNull(contract);

            var components = ServiceLocator.GetAll<ISimpleContract>().ToArray();
            Assert.IsTrue(components.Length == 2);
            Assert.IsTrue(components[0] is SimpleComponent);
            Assert.IsTrue(components[1] is SimpleComponentTwo);

            var component = ServiceLocator.Get<SimpleComponent>();
            Assert.IsNotNull(component);

            Assert.AreSame(contract, component);
            Assert.AreSame(contract, components[0]);

            var nonContract = ServiceLocator.Get<INonContractInterface>();
            Assert.IsNotNull(nonContract);

            
        }
    }
}
