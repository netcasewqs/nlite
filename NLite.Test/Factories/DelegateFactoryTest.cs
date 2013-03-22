//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;

//namespace NLite.Test.Factories
//{
//    [TestFixture]
//    public class DelegateFactoryTest
//    {
//        [Test]
//        public void Test()
//        {

//            var factory = new NLite.Factories.DelegateFactory<int, List<int>>(i => new List<int>(i));

//            Assert.AreEqual(5, factory.Create(5).Capacity);

//            Func<string,List<string>> creator = null;
//            var factory2 = new NLite.Factories.DelegateFactory<string, List<string>>(creator);

//            Assert.AreEqual(null, factory2.Create("how are you"));

//            var f = factory as NLite.Factories.IFactory;
//            var list = (List<int>)f.Create(5);
//            Assert.IsNotNull(list);
//            Assert.AreEqual(5, list.Capacity);


//        }
//    }
//}
