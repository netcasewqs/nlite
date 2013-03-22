using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using NLite.Reflection;

namespace NLite.Test.Reflection
{
    [Contract]
    public interface IRunable
    {
        void Run();
    }

    //测试器元数据
    public interface ITestInfo
    {
        //目录
        string Category { get; }
        //名称
        string Name { get; }
    }

    //映射器元数据注解
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttributeAttribute]
    public class TestInfoAttribute : ComponentAttribute
    {
        public string Category { get; set; }
        public string Name { get; set; }
    }


    [TestFixture]
    public class SpecBase
    {
        public SpecBase()
        {
        }

        [SetUp]
        public void SetUp()
        {
            Given();
            When();
        }

        public virtual void Given() { }
        public virtual void When()
        {
        }

        [Test]
        public void Test()
        {
            
        }
    }

    public abstract class PerformanceSpecBase : SpecBase
    {
        [InjectMany]
        protected Lazy<IRunable, ITestInfo>[] Mappers;

        protected abstract void RegisterComponents();

        public virtual int Times
        {
            get { return 100000; }
        }

        public override void Given()
        {
            NLiteEnvironment.Refresh();
            RegisterComponents();
            ServiceRegistry.Compose(this);
        }


        public override void When()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var item in Mappers)
                    CodeTimer.Time(item.Metadata.Category + "->" + item.Metadata.Name, 100000, () => item.Value.Run());
            }
        }
    }

    public class InvokeConstructorPerformanceSpec : PerformanceSpecBase
    {
        class TestEntity { }

        protected override void RegisterComponents()
        {
            ServiceRegistry
                .Register<DirectInvokeMode>()
                .Register<ReflectInvokeMode>()
                .Register<GenericReflectInvokeMode>()
                .Register <GenericCreateInvokeMode>()
                .Register<EmitInvokeMode>()
                .Register < NoCacheEmitInvokeMode>()
                .Register < GenericReflectInvokeMode2>()
                ;
        }
        [TestInfo(Category = "Class.Constructor", Name = "Direct")]
        class DirectInvokeMode:IRunable
        {
            public void Run()
            {
                new TestEntity();
            }
        }

        [TestInfo(Category = "Class.Constructor", Name = "Reflect")]
        class ReflectInvokeMode : IRunable
        {
            public void Run()
            {
                Activator.CreateInstance(typeof(TestEntity));
            }
        }

        [TestInfo(Category = "Class.Constructor", Name = "GenericReflect")]
        class GenericReflectInvokeMode : IRunable
        {
            public void Run()
            {
                Activator.CreateInstance<TestEntity>();
            }
        }

        [TestInfo(Category = "Class.Constructor", Name = "Reflect->Reflect")]
        class GenericReflectInvokeMode2 : IRunable
        {
            static readonly MethodInfo CreateMethod = typeof(Activator)
                .GetMethod("CreateInstance", Type.EmptyTypes)
                .MakeGenericMethod(typeof(TestEntity));

            public void Run()
            {
                CreateMethod.Invoke(null,null);
            }


        }

        

        [TestInfo(Category = "Class.Constructor", Name = "Generic Create")]
        class GenericCreateInvokeMode : IRunable
        {
            public void Run()
            {
                Create<TestEntity>();
            }

            static T Create<T>() where T : new()
            {
                return new T();
            }
        }

        [TestInfo(Category = "Class.Constructor", Name = "Emit")]
        class EmitInvokeMode : IRunable
        {
            static readonly ConstructorHandler Ctor = typeof(TestEntity).GetConstructor(Type.EmptyTypes).GetCreator();

            public void Run()
            {
                Ctor();
            }
        }


        [TestInfo(Category = "Class.Constructor", Name = "NoCacheEmit")]
        class NoCacheEmitInvokeMode : IRunable
        {
            public void Run()
            {
                typeof(TestEntity).GetConstructor(Type.EmptyTypes).GetCreator()();
            }
        }


    }

    //Emit并不是第二的，泛型New在创建结构体上超过了Emit速度
    public class InvokeStructConstructorPerformanceSpec : PerformanceSpecBase
    {
        struct TestEntity { }

        protected override void RegisterComponents()
        {
            ServiceRegistry
                .Register<DirectInvokeMode>()
                .Register<ReflectInvokeMode>()
                .Register<GenericReflectInvokeMode>()
                .Register<GenericCreateInvokeMode>()
                .Register<EmitInvokeMode>()
                //.Register<GenericReflectInvokeMode2>()
                //.Register < DynamicGenericCreateInvokeMode>()
                ;
        }
        [TestInfo(Category = "Struct.Constructor", Name = "Direct")]
        class DirectInvokeMode : IRunable
        {
            public void Run()
            {
                new TestEntity();
            }
        }

        [TestInfo(Category = "Struct.Constructor", Name = "Reflect")]
        class ReflectInvokeMode : IRunable
        {
            public void Run()
            {
                Activator.CreateInstance(typeof(TestEntity));
            }
        }

        [TestInfo(Category = "Struct.Constructor", Name = "GenericReflect")]
        class GenericReflectInvokeMode : IRunable
        {
            public void Run()
            {
                Activator.CreateInstance<TestEntity>();
            }
        }

        [TestInfo(Category = "Struct.Constructor", Name = "Reflect->Reflect")]
        class GenericReflectInvokeMode2 : IRunable
        {
            static readonly MethodInfo CreateMethod = typeof(Activator)
                .GetMethod("CreateInstance", Type.EmptyTypes)
                .MakeGenericMethod(typeof(TestEntity));

            public void Run()
            {
                CreateMethod.Invoke(null, null);
            }
        }

        [TestInfo(Category = "Struct.Constructor", Name = "Generic Create")]
        class GenericCreateInvokeMode : IRunable
        {
            public void Run()
            {
                Create<TestEntity>();
            }

            static T Create<T>() where T : new()
            {
                return new T();
            }
        }

        [TestInfo(Category = "Struct.Constructor", Name = "Dynamic Generic Create")]
        class DynamicGenericCreateInvokeMode : IRunable
        {
            static readonly Func CreateMethod = typeof(DynamicGenericCreateInvokeMode)
                .GetMethod("Create")
                .MakeGenericMethod(typeof(TestEntity))
                .GetFunc();
            public void Run()
            {
                CreateMethod(null);
            }

            public static T Create<T>() where T : new()
            {
                return new T();
            }
        }
        
        [TestInfo(Category = "Struct.Constructor", Name = "Emit")]
        class EmitInvokeMode : IRunable
        {
            //结构体没有缺省构造函数
            static readonly DefaultConstructorHandler Ctor = typeof(TestEntity).GetDefaultCreator();

            public void Run()
            {
                Ctor();
            }

            public static TestEntity Create()
            {
                TestEntity t;
                return t;
            }
        }
    }

    public class InvokeActionMethodPerformanceSpec : PerformanceSpecBase
    {
        class TestEntity
        {
            public void Action()
            {
            }
        }

        protected override void RegisterComponents()
        {
            ServiceRegistry
                .Register<DirectInvokeMode>()
                .Register<ReflectInvokeMode>()
                .Register<EmitInvokeMode>()
                ;
        }
        [TestInfo(Category = "Class.Action", Name = "Direct")]
        class DirectInvokeMode : IRunable
        {
            TestEntity Instance = new TestEntity();

            public void Run()
            {
                Instance.Action();
            }
        }

        [TestInfo(Category = "Class.Action", Name = "Reflect")]
        class ReflectInvokeMode : IRunable
        {
            TestEntity Instance = new TestEntity();
            static MethodInfo ActionMethod = typeof(TestEntity).GetMethod("Action");
            public void Run()
            {
                ActionMethod.Invoke(Instance,null);
            }
        }

       
        [TestInfo(Category = "Class.Action", Name = "Emit")]
        class EmitInvokeMode : IRunable
        {
            TestEntity Instance = new TestEntity();
            static Proc ActionMethod = typeof(TestEntity).GetMethod("Action").GetProc();

            public void Run()
            {
                ActionMethod(Instance);
            }
        }
    }

}
