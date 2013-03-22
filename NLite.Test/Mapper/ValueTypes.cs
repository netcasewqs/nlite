using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class ValueTypes
    {
        #region Test types
        public struct A1
        {
            public int fld1;
        }

        public class B1
        {
            public int fld1 = 10;
        }

        public struct A2
        {
            public int fld1;
        }

        public class B2
        {
            public int fld1;
        }

        public class A3
        {
            public int fld1;
        }

        public struct B3
        {
            public int fld1;
        }

        public struct A4
        {
            public struct Int
            {
                public string fld1;
            }

            Int m_fld1;

            public Int fld1
            {
                get
                {
                    return m_fld1;
                }
                set
                {
                    m_fld1 = value;
                }
            }

            public Int fld2;
            public Int fld3 { get; set; }
        }

        public class B4
        {
            public struct Int
            {
                public decimal fld1;
            }


            Int m_fld1;

            public Int fld1
            {
                get
                {
                    return m_fld1;
                }
                set
                {
                    m_fld1 = value;
                }
            }


            public Int fld2 { get; set; }

            public Int fld3;

            public B4()
            {
                m_fld1 = new Int();
                m_fld1.fld1 = 12.444M;
                fld2 = new Int() { fld1 = 1111 };
                fld3.fld1 = 444;
            }
        }

        public class A5
        {
            public A1 a;
        }

        public class B5
        {
            public A1 a;
        }

        #endregion

        [Test]
        public void Test_ClassToStruct()
        {
            B1 b = new B1();
            var a = Mapper.Map<B1, A1>(b);
            Assert.AreEqual(a.fld1, 10);
        }

        [Test]
        public void Test_StructToStruct()
        {
            B2 b = new B2();
            b.fld1 = 99;
            var a = Mapper.Map<B2, A2>(b);
            Assert.AreEqual(a.fld1, 99);
        }

        [Test]
        public void Test_StructToClass()
        {
            B3 b = new B3();
            b.fld1 = 87;
            var a = Mapper.Map<B3, A3>(b);
            Assert.AreEqual(a.fld1, 87);
        }

        [Test]
        public void Test_StructProperties()
        {
            B4 b = new B4();
           
            var a = Mapper.Map<B4,A4>(b);
            Assert.AreEqual(b.fld1.fld1.ToString(), a.fld1.fld1);
            Assert.AreEqual(b.fld2.fld1.ToString(), a.fld2.fld1);
            Assert.AreEqual(b.fld3.fld1.ToString(), a.fld3.fld1);
        }

        [Test]
        public void Test_StructFields()
        {
          
            var b = new B5();
            b.a.fld1 = 10;
            var a = Mapper.Map<B5,A5>(b);
            Assert.AreEqual(10, a.a.fld1);
        }

        public class A6
        {
            public struct S1
            {
                public int i { get; set; }
            }
            public struct S2
            {
                public S1 s { get; set; }
            }

            public class C1
            {
                public S1 s { get; set; }
            }

            public class C2
            {
                public S1 s;
            }

            public class C3
            {
                public C2 c1;
                public C2 c2;
                public C2 c3;
            }

            public S2 s { get; set; }
            public S2 s2;
            public C1 s3 { get; set; }
            public C2 s4;
            public C3 s5;
        }

        public class B6
        {
            public struct S1
            {
                public int i { get; set; }
            }
            public struct S2
            {
                public S1 s;
            }

            public class C1
            {
                public S1 s;
            }

            public class C3
            {
                public C1 c1;
                public C1 c2;
                public C1 c3;
            }

            public S2 s = new S2();
            public S2 s2 { get; set; }
            public S2 s3;
            public C1 s4 { get; set; }
            public C3 s5;
        }

        [Test]
        public void Test_NestedStructs()
        {
           
            B6 b = new B6();

            var bs2 = new B6.S2();
            bs2.s.i = 15;
            b.s2 = bs2;
            b.s.s.i = 13;
            b.s3.s.i = 10;
            b.s4 = new B6.C1();
            b.s4.s.i = 11;
            b.s5 = new B6.C3();
            b.s5.c1 = new B6.C1();
            b.s5.c1.s.i = 1;
            b.s5.c2 = new B6.C1();
            b.s5.c2.s.i = 2;
            b.s5.c3 = new B6.C1();
            b.s5.c3.s.i = 3;

            var a = Mapper.Map<B6,A6>(b);
            Assert.AreEqual(13, a.s.s.i);
            Assert.AreEqual(15, a.s2.s.i);
            Assert.AreEqual(10, a.s3.s.i);
            Assert.AreEqual(11, a.s4.s.i);
            Assert.AreEqual(1, a.s5.c1.s.i);
            Assert.AreEqual(2, a.s5.c2.s.i);
            Assert.AreEqual(3, a.s5.c3.s.i);
        }

    }
}
