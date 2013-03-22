using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Test.IoC.Contracts.Components;
using NLite;
using NLite.Mini.Activation;
using NLite.Mini;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class ExceptionTest:TestBase
    {
        class ExceptionComponent
        {
            [InjectMany]//InjectMany 支持批量注入，不支持单个注入，否则将会Throw InjectManyException
            public Lazy<ISimpleContract> array { get; set; }
        }

        abstract class AbstractComponent
        {
            public Lazy<ISimpleContract> array { get; set; }
        }

        class No_Public_Constructure_Component
        {
            No_Public_Constructure_Component() { }//被注入组件的构造函数必须是public 否则Throw ActivatorException
        }

        [Test]
        [ExpectedException(typeof(InjectManyException))]
        public void InjectManyExceptionTest()
        {
            ServiceRegistry.Register<ExceptionComponent>();

            //Assert.Throws<InjectManyException>(() => ServiceLocator.Get<ExceptionComponent>());
        }

        [Test]
        public void AbstractComponentTest()
        {
            ServiceRegistry.Register<AbstractComponent>();

            var ex = Assert.Throws<ActivatorException>(() => ServiceLocator.Get<AbstractComponent>());
            Console.WriteLine(ex.Message);
        }

        [Test]
        public void InterfaceComponentTest()
        {
            ServiceRegistry.Register<ISimpleContract>();

            var ex = Assert.Throws<ActivatorException>(() => ServiceLocator.Get<ISimpleContract>());
            Console.WriteLine(ex.Message);
        }

        [Test]
        public void No_Public_Constructure_Component_Test()
        {
            ServiceRegistry.Register<No_Public_Constructure_Component>();

            var ex = Assert.Throws<ActivatorException>(() => ServiceLocator.Get<No_Public_Constructure_Component>());
            Console.WriteLine(ex.Message);
        }

        

        [Test]
        public void LoopDependencyTest()
        {

            ServiceRegistry.Register<_A>()
                .Register<_B>();

            var ex = Assert.Throws<LoopDependencyException>(() =>
            ServiceLocator.Get<_A>());
            Console.WriteLine("Loop dependency exception message:");
            Console.WriteLine(ex.Message);
        }


        [Test]
        public void ValueType_Test()
        {
            ServiceRegistry.Register<int>();

            var i = ServiceLocator.Get<int>();
            Console.WriteLine(i);
        }
    }
}
