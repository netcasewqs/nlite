using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using NLite.Reflection;
using NUnit.Framework;

namespace NLite.Test.Reflection
{
    [TestFixture]
    public class FieldTest
    {
        class TestClass
        {
            int A;
        }

        [Test]
        public void Test()
        {
            TestClass o = new TestClass();
            var field = o.GetType().GetField("A", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
            field.SetValue(o, 10);

            int value = (int)field.Get(o);

            Assert.IsTrue(value == 10);
        }


        struct MyStruct
        {
            public int Id;
            public string Name;
        }

        [Test]
        public void TestStruct()
        {
            NLite.Reflection.DynamicAssemblyManager.SaveAssembly();
            MyStruct a = default(MyStruct);
            InitStruct(ref a);
            Console.WriteLine(a.Id);
        }

        void InitStruct(ref MyStruct a)
        {
            a.Id = 5;
        }
    }

    [TestFixture]
    public class PropertyTest
    {
        class TestClass
        {
            public int A { get; set; }
        }

        [Test]
        public void Test()
        {
            TestClass o = new TestClass();
            var p = o.GetType().GetProperty("A");
            p.SetValue(o, 10,null);

            int value = (int)p.Get(o);

            Assert.IsTrue(value == 10);
        }
    }

    [TestFixture]
    public class ConsturctureTest
    {
        class TestClass
        {
            public bool HasVisited;
            public TestClass()
            {
                HasVisited = true;
            }
        }


        [Test]
        public void Test()
        {
            var c = typeof(TestClass).GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(c);
            var o = c.FastInvoke(new object[0]) as TestClass;
            Assert.IsNotNull(o);
            Assert.IsTrue(o.HasVisited);
        }
    }
}
