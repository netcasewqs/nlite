using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    public class ServiceProxySpec : TestBase
    {
        class CalculateService
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
            public int Sub(int a, int b)
            {
                return a - b;
            }
            public int Multiply(int a, int b)
            {
                return a * b;
            }
            public int Divide(int a, int b)
            {
                return a / b;
            }
        }

        interface ICalculate
        {
            int Add(int a, int b);
            int Sub(int a, int b);
            int Multiply(int a, int b);
            int Divide(int a, int b);
        }

        [Test]
        public void Test()
        {
            ServiceRegistry.Register<CalculateService>();

            var calculate = ServiceProxyFactory.Create<ICalculate>();
            Assert.IsNotNull(calculate);
            Assert.AreEqual(5, calculate.Add(2, 3));
            Assert.AreEqual(9, calculate.Sub(12, 3));
        }
    }

    public class RestSpec : TestBase
    {

    }
}
