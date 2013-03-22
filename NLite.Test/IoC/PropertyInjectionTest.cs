using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class PropertyInjectionTest : TestBase
    {
        [Contract]
        interface ISampleContract
        {
            void Test();
        }

        [Contract]
        interface IPropertyContract
        {
        }



        sealed class SampleComponent : ISampleContract
        {
            [Inject]
            IPropertyContract property { get; set; }

            public void Test()
            {
                Assert.IsNotNull(property);
            }
        }

        class PropertyComponent : IPropertyContract { }


        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SampleComponent>()
                .Register<PropertyComponent>();

            var component = ServiceLocator.Get<ISampleContract>();
            component.Test();
        }
    }
}
