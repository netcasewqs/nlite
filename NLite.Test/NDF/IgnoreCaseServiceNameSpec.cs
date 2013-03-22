using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    public class IgnoreCaseOServiceNameSpec : TestBase
    {
        class CalculteService
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
        }


        [Test]
        public void Test()
        {
            ServiceRegistry.Register<CalculteService>();

            int expected = 5;
            int actual = ServiceDispatcher.Dispatch<int>("calculte", "add", new { a = 2, b = 3 });

            Assert.AreEqual(expected, actual);
        }
    }
}
