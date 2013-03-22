using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class FieldInjectionTest : TestBase
    {
        [Contract]
        interface ISampleContract
        {
            void Test();
        }

        [Contract]
        interface IFieldContract
        {
        }



        sealed class SampleComponent : ISampleContract
        {
            [Inject]
            IFieldContract field;

            public void Test()
            {
                Assert.IsNotNull(field);
            }
        }

        class FieldComponent : IFieldContract { }


        [Test]
        public void Test()
        {
            ServiceRegistry
                .Register<SampleComponent>()
                .Register<FieldComponent>();

            var component = ServiceLocator.Get<ISampleContract>();
            component.Test();
        }
    }
}
