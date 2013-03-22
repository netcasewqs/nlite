using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class CustomMapping
    {
        public class A1
        {
            private string _fld2 = "";

            public string fld1 = "";
            public string fld2
            {
                get
                {
                    return _fld2;
                }
            }
            public void SetFld2(string value)
            {
                _fld2 = value;
            }
        }


        public class A2
        {
            public string fld1;
            public string fld2;
        }

        public class B2
        {
            public string fld2 = "B2::fld2";
            public string fld3 = "B2::fld3";
        }

        [Test]
        public void Test_CustomConverter()
        {
            var a = Mapper.Map<B2, A2>(new B2());
            Assert.AreEqual("B2::fld2", a.fld2);
        }

        public class AA
        {
            public string fld1;
            public string fld2;
        }

        public class BB
        {
            public string fld1 = "B2::fld1";
            private string _fld2 = "B2::fld2";

            public string Getfld2 () { return _fld2; } 
        }

        [Test]
        public void Test_CustomConverter2()
        {
            var b = new BB();
            var a = Mapper.Map<BB, AA>(b);
            Assert.AreEqual(b.fld1, a.fld1);
            Assert.AreEqual(b.Getfld2(), a.fld2);
        }
     
    }
}
