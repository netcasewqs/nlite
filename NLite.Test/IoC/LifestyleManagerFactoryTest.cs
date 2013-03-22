//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using NLite.Mini.Lifestyle;

//namespace NLite.Test.IoC
//{
//    [TestFixture]
//    public class LifestyleManagerFactoryTest
//    {
//        [Test]
//        public void Test()
//        {
//            var factory = new LifestyleManagerFactory();

//            Assert.IsTrue(factory.HasRegister(LifestyleFlags.Singleton));
//            factory.UnRegister(LifestyleFlags.Singleton);
//            Assert.IsFalse(factory.HasRegister(LifestyleFlags.Singleton));

//            factory.UnRegister(LifestyleFlags.Thread);
//            factory.UnRegister(LifestyleFlags.Transient);

//            var ex = Assert.Throws<Exception>(() => factory.UnRegister(LifestyleFlags.Singleton));
           
           

//        }
//    }
//}
