using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLite;
using NLite.Mini.Lifestyle;
using NUnit.Framework;
using System;
using NLite.Mini;
using NLite.Mini.Resolving;
using NLite.Reflection;
namespace NLite.Test.IoC
{
    [TestFixture]
    public class KernelTest:TestBase
    {
        protected override void Init()
        {
            Person.HasVisited = LazyPerson.HasVisited = false;
        }

        [Test]
        public void RegisterTest()
        {
            ServiceRegistry.Register<Person>("person");

            Assert.IsTrue(ServiceRegistry.HasRegister("person"));
            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());
        }

        [Test]
        public void UnRegister_ById_Test()
        {
            ServiceRegistry.Register<Person>("person");

            Assert.IsTrue(ServiceRegistry.HasRegister("person"));
            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());

            ServiceRegistry.Current.UnRegister("person");
            Assert.IsFalse(ServiceRegistry.HasRegister("person"));
            Assert.IsFalse(ServiceRegistry.HasRegister<IPerson>());

        }

        [Test]
        public void UnRegister_ByType_Test()
        {
            ServiceRegistry.Register<Person>("person");
            ServiceRegistry.Register<Person2>();

            Assert.IsTrue(ServiceRegistry.HasRegister("person"));
            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());

            ServiceRegistry.Current.UnRegister(typeof(IPerson));
           
            Assert.IsFalse(ServiceRegistry.HasRegister<IPerson>());
            Assert.IsTrue(ServiceRegistry.HasRegister("person"));

        }

        [Test]
        public void RegisterByFactoryTest()
        {
            ServiceRegistry.Current.Register<IPerson>(() => new Person());

            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person2 != null);

            Assert.AreSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }



        [Test]
        public void GetByIdTest()
        {
            ServiceRegistry.Register<Person>("person");

            var person = ServiceLocator.Get<IPerson>("person");
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<Person>("person");
            Assert.IsTrue(person2 != null);

            Assert.AreSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }

        [Test]
        public void GetByTypeTest()
        {
            ServiceRegistry.Register<Person>();

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<IPerson,Person>();
            Assert.IsTrue(person2 != null);

            Assert.AreSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }

        [Test]
        public void GetAllTest()
        {
            ServiceRegistry
                .Register<Person>()
                .Register(typeof(Person2));

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);
            Assert.IsTrue(typeof(IPerson).IsAssignableFrom(person.GetType()));

            var items = ServiceLocator.GetAll<IPerson>().ToArray();
            Assert.IsTrue(items.Length == 2);
            Assert.IsTrue(items[0] is Person);
            Assert.IsTrue(items[1] is Person2);

        }

      
        [Test]
        public void RegisterInstanceTest()
        {
            var p = new Person();
            p.Name = "Test !";
            ServiceRegistry.Current.RegisterInstance(p);

            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsNotNull(person);
            Assert.AreSame(p, person);
        }

        [Test]
        public void StartableListnerTest()
        {
            ServiceRegistry.Current.Register("person", typeof(IPerson), typeof(StartablePerson));

            var person = ServiceLocator.Get(typeof(IPerson)) as StartablePerson;
            Assert.IsTrue(person != null);
            Assert.IsTrue(person.HasStarted);
            Assert.IsFalse(person.HasStopped);
        }

        [Test]
        public void ConstructorInjectTest()
        {
            
            ServiceRegistry
                .Register<Person2>()
                .Register<RedHorse>();

            var person = ServiceLocator.Get<Person2>();
            Assert.IsTrue(person != null);
            Assert.IsNotNull(person.Horse);
        }

        [Test]
        public void ConstructorInject2Test()
        {
            LifestyleType.Default = LifestyleFlags.Transient;
            ServiceRegistry.Current.Register("person", typeof(IPerson), typeof(Person5))
                .Register("horse", typeof(IHorse), typeof(RedHorse))
                .Register("A", typeof(A), typeof(A))
                .RegisterInstance("str", typeof(string), "ZhangSan");

            for (int i = 0; i < 1000; i++)
            {

                var person = ServiceLocator.Get("person") as Person5;
                Assert.IsTrue(person != null);
                Assert.IsNotNull(person.Horse);
                Assert.IsNotNull(person.A);
                Assert.IsTrue(person.HasVisited);
                Assert.IsTrue(string.IsNullOrEmpty(person.Name));
            }
        }

        [Test]
        public void ConstructorInject3Test()
        {
            ServiceRegistry.Current.Register("person", typeof(IPerson), typeof(Person6))
                .Register("horse", typeof(IHorse), typeof(RedHorse))
                .Register("A", typeof(A), typeof(A))
                .RegisterInstance("str", typeof(string), "ZhangSan");


            var person = ServiceLocator.Get("person") as Person6;
            Assert.IsTrue(person != null);
            Assert.IsNotNull(person.Horse);
            Assert.IsNotNull(person.A);
            Assert.IsTrue(person.HasVisited);
            Assert.AreEqual("ZhangSan", person.Name);
        }

        [Test]
        public void AutoConstructorTest()
        {
            ServiceRegistry.Current.Register("person", typeof(IPerson), typeof(Person2));
            // ServiceRegistry.Do("horse", typeof(IHorse), typeof(RedHorse), Lifestyle.Singleton, true);

            var person = ServiceLocator.Get("person") as Person2;
            Assert.IsTrue(person != null);
            Assert.IsNull(person.Horse);
        }

        [Test]
        public void NeedNamedParameterConstructorTest()
        {
            var ps = new Dictionary<string, object>();
            var person = new Person();

            ps["id"] = 10;
            ps["name"] = "ZhangSan";
            ps["person"] = person;

            ServiceRegistry.Register<ParameterConstructorClass>();
            var instance = ServiceLocator.Current.Get(typeof(IParameterConstructorInterface), ps) as IParameterConstructorInterface;

            Assert.IsNotNull(instance);
            Assert.AreEqual(10, instance.Id);
            Assert.IsTrue("ZhangSan" == instance.Name);
            Assert.AreSame(person, instance.Person);

        }

        [Test]
        public void NeedParameterConstructorTest()
        {
            var person = new Person();

            ServiceRegistry.Register<ParameterConstructorClass>();

            var instance = ServiceLocator.Current.Get(typeof(IParameterConstructorInterface), 10, "ZhangSan", person) as IParameterConstructorInterface;

            Assert.IsNotNull(instance);

            Assert.IsNotNull(instance);
            Assert.AreEqual(10, instance.Id);
            Assert.IsTrue("ZhangSan"== instance.Name);
            Assert.AreSame(person, instance.Person);
        }

        [Test]
        public void LoadingInSequence()
        {
            ServiceRegistry.Current
                .Register("C", typeof(C))
                .Register("B", typeof(B))
                .Register("A", typeof(A));

            Assert.IsNotNull(ServiceLocator.Get("A"));
            Assert.IsNotNull(ServiceLocator.Get("B"));
            Assert.IsNotNull(ServiceLocator.Get("C"));
        }



        [Test]
        public void LoadingPartiallyInSequence()
        {
            ServiceRegistry.Current
                .Register("A", typeof(A))
                .Register("B", typeof(B))
                .Register("C", typeof(C));

            Assert.IsNotNull(ServiceLocator.Get("A"));
            Assert.IsNotNull(ServiceLocator.Get("B"));
            Assert.IsNotNull(ServiceLocator.Get("C"));
        }

        [Test]
        public void LoadingOutOfSequenceWithExtraLoad()
        {
            ServiceRegistry.Current
                .Register("A", typeof(A))
                .Register("B", typeof(B))
                .Register("C", typeof(C))
                .RegisterInstance("I", typeof(int), 5);

            Assert.IsNotNull(ServiceLocator.Get("A"));
            Assert.IsNotNull(ServiceLocator.Get("B"));
            Assert.IsNotNull(ServiceLocator.Get("C"));
            Assert.AreEqual(5, ServiceLocator.Get<C>().I);
        }




        [Test]
        public void FieldInjectTest()
        {
            ServiceRegistry.Current
                .Register<FieldInjectionModel>(() => new FieldInjectionModel())
                .Register(typeof(IPerson), typeof(Person3))
                ;
            var instance = ServiceLocator.Get<FieldInjectionModel>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Person);
        }


        [Test]
        public void PropertyInjectTest()
        {
            ServiceRegistry.Current
                .Register(typeof(IPerson), typeof(Person3))
                .Register(typeof(IHorse), typeof(RedHorse));
            ReferenceManager.Instance.Enabled = true;
            var instance = ServiceLocator.Get<IPerson>() as Person3;

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Horse);
            Assert.IsTrue(instance.HasVisited);
            Assert.IsTrue(instance.Horse is RedHorse);
            ReferenceManager.Instance.Enabled = false;

        }

        [Test]
        public void MethodInjectTest()
        {
            ServiceRegistry.Current
                .Register(typeof(IPerson), typeof(Person4))
                .Register(typeof(IHorse), typeof(RedHorse));

            var instance = ServiceLocator.Get<IPerson,Person4>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Horse);
            Assert.IsTrue(instance.HasVisited);
            Assert.IsTrue(instance.Horse is RedHorse);

        }
        
       

        [Test]
        public void AssemblyLoaderTest()
        {
            ServiceRegistry.RegisteryFromAssemblyOf<IHorse>();

            var instance = ServiceLocator.Get<IPerson,Person4>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Horse);
            Assert.IsTrue(instance.HasVisited);
            Assert.IsTrue(instance.Horse is RedHorse);
        }

       

        [Test]
        public void GenericTest()
        {
            ServiceRegistry.Current
                .Register(typeof(IList<>),typeof(List<>))
                .Register(typeof(GenericCollection<>), typeof(GenericCollection<>));

            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<int>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<string>)));

            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection<>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection<int>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection<string>)));

            var coll = ServiceLocator.Get<GenericCollection<int>>();
            Assert.IsNotNull(coll);
            Assert.IsNotNull(coll.Items);

            var instance = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance);
            Assert.AreSame(coll.Items, instance);

            var instance2 = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance2);
            Assert.AreSame(coll.Items, instance2);


            var coll2 = ServiceLocator.Get<GenericCollection<string>>();
            var instance3 = ServiceLocator.Get<IList<string>>();
            Assert.IsNotNull(instance3);
            Assert.AreSame(coll2.Items, instance3);

            var instance4 = ServiceLocator.Get<IList<string>>();
            Assert.IsNotNull(instance4);
            Assert.AreSame(coll2.Items, instance4);


        }

        [Test]
        public void Generic2Test()
        {
            ServiceRegistry.Current
                .Register(typeof(IList<>), typeof(List<>))
                .Register(typeof(GenericCollection2<>), typeof(GenericCollection2<>));

            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<int>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<string>)));

            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection2<>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection2<int>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection2<string>)));

            var coll = ServiceLocator.Get<GenericCollection2<int>>();
            Assert.IsNotNull(coll);
            Assert.IsTrue(coll.HasInjected);
            Assert.IsNotNull(coll.Items);

            var instance = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance);
            Assert.AreSame(coll.Items, instance);

            var instance2 = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance2);
            Assert.AreSame(coll.Items, instance2);


            var coll2 = ServiceLocator.Get<GenericCollection2<string>>();
            Assert.IsNotNull(coll2);
            Assert.IsTrue(coll2.HasInjected);
            Assert.IsNotNull(coll2.Items);

            var instance3 = ServiceLocator.Get<IList<string>>();
            Assert.IsNotNull(instance3);
            Assert.AreSame(coll2.Items, instance3);

            var instance4 = ServiceLocator.Get<IList<string>>();
            Assert.IsNotNull(instance4);
            Assert.AreSame(coll2.Items, instance4);


        }

        [Test]
        public void Generic3Test()
        {

            ServiceRegistry.Current
                .Register(typeof(IList<>), typeof(List<>))
                .Register(typeof(GenericCollection3<>), typeof(GenericCollection3<>));

            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<int>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(IList<string>)));

            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection3<>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection3<int>)));
            Assert.IsTrue(ServiceRegistry.HasRegister(typeof(GenericCollection3<string>)));

            var coll = ServiceLocator.Get<GenericCollection3<int>>();
            Assert.IsNotNull(coll);
            Assert.IsTrue(coll.HasInjected);
            Assert.IsNotNull(coll.Items);

            var instance = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance);
            Assert.AreSame(coll.Items, instance);

            var instance2 = ServiceLocator.Get<IList<int>>();
            Assert.IsNotNull(instance2);
            Assert.AreSame(coll.Items, instance2);


            var coll2 = ServiceLocator.Get<GenericCollection3<string>>();
            Assert.IsNotNull(coll2);
            Assert.IsTrue(coll2.HasInjected);
            Assert.IsNotNull(coll2.Items);

            var instance3 = ServiceLocator.Get<IList<string>>();
            Assert.IsNotNull(instance3);
            Assert.AreSame(coll2.Items, instance3);

            var instance4 = ServiceLocator.Get<IList<string>>();
            Assert.IsNotNull(instance4);
            Assert.AreSame(coll2.Items, instance4);
        }

        [Test]
        public void Generic4Test()
        {
            var instance = ServiceLocator.Get<List<int>>();
            Assert.IsNotNull(instance);

            var instance2 = ServiceLocator.Get<List<int>>();
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance, instance2);

            var coll = ServiceLocator.Get<GenericCollection3<int>>();
            Assert.IsNotNull(coll);
          
           

            var coll2 = ServiceLocator.Get<GenericCollection3<string>>();
            Assert.IsNotNull(coll2);
          
            var instance3 = ServiceLocator.Get<List<string>>();
            Assert.IsNotNull(instance3);

            var instance4 = ServiceLocator.Get<List<string>>();
            Assert.IsNotNull(instance4);
            Assert.AreSame(instance3, instance4);
        }

        [Contract]
        interface IRepository<T, TId> { }
        [Contract]
        interface IRepository<T> :IRepository<T,long>{ }
        class Repository<T> : IRepository<T> { }
        class User { }
        class UserRepository : Repository<User> { }

        class EntityService<T>
        {
            [Inject]
            public IRepository<T> Repository;
        }

        class UserService : EntityService<User>
        {
        }
        class Department{}
        class DepartmentService : EntityService<Department>
        {
        }

        [Test]
        public void TestReposiotry()
        {
            var kernel = ServiceRegistry.Current as IKernel;
            kernel.Register(typeof(Repository<>));
            kernel.Register<UserRepository>();
            kernel.Register<UserService>();
            kernel.Register<DepartmentService>();

            Assert.IsTrue(kernel.HasRegister(typeof(IRepository<>)));
            Assert.IsTrue(kernel.HasRegister(typeof(IRepository<int>)));
            Assert.IsTrue(kernel.HasRegister(typeof(IRepository<int,long>)));
            Assert.IsTrue(kernel.HasRegister(typeof(IRepository<User>)));
            Assert.IsTrue(kernel.HasRegister(typeof(Repository<User>)));
            Assert.IsTrue(kernel.HasRegister(typeof(Repository<int>)));
           // Assert.IsFalse(registry.HasRegister(typeof(UserRepository)));

            var a = kernel.Get<IRepository<int>>();
            Assert.IsNotNull(a);
            var a2 = kernel.Get<IRepository<string>>();
            Assert.IsNotNull(a2);
            var a3 = kernel.Get<IRepository<User>>();
            Assert.IsNotNull(a3);

            //var b = registry.Get<IRepository<int, long>>();
            //Assert.IsNotNull(b);

            var type = typeof(IRepository<int, long>);
            var type2 = typeof(IRepository<int>);

            var args = type.GetGenericArguments();
            Assert.IsTrue(args.Length == 2);

            var departmentService = kernel.Get<DepartmentService>();
            var userService = kernel.Get<UserService>();
        }

    }

    [TestFixture]
    public class ChildKernelTest : TestBase
    {
        [Test]
        public void Should_null_when_not_set_parent_container()
        {
            var root = new Kernel();
            Assert.IsNull(root.Parent);
        }


        [Test]
        public void Should_not_null_when_set_parent_container()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = parent;
            Assert.IsNotNull(child.Parent);
        }

        [Test]
        public void Should_not_null_when_set_parent_container_is_null()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = null;
            Assert.IsNull(child.Parent);
        }

        [Test]
        public void Should_can_inherite_parent_resource()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = parent;
            Assert.IsNotNull(child.Parent);

            parent.Register<Person>();

            Assert.IsTrue(child.HasRegister<Person>());
        }

        [Test]
        public void Should_can_override_parent_resource()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = parent;
            Assert.IsNotNull(child.Parent);

            parent.Register<IPerson,Person>();

            Assert.IsTrue(child.HasRegister<IPerson>());

            var person = child.Get<IPerson>();
            Assert.IsNotNull(person);

            child.Register<IPerson, Person2>();
            var person2 = child.Get<IPerson>();

            Assert.AreNotEqual(person.GetType().Name, person2.GetType().Name);
        }

        [Test]
        public void Should_can_get_all_object_include_parent_resource_by_contrac()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = parent;
            Assert.IsNotNull(child.Parent);

            parent.Register<IPerson, Person>();
            child.Register<IPerson, Person2>();
            var items = child.GetAll<IPerson>();

            Assert.AreEqual(2, items.Count());

            items = parent.GetAll<IPerson>();
            Assert.AreEqual(1, items.Count());
        }

        [Test]
        public void Should_unregister_local_resource_when_exist_resource()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = parent;

            parent.Register<IPerson, Person>();
            child.Register<IPerson, Person2>();

            var items = child.GetAll<IPerson>();
            Assert.AreEqual(2, items.Count());

            child.UnRegister(typeof(IPerson));
            items = child.GetAll<IPerson>();
            Assert.AreEqual(1, items.Count());
        }

        [Test]
        public void Should_unregister_parent_container_resource_when_not_exist_resource()
        {
            var parent = new Kernel();
            var child = new Kernel();
            child.Parent = parent;

            parent.Register<IPerson, Person>();

            var items = child.GetAll<IPerson>();
            Assert.AreEqual(1, items.Count());

            child.UnRegister(typeof(IPerson));
            items = child.GetAll<IPerson>();
            Assert.AreEqual(0, items.Count());
        }
    }
}
