using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class TypeConversion
    {
        public class A1
        {
            public int fld1;
            public string fld2;
        }

        public class B1
        {
            public decimal fld1 = 15;
            public decimal fld2 = 11;
        }

        public class A2
        {
            public string[] fld3;
        }

        public class B2
        {
            public int fld3 = 99;
        }

        public class A3
        {
            public A1 a1 = new A1();
            public A2 a2 = new A2();
        }

        public class B3
        {
            public A1 a1 = new A1();
            public A2 a2 = new A2();
        }

        public class A4
        {
            private string m_str;
            public string str
            {
                set
                {
                    m_str = value;
                }
                get
                {
                    return m_str;
                }
            }
        }

        public class B4
        {
            public class BInt
            {
                public override string ToString()
                {
                    return "string";
                }
            }


            BInt m_bint = new BInt();
            public BInt str
            {
                get
                {
                    return m_bint;
                }
            }
        }

        public class A5
        {
            public string fld2;
        }

        public class B5
        {
            public decimal fld2 = 11;
        }

        [Test]
        public void Test1()
        {
            B1 b = new B1();
           
            var a = Mapper.Map<B1,A1>(b);
            Assert.AreEqual(a.fld1, 15);
            Assert.AreEqual(a.fld2, "11");
        }

        [Test]
        public void Test2()
        {
            
            B2 b = new B2();
            var a = Mapper.Map<B2, A2>(b);
            Assert.AreEqual("99", a.fld3[0]);
        }

        [Test]
        public void Test3_ShallowCopy()
        {
            //A3 a = new A3();
            //B3 b = new B3();
            //b.a1.fld1 = 15;
            //Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().DeepMap()).Map(b, a);
            //Assert.AreEqual(a.a1.fld1, 15);
            //b.a1.fld1 = 666;
            //Assert.AreEqual(a.a1.fld1, 15);

            //Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap()).Map(b, a);
            //b.a1.fld1 = 777;
            //Assert.AreEqual(777, a.a1.fld1);

            //b = new B3();
            //Context.objMan.GetMapper<B3, A3>(new DefaultMapConfig().ShallowMap<A1>().DeepMap<A2>()).Map(b, a);
            //b.a1.fld1 = 333;
            //b.a2.fld3 = new string[1];

            //Assert.AreEqual(333, a.a1.fld1);
            //Assert.IsNull(a.a2.fld3);
        }

        [Test]
        public void Test4()
        {
            B4 b = new B4();
            var a = Mapper.Map<B4, A4>(b);
            Assert.AreEqual(a.str, "string");
        }

        public class SimpleClassFrom
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Email { get; set; }
        }

        public class SimpleClassTo
        {
            public long ID { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Email { get; set; }
        }

        List<SimpleClassFrom> fromCollection = new List<SimpleClassFrom>();
        int count = 10000;
        //test nlite mapper
        NLite.Mapping.IMapper<SimpleClassFrom, SimpleClassTo> a = NLite.Mapper.CreateMapper<SimpleClassFrom, SimpleClassTo>();
        //test emit mapper
        //EmitMapper.ObjectsMapper<SimpleClassFrom, SimpleClassTo> mapper = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<SimpleClassFrom, SimpleClassTo>();

        public TypeConversion()
        {
            //consturct data

            for (int i = 0; i < count; i++)
            {
                SimpleClassFrom from = new SimpleClassFrom
                {
                    ID = 123456,
                    Name = "test",
                    Age = 30,
                    Email = "hhhhhhhhhhhhhhh@hotmail.com"
                };
                fromCollection.Add(from);
            }

            a.Map(fromCollection[0]);
            //mapper.Map(fromCollection[0]);

        }

        [Test]
        public void CompareTest()
        {
            for(int i = 0;i< 5;i++)
                InternalCompare();
        }

        private void InternalCompare()
        {
            var sw = Stopwatch.StartNew();
            //for (int i = 0; i < count; i++)
            //{
            //    mapper.Map(fromCollection[i]);
            //}
            //sw.Stop();

            //Console.WriteLine("emitmapper elapsed:{0}ms", sw.ElapsedMilliseconds);


            sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                a.Map(fromCollection[i]);
            }


            sw.Stop();

            Console.WriteLine("nlitemapper elapsed:{0}ms", sw.ElapsedMilliseconds);

            Console.Read();
        }
    }
}
