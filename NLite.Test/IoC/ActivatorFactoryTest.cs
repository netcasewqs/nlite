//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using NLite.Mini.Activation;

//namespace NLite.Test.IoC
//{
//    [TestFixture]
//    public class ActivatorFactoryTest
//    {
//        [Test]
//        public void Test()
//        {
//            var factory = new ActivatorRegistry();

//            Assert.IsTrue(factory.HasRegister(ActivatorType.Default));
//            factory.UnRegister(ActivatorType.Default);
//            Assert.IsFalse(factory.HasRegister(ActivatorType.Default));

//            factory.UnRegister(ActivatorType.Factory);
//            factory.UnRegister(ActivatorType.Instance);

//            var ex = Assert.Throws<Exception>(() => factory.UnRegister(ActivatorType.Proxy));
//            Console.WriteLine(ex.Message);
//            ex = Assert.Throws<ArgumentNullException>(() => factory.UnRegister(""));
//            Console.WriteLine(ex.Message);

//            ex = Assert.Throws<ArgumentNullException>(() => factory.Register("", () => new DefaultActivator()));
//            Console.WriteLine(ex.Message);
//            ex = Assert.Throws<ArgumentNullException>(() => factory.Register(ActivatorType.Default, null));
//            Console.WriteLine(ex.Message);
//            factory.Register(ActivatorType.Default, () => new DefaultActivator());
//            Assert.IsTrue(factory.HasRegister(ActivatorType.Default));
//            ex = Assert.Throws<RepeatRegistrationException>(() => factory.Register(ActivatorType.Default, () => new InstanceActivator()));
//            Console.WriteLine(ex.Message);
//            ex = Assert.Throws<RepeatRegistrationException>(() => factory.Register(ActivatorType.Default, () => new DefaultActivator()));
//            Console.WriteLine(ex.Message);


//            ex = Assert.Throws<ArgumentNullException>(() => factory.Create(""));
//            Console.WriteLine(ex.Message);

//            ex = Assert.Throws<InvalidOperationException>(() => factory.Create("fsfsf"));
//            Console.WriteLine(ex.Message);

//            var activator = factory.Create(ActivatorType.Default);
//            Assert.IsNotNull(activator);

//        }
//    }
//}
