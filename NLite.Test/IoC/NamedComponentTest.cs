using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite;
using NUnit.Framework;
using NLite.Test.IoC.Contracts.Components;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class NamedComponentTest:TestBase
    {
        class NamedComponent:ISimpleContract
        {
        }

        class NamedComponentTwo : ISimpleContract
        {
        }


        [Inject(Id = "Named_A")]
        ISimpleContract _NamedComponent;

        [Inject(Id = "Named_B")]
        ISimpleContract _NamedComponent_Two { get; set; }

        [Inject]
        ISimpleContract _Contract;


        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SimpleComponent>()
                .Register<NamedComponent>("Named_A")
                .Register<NamedComponentTwo>("Named_B")
                .Compose(this);

            Assert.IsNotNull(_NamedComponent);
            Assert.IsNotNull(_NamedComponent_Two);
            Assert.IsNotNull(_Contract);

            Assert.IsInstanceOf<NamedComponent>(_NamedComponent);
            Assert.IsInstanceOf<NamedComponentTwo>(_NamedComponent_Two);
            Assert.IsInstanceOf<SimpleComponent>(_Contract);


        }
    }
}
