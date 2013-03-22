using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec.DataBindings
{
    public class SingleParameterActionForBuildInTypeSpec:TestBase
    {
        class DemoService
        {
            public string Hello(string name)
            {
                return name;
            }
        }


        [Test]
        public void Test()
        {
            ServiceRegistry.Register<DemoService>();

            string expected = "hello abc";
            string actual = ServiceDispatcher.Dispatch<string>("Demo", "hello", new { name = expected });

            Assert.AreEqual(expected, actual);
        }

    }
}
