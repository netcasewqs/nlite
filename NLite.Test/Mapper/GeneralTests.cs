using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class GeneralTests
    {
        [TearDown]
        public void TearDown()
        {
            Mapper.Reset();
        }

       

        public class A
        {
            public enum En
            {
                En1,
                En2,
                En3
            }
            public class AInt
            {
                internal int intern = 13;
                public string str = "AInt";

                public AInt()
                {
                    intern = 13;
                }
            }

            private string m_str1 = "A::str1";

            public string str1
            {
                get
                {
                    return m_str1;
                }
                set
                {
                    m_str1 = value;
                }
            }

            public string str2 = "A::str2";
            public AInt obj;
            public En en = En.En3;

            int[] m_arr;

            public int[] arr
            {
                set
                {
                    m_arr = value;
                }
                get
                {
                    return m_arr;
                }
            }

            public AInt[] objArr;

            public string str3 = "A::str3";

            public A()
            {
                Console.WriteLine("A::A()");
            }
        }

        public class B
        {
            public enum En
            {
                En1,
                En2,
                En3
            }
            public class BInt
            {
                public string str = "BInt";
                /*
				public string str
				{
					get
					{
						throw new Exception("reading BInt::str");
					}
					set { }
				}
				 */
            }

            public string str1 = "B::str1";
            public string str2
            {
                get
                {
                    return "B::str2";
                }

            }
            public BInt obj = new BInt();
            public En en = En.En2;

            public BInt[] objArr;

            public int[] arr
            {
                get
                {
                    return new int[] { 1, 5, 9 };
                }
            }

            public object str3 = null;


            public B()
            {
                Console.WriteLine("B::B()");

                objArr = new BInt[2];
                objArr[0] = new BInt();
                objArr[0].str = "b objArr 1";
                objArr[1] = new BInt();
                objArr[1].str = "b objArr 2";
            }
        }

        internal class A1
        {
            public string f1 = "A1::f1";
            public string f2 = "A1::f2";
        }

        internal class B1
        {
            public string f1 = "B1::f1";
            public string f2 = "B1::f2";
        }

        public class Simple1
        {
            public int I = 10;
            public A.En fld1 = A.En.En1;
        }

        public class Simple2
        {
            public int I = 20;
            public B.En fld1 = B.En.En2;
        }

        [Test]
        public void SimpleTest()
        {
           
            Simple1 s = Mapper.Map<Simple2, Simple1>(new Simple2());
            Assert.AreEqual(20, s.I);
            Assert.AreEqual(A.En.En2, s.fld1);
        }

        [Test]
        public void SimpleTestEnum()
        {
           
            A.En aen = Mapper.Map<B.En, A.En>(B.En.En3);
            Assert.AreEqual(A.En.En3, aen);
        }

        public struct Struct1
        {
            public int fld;
        }

        public struct Struct2
        {
            public int fld;
        }

        [Test]
        public void SimpleTestStruct()
        {
            
            Struct1 s = Mapper.Map<Struct2, Struct1>(new Struct2() { fld = 13 });
            Assert.AreEqual(13, s.fld);
        }

        public struct Class1
        {
            public int fld;
        }

        public struct Class2
        {
            public int fld;
        }

        [Test]
        public void SimpleTestClass()
        {
            Class1 s = Mapper.Map<Class2, Class1>(new Class2() { fld = 13 });
            Assert.AreEqual(13, s.fld);
        }


        [Test]
        public void GeneralTests_Test1()
        {
            B b = new B();
           
            var a = Mapper.Map<B,A>(b);
            Assert.AreEqual(a.en, A.En.En2);
            Assert.AreEqual(a.str1, b.str1);
            Assert.AreEqual(a.str2, b.str2);
            Assert.AreEqual(a.obj.str, b.obj.str);
            Assert.AreEqual(a.obj.intern, 13);
            Assert.AreEqual(a.arr.Length, b.arr.Length);
            Assert.AreEqual(a.arr[0], b.arr[0]);
            Assert.AreEqual(a.arr[1], b.arr[1]);
            Assert.AreEqual(a.arr[2], b.arr[2]);

            Assert.AreEqual(a.objArr.Length, b.objArr.Length);
            Assert.AreEqual(a.objArr[0].str, b.objArr[0].str);
            Assert.AreEqual(a.objArr[1].str, b.objArr[1].str);
            Assert.IsNull(a.str3);
        }

        [Test]
        public void GeneralTests_Test2()
        {
            B b = new B();
            b.obj = null;

            var a = Mapper.Map<B, A>(b);
            Assert.IsNull(a.obj);
        }

     

        public class Source
        {
            public string field1 = "Source::field1";
            public string field2 = "Source::field2";
            public string field3 = "Source::field3";
        }

        public class Destination
        {
            public string m_field1;
            public string m_field2;
            public string m_field3;
        }

        [Test]
        public void GeneralTests_Example2()
        {
            Source src = new Source();

            var dst = Mapper
                .CreateMapper<Source, Destination>()
                .MatchMembers((m1, m2) => "m_" + m1 == m2)
                .Map(src);

            
            Assert.AreEqual(src.field1, dst.m_field1);
            Assert.AreEqual(src.field2, dst.m_field2);
            Assert.AreEqual(src.field3, dst.m_field3);
        }

        public class A2
        {
            public string str;
        }

        public class B2
        {
            public string str = "str";
        }

        [Test]
        public void GeneralTests_ConvertUsing()
        {
            var a = Mapper
                .CreateMapper<B2, A2>()
                .ConvertUsing<string, string>(s => "converted " + s)
                .Map(new B2());
            Assert.AreEqual("converted str", a.str);
        }

        [Test]
        public void GeneralTests_Ignore()
        {
            var a = Mapper
                .CreateMapper<B, A>()
                .IgnoreSourceMember(x=>x.str1)
                .Map(new B());

            Assert.AreEqual("A::str1", a.str1);
            Assert.AreEqual(a.en, A.En.En2);
        }

        public class A3
        {
            public class Int1
            {
                public string str1;
                public string str2;
                public int i;
            }

            public class Int2
            {
                public Int1 i1;
                public Int1 i2;
                public Int1 i3;
            }

            public Int2 i1;
            public Int2 i2;
            public Int2 i3;
        }

        public class B3
        {
            public class Int1
            {
                public string str1 = "1";
                public string str2 = null;
                public long i = 10;
            }

            public class Int2
            {
                public Int1 i1 = new Int1();
                public Int1 i2 = new Int1();
                public Int1 i3 = null;
            }

            public Int2 i1 = null;
            public Int2 i2 = new Int2();
            public Int2 i3 = new Int2();

        }

        [Test]
        public void GeneralTests_Exception()
        {
          
            var a = Mapper.Map<B3, A3>(new B3());
            Assert.IsNotNull(a);
            Assert.IsNull(a.i1);
            Assert.IsNotNull(a.i2);
            Assert.IsNotNull(a.i3);
            Assert.IsNotNull(a.i2.i1);
            Assert.IsNotNull(a.i2.i2);
            Assert.IsNull(a.i2.i3);
            Assert.IsNotNull(a.i3.i1);
            Assert.IsNotNull(a.i3.i2);
            Assert.IsNull(a.i3.i3);

            Assert.AreEqual("1", a.i2.i1.str1);
            Assert.AreEqual("1", a.i2.i2.str1);
            Assert.AreEqual("1", a.i3.i1.str1);
            Assert.AreEqual("1", a.i3.i2.str1);
            Assert.IsNull(a.i2.i1.str2);
            Assert.IsNull(a.i2.i2.str2);
            Assert.IsNull(a.i3.i1.str2);
            Assert.IsNull(a.i3.i2.str2);
            Assert.AreEqual(10, a.i2.i1.i);
            Assert.AreEqual(10, a.i2.i2.i);
            Assert.AreEqual(10, a.i3.i1.i);
            Assert.AreEqual(10, a.i3.i2.i);
        }

   

        public class TreeNode
        {
            public string data;
            public TreeNode next;
            public TreeNode[] subNodes;
        }

        [Test]
        public void TestRecursiveClass()
        {
            var tree = new TreeNode
            {
                data = "node 1",
                next = new TreeNode
                {
                    data = "node 2",
                    next = new TreeNode
                    {
                        data = "node 3",
                        subNodes = new[]
						{
							new TreeNode
							{
								data = "sub sub data 1"
							},
							new TreeNode
							{
								data = "sub sub data 2"
							}
						}

                    }
                },
                subNodes = new[]
				{
					new TreeNode
					{
						data = "sub data 1"
					}
				}
            };
          
            var tree2 = Mapper.Map<TreeNode, TreeNode>(tree);
            Assert.AreEqual("node 1", tree2.data);
            Assert.AreEqual("node 2", tree2.next.data);
            Assert.AreEqual("node 3", tree2.next.next.data);
            Assert.AreEqual("sub data 1", tree2.subNodes[0].data);
            Assert.AreEqual("sub sub data 1", tree2.next.next.subNodes[0].data);
            Assert.AreEqual("sub sub data 2", tree2.next.next.subNodes[1].data);
            Assert.IsNull(tree2.next.next.next);
        }

        public class BaseSource
        {
            public int i1;
            public int i2;
            public int i3;
        }

        public class DerivedSource : BaseSource
        {
            public int i4;
        }

        public class InherDestination
        {
            public int i1;
            public int i2;
            public int i3;
        }

        [Test]
        public void TestInheritence()
        {
            var dest = Mapper.Map<BaseSource, InherDestination>(
                new DerivedSource
                {
                    i1 = 1,
                    i2 = 2,
                    i3 = 3,
                    i4 = 4
                }
            );

            Assert.AreEqual(1, dest.i1);
            Assert.AreEqual(2, dest.i2);
            Assert.AreEqual(3, dest.i3);
        }

        [Test]
        public void SameTest()
        {
            var from = new BaseSource { i1 = 1, i2 = 2, i3 = 3 };
            var to = Mapper.Map<BaseSource, BaseSource>(from);
            Assert.AreEqual(from.i1, to.i1);
            Assert.AreEqual(from.i2, to.i2);
            Assert.AreEqual(from.i3, to.i3);
        }

        [Test]
        public void SameTest2()
        {
            var from = new BaseSource { i1 = 1, i2 = 2, i3 = 3 };
            var to = new BaseSource();
            Mapper.Map<BaseSource, BaseSource>(from,ref to);
            Assert.AreEqual(from.i1, to.i1);
            Assert.AreEqual(from.i2, to.i2);
            Assert.AreEqual(from.i3, to.i3);
        }
    }
}
