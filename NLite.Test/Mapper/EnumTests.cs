using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class EnumTests
    {
        public enum En1 : byte
        {
            a = 1,
            b = 2,
            c = 3
        }

        public enum En2 : long
        {
            a = 1,
            b = 2,
            c = 3
        }

        public enum En3 : int
        {
            b = 2,
            c = 3,
            a = 1
        }

        public class A
        {
            public En1 en1 { get; set; }
            public En2 en2;
            public En3 en3 { get; set; }
            public decimal en4;
            public string en5;
            public En1? en6;
            public En3 en7;
            public En3? en8;
            public En3? en9 = En3.c;
        }

        public class B
        {
            public decimal en1 = 3;
            public En1 en2 { get; set; }
            public string en3 = "c";
            public En2 en4 = En2.b;
            public En3 en5 = En3.a;
            public En2 en6 = En2.c;
            public En1? en7 = En1.c;
            public En1? en8 = En1.c;
            public En2? en9 = null;

            public B()
            {
                en2 = En1.c;
            }
        }
        [Test]
        public void EnumTests1()
        {
             Mapper.CreateMapper<B, A>();

            var a = Mapper.Map<B,A>(new B());
            Assert.IsTrue(a.en1 == En1.c);
            Assert.IsTrue(a.en2 == En2.c);
            Assert.IsTrue(a.en3 == En3.c);
            Assert.IsTrue(a.en4 == 2);
            Assert.IsTrue(a.en6 == En1.c);
            Assert.IsTrue(a.en7 == En3.c);
            Assert.IsTrue(a.en8 == En3.c);
            Assert.IsNull(a.en9);
        }
    }

  
}
