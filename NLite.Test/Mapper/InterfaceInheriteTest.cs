using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Test.Mapper
{
    /// <summary>
    /// Bug:EmitMapper与NLiteMapper在多接口继承下映射的问题:http://www.cnblogs.com/repository/archive/2011/04/14/2015926.html
    /// </summary>
    [TestFixture]
    public class InterfaceInheriteTest
    {
        interface IBase
        {
            string StrA { get; set; }
        }

        interface ISub : IBase
        {
            string StrB { get; set; }
        }

        class ClassAImpISub : ISub
        {
            public string StrA { get; set; }
            public string StrB { get; set; }
        }

        class ClassB
        {
            public string StrA { get; set; }
            public string StrB { get; set; }
        }

        [Test]
        public void Test()
        {
            ClassAImpISub from = new ClassAImpISub();
            from.StrA = "StrA";
            from.StrB = "StrB";

            var to = NLite.Mapper.Map<ISub, ClassB>(from);

            Assert.AreEqual(from.StrA, to.StrA);
            Assert.AreEqual(from.StrB, to.StrB);

        }
    }
}
