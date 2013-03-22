using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Threading;

namespace NLite.Test.Threading
{
    [TestFixture]
    public class AtomicIntegerTest
    {
        [Test]
        public void Test()
        {
            AtomicInteger m = new AtomicInteger(0);
            Assert.IsTrue(m == 0);

            Assert.IsFalse(m.CompareAndSet(3, 2));
            Assert.IsTrue(m == 0);

            Assert.IsTrue(m.CompareAndSet(0, 2));
            Assert.IsTrue(m == 2);

            var rs = m + 3;// 2+3
            Assert.AreEqual(5, rs);

            Assert.AreEqual(4, m--);

            Assert.AreEqual(2, m - 2);

            Assert.AreEqual(6, m + 4);

            Assert.AreEqual(6, m.GetAndAdd(4));
            Assert.AreEqual(10, m);
           
        }
    }
}
