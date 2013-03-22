using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLite.Reflection;
using NUnit.Framework;
using NLite.Collections;
using NLite.Mini;
using System.Text;

namespace NLite.Test.Reflection
{
    [TestFixture]
    public class ClassLoaderTest
    {
        protected virtual IClassLoader CreateClassLoader()
        {
            var loader = new SimpleClassLoader();
            //Assert.IsTrue(loader.IsDefault);
            return loader;
        }

         [Test]
        public virtual void DisplayNLiteInterfaceCounts()
        {
            var types = typeof(ServiceLocator).Assembly.GetTypes();
            var interC = (from t in types
                          where t.IsInterface
                          select t).Count();
            var classC = (from t in types
                          where t.IsClass || t.IsValueType
                          select t).Count();

            Console.WriteLine("Interfaces:" + interC.ToString());
            Console.WriteLine("classes:" + classC.ToString());

            Console.WriteLine(typeof(List<>).Assembly.FullName);
            Console.WriteLine(typeof(List<int>).FullName);
            Console.WriteLine(typeof(Dictionary<int,string>).FullName);
            Console.WriteLine(typeof(IGenericType<,,,>).FullName);
            Console.WriteLine(Assembly.GetExecutingAssembly().FullName);


            string str = "NLite, Version=1.0.3860.19500, Culture=neutral, PublicKeyToken=null";
            var asmName = new AssemblyName(str);
            var version = asmName.Version;

            str = "NLite";
            asmName = new AssemblyName(str);
            version = asmName.Version;

        }

      

        interface IGenericType<T1, T2, T3, T4>
        {

        }

        [Test]
        public virtual void GenericNameTest()
        {
            var expectedListType = "System.Collections.Generic.IList`1";
            Assert.AreEqual(expectedListType, typeof(IList<>).FullName);

            expectedListType = "System.Collections.Generic.Dictionary`2";
            Assert.AreEqual(expectedListType, typeof(Dictionary<,>).FullName);

            expectedListType = "System.Collections.Generic.IList`1[[System.Int32]]";

            var classLoader = CreateClassLoader();
            var type = classLoader.Load("System.Collections.Generic.List`1[[System.Int32,mscorlib]],mscorlib");
            Assert.IsNotNull(type);


            type = classLoader.Load("NLite.Test.Reflection.ClassLoaderTest+IGenericType`4,NLite.Test");
            Assert.IsNotNull(type);


            var sb = new StringBuilder("123");
            sb.Length--;
            Console.WriteLine(sb);


            type = classLoader.Load("NLite.Test.Reflection.ClassLoaderTest+IGenericType`4[[System.Int32,mscorlib],[System.String,mscorlib],[System.Object,mscorlib],[System.Boolean,mscorlib]],NLite.Test");
            Assert.IsNotNull(type);


            type = classLoader.Load("NLite.Test.Reflection.ClassLoaderTest+IGenericType`4,NLite.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            Assert.IsNotNull(type);

            type = classLoader.Load("NLite.Test.Reflection.ClassLoaderTest+IGenericType`4[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]],NLite.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            Assert.IsNotNull(type);
        }

        //[Test]
        //public void Test2()
        //{
        //    var classLoader = CreateClassLoader();
        //    classLoader.AssemblyLoader.Resolvers.Add(new FileDirectoryAssemblyResolver(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin\Debug", false));


        //    var binderType = classLoader.Load("NLite.Data.Oracle8i.DataClient.Test.SchemaTest,NLite.Data.Query.Test");
        //    Assert.IsNotNull(binderType);
        //}

        //[Test]
        //public void Test3()
        //{
        //    ClassLoader.Current.AssemblyLoader.Resolvers.Add(new FileDirectoryAssemblyResolver(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin", true));

        //    var binderType = ClassLoader.Load("NLite.Data.Oracle8i.DataClient.Test.SchemaTest,NLite.Data.Query.Test");
            
        //    Assert.IsNotNull(binderType);
        //}

        //[Test]
        //public void Test5()
        //{
        //    var classLoader = CreateClassLoader();
        //    classLoader.AssemblyLoader.Resolvers.Add(new FileDirectoryAssemblyResolver(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin", true));

        //    var binderType = classLoader.Load("NLite.Data.Test.IGenericType`4[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]],NLite.Data.Query.Test");
        //    Assert.IsNotNull(binderType);
        //}

        //[Test]
        //public void Test6()
        //{
        //    var classLoader = CreateClassLoader();
        //    classLoader.AssemblyLoader.Resolvers.Add(new FileDirectoryAssemblyResolver(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin", true));
        //    var asm = classLoader.AssemblyLoader.Load("NLite.Data.Query.Test");

        //    var binderType = asm.GetType("NLite.Data.Test.IGenericType`4[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]");
        //    Assert.IsNotNull(binderType);
        //}

        //[Test]
        //public void Test7()
        //{
        //    var classLoader = CreateClassLoader();
        //    ClassLoader.Current.AssemblyLoader.Resolvers.Add(new FileDirectoryAssemblyResolver(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin", true));

        //    var binderType = ClassLoader.Load("NLite.Data.Query.Test", "NLite.Data.Test.IGenericType`4[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]");
        //    Assert.IsNotNull(binderType);
        //}

        //[Test]
        //public void Test4()
        //{
        //    var classLoader = ClassLoader.Current;
        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));
        //    classLoader.AssemblyLoader.Resolvers.Add(new FileDirectoryAssemblyResolver(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin\Debug", true));


        //    var binderType = classLoader.Load("NLite.Data.Oracle8i.DataClient.Test.SchemaTest,NLite.Data.Query.Test,Version=1.0.4044.26629,Culture=neutral,PublicKeyToken=null");
        //    Console.WriteLine("-----111");
        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));

        //    var o = Activator.CreateInstance(binderType);
        //    Console.WriteLine("-----111");
        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));
        //    Assert.IsNotNull(binderType);

        //    Console.WriteLine("-----111");
        //    o.Proc("SetUp");
        //    o.Proc("Collections");
           
        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));

        //    Console.WriteLine("-----111");
        //    var asm = Assembly.LoadFrom(@"E:\NLite\trunk\Source\Infrastructure\NLite.Data.Test\bin\debug\NLite.Data.Query.Test.dll");
           
        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));

        //    binderType = asm.GetType("NLite.Data.Oracle8i.DataClient.Test.SchemaTest");
        //    o = Activator.CreateInstance(binderType);
        //    Console.WriteLine("-----111");
        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));
        //    Assert.IsNotNull(binderType);

        //    Console.WriteLine("-----111");
        //    o.Proc("SetUp");
        //    o.Proc("Collections");

        //    AppDomain.CurrentDomain.GetAssemblies().ForEach(a => Console.WriteLine(a.FullName));
        //    Assert.IsNotNull(binderType);
        //}

        //[Test]
        //public virtual void NewInstanceTest()
        //{
        //    var classLoader = CreateClassLoader();
           
        //    var o = classLoader.NewInstance<TestClass>();
        //    Assert.IsNotNull(o);
        //    Assert.IsTrue(o.HasVisited);

        //    var o2 = classLoader.NewInstance<TestClass>(100);
        //    Assert.IsNotNull(o2);
        //    Assert.IsTrue(o2.Arg1 == 100);

        //    var o3 = classLoader.NewInstance("NLite.Test.Reflection.ClassLoaderTest+TestClass,NLite.Test", 50) as TestClass;
        //    Assert.IsNotNull(o3);
        //    Assert.IsTrue(o3.Arg1 == 50);

        //    var o4 = classLoader.NewInstance<TestClass>("NLite.Test.Reflection.ClassLoaderTest+TestClass,NLite.Test", 50) ;
        //    Assert.IsNotNull(o4);
        //    Assert.IsTrue(o4.Arg1 == 50);

        //    var o5 = classLoader.NewInstance<IList<int>>(typeof(List<int>));
        //    Assert.IsNotNull(o5);
        //}

        //[Test]
        //public virtual void InvokeTest()
        //{
        //    Func<ConstructorInfo> newHandler = () => typeof(TestClass).GetConstructor(Type.EmptyTypes);

        //    var classLoader = CreateClassLoader();
        //    var o = (classLoader.Invoke(newHandler)as ConstructorInfo).FastInvoke(null) as TestClass;
        //    Assert.IsNotNull(o);
        //    Assert.IsTrue(o.HasVisited);

        //}

        //[Test]
        //public void LoadAssemblyTest()
        //{
        //    var classLoader = CreateClassLoader();
        //    var asm1 = classLoader.LoadAssembly("Castle.core.dll");
        //    Assert.IsNotNull(asm1);

        //    var asm2 = classLoader.LoadAssembly("Castle.core.dll");
        //    Assert.IsNotNull(asm2);

        //    Assert.AreSame(asm1, asm2);
        //    classLoader.Dispose();
        //}

        //[Test]
        //public virtual void LoadAssembliesTest()
        //{
        //    var classLoader = CreateClassLoader();
        //    var beforeAssemblyCount = classLoader.AssemblyLoader.GetAssemblies().Length;
        //   classLoader.AssemblyLoader.LoadFromDirectory(AppDomain.CurrentDomain.BaseDirectory);
        //   var afterAssemblyCount = classLoader.AssemblyLoader.GetAssemblies().Length;

        //   Assert.IsTrue(afterAssemblyCount >= beforeAssemblyCount);
           
           
        //}

        [Test]
        public virtual void LoadAssembliesTest2()
        {
            var classLoader = CreateClassLoader();
            var beforeAssemblyCount = classLoader.AssemblyLoader.GetAssemblies().Length;
            //classLoader.LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory,true);
            //var afterAssemblyCount = classLoader.GetAssemblies().Length;

            //Assert.IsTrue(afterAssemblyCount > beforeAssemblyCount);
            //classLoader.Dispose();
        }

      

        [Serializable]
        protected class TestClass
        {
            public bool HasVisited;
            public TestClass()
            {
                HasVisited = true;
            }

            public int Arg1;

            public TestClass(int a1)
            {
                Arg1 = a1;
            }
        }

        [Serializable]
        protected class Test2Class:MarshalByRefObject
        {
            public bool HasVisited;
            public IKernel Kernel { get; private set; }
            public Test2Class()
            {
                HasVisited = true;
                Kernel = new Kernel();
            }

            public int Arg1;

            public Test2Class(int a1)
            {
                Arg1 = a1;
            }
        }
    }

    //[TestFixture]
    //public class RemoteClassLoaderTest : ClassLoaderTest
    //{
    //    protected override IClassLoader CreateClassLoader()
    //    {
    //        var loader = new RemoteClassLoader();
    //        Assert.AreNotEqual(loader, AppDomain.CurrentDomain);
    //        Assert.IsFalse(loader.IsDefault);
    //        return loader;
    //    }

    //    [Test]
    //    public override void InvokeTest()
    //    {
    //        Func<ConstructorInfo> newHandler = () => typeof(TestClass).GetConstructor(Type.EmptyTypes);
    //        Func<ConstructorInfo> new2Handler = () => typeof(Test2Class).GetConstructor(Type.EmptyTypes);

    //        var classLoader = base.CreateClassLoader();
    //        var o = (classLoader.Invoke(newHandler) as ConstructorInfo).FastInvoke(null) as TestClass;
    //        Assert.IsNotNull(o);
    //        Assert.IsTrue(o.HasVisited);

    //        var remoteClassLoader = CreateClassLoader();
    //        var remoteObject = (remoteClassLoader.Invoke(new2Handler) as ConstructorInfo).FastInvoke(null) as Test2Class;
    //        Assert.IsNotNull(remoteObject);
    //        Assert.IsTrue(remoteObject.HasVisited);

    //        remoteObject.Arg1 = 10;
    //        o.Arg1 = remoteObject.Arg1;

    //        o.Arg1 = 15;
    //        remoteObject.Arg1 = o.Arg1;

            
    //    }
    //    //public override void LocatorTest()
    //    //{

    //    //}

    //    //public override void LoadAssembliesTest2()
    //    //{

    //    //}

    //}


}
