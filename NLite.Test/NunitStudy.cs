using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Test
{
    [TestFixture]
    public class NunitStudy
    {



        class Calculator
        {
           
            public int Add(int a, int b)
            {

                return a + b;
            }
        }

        [Test]
        public void Calculator_Add_Test()
        {
            var calculator = new Calculator();

            var expected = 8;

            var actual = calculator.Add(3, 5);

            Assert.AreEqual(expected, actual);
        }
    }
}
