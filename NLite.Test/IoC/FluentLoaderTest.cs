using System.Collections.Generic;
using System.Linq;
using NLite;
using NUnit.Framework;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class FluentLoaderTest:TestBase
    {
        protected override void Init()
        {
            Person.HasVisited = LazyPerson.HasVisited = false;
        }

        [Test]
        public void RegisterTest()
        {
            ServiceRegistry.Register(ft => ft.To<Person>());
            //ServiceRegistry.Register(ft=>ft);
            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());
        }

        [Test]
        public void RegisterByFactoryTest()
        {
            ServiceRegistry.Register(ft => ft.Bind<IPerson>("person").Factory(() => new Person()));

            Assert.IsTrue(ServiceRegistry.HasRegister("person"));
            Assert.IsTrue(ServiceRegistry.HasRegister<IPerson>());

            var person = ServiceLocator.Get<IPerson>("person");
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<Person>("person");
            Assert.IsTrue(person2 != null);

            Assert.AreSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }

        [Test]
        public void RegisterByFactory2Test()
        {
            var person = new Person();

            ServiceRegistry.Register(f => f.Bind<IParameterConstructorInterface>()
                .Factory(() => new ParameterConstructorClass(10, "ZhangSan", person)));

            var instance = ServiceLocator.Get(typeof(IParameterConstructorInterface)) as IParameterConstructorInterface;

            Assert.IsNotNull(instance);
            Assert.AreEqual(10, instance.Id);
            Assert.IsTrue("ZhangSan" == instance.Name);
            Assert.AreSame(person, instance.Person);

        }


        [Test]
        public void GetByIdTest()
        {
            ServiceRegistry.Register(ft => ft.Bind<IPerson>("person").To<Person>());

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
            ServiceRegistry.Register(ft => ft.Named("person").Bind<IPerson>().To<Person>());

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get<IPerson, Person>();
            Assert.IsTrue(person2 != null);

            Assert.AreSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }

        [Test]
        public void GetAllTest()
        {
            ServiceRegistry.Register(ft => ft.Named("person").Bind<IPerson>().To<Person>())
                           .Register(ft => ft.Bind<IPerson>().To<Person2>());

            var person = ServiceLocator.Get<IPerson>();
            Assert.IsTrue(person != null);
            Assert.IsTrue(typeof(IPerson).IsAssignableFrom(person.GetType()));

            var items = ServiceLocator.GetAll<IPerson>().ToArray();
            Assert.IsTrue(items.Length == 2);
            Assert.IsTrue(items[0] is Person);
            Assert.IsTrue(items[1] is Person2);

        }

        [Test]
        public void TransientLifestyleTest()
        {
            ServiceRegistry.Register(ft => ft.Named("person").Bind<IPerson>().To<Person>().Transient());

            var person = ServiceLocator.Get(typeof(IPerson)) as IPerson;
            Assert.IsTrue(person != null);

            var person2 = ServiceLocator.Get(typeof(IPerson)) as Person;
            Assert.IsTrue(person2 != null);

            Assert.AreNotSame(person, person2);
            Assert.IsTrue(Person.HasVisited);
        }


        [Test]
        public void StartableListnerTest()
        {
            ServiceRegistry.Register(ft => ft.To<StartablePerson>());

            var person = ServiceLocator.Get(typeof(IPerson)) as StartablePerson;
            Assert.IsTrue(person != null);
            Assert.IsTrue(person.HasStarted);
            Assert.IsFalse(person.HasStopped);
        }

        [Test]
        public void ConstructorInjectTest()
        {
            ServiceRegistry.Register(f => f.To<Person2>())
                .Register(f => f.Bind<IHorse>().To<RedHorse>());

            var person = ServiceLocator.Get<IPerson,Person2>();
            Assert.IsTrue(person != null);
            Assert.IsNotNull(person.Horse);
        }

        [Test]
        public void ConstructorInject2Test()
        {

            ServiceRegistry.Current.RegisterInstance<string, string>("str", "ZhangSan")
                .Register(f => f.To<Person5>())
                .Register(f => f.To<RedHorse>())
                .Register(f => f.To<A>());

            var person = ServiceLocator.Get<IPerson,Person5>();
            Assert.IsTrue(person != null);
            Assert.IsNotNull(person.Horse);
            Assert.IsNotNull(person.A);
            Assert.IsTrue(person.HasVisited);
            Assert.IsTrue(string.IsNullOrEmpty(person.Name));
        }

        [Test]
        public void ConstructorInject3Test()
        {
            ServiceRegistry.Current.RegisterInstance<string, string>("str", "ZhangSan")
                  .Register(f => f.To<Person6>())
                  .Register( f => f.To<RedHorse>())
                  .Register(f => f.To<A>());


            var person = ServiceLocator.Get<IPerson, Person6>();
            Assert.IsTrue(person != null);
            Assert.IsNotNull(person.Horse);
            Assert.IsNotNull(person.A);
            Assert.IsTrue(person.HasVisited);
            Assert.AreEqual("ZhangSan", person.Name);
        }

        [Test]
        public void AutoConstructorTest()
        {
            ServiceRegistry.Register(f =>f.Bind<IPerson>().To<Person2>());

            var person = ServiceLocator.Get<IPerson,Person2>();
            Assert.IsTrue(person != null);
            Assert.IsNull(person.Horse);
        }

        [Test]
        public void GenericTest()
        {
            ServiceRegistry.Register(f=>f.Bind(typeof(IList<>)).To(typeof(List<>)))
                           .Register( f=>f.To( typeof(GenericCollection<>)));

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
            ServiceRegistry.Register(f => f.Bind(typeof(IList<>)).To(typeof(List<>)))
                           .Register(f => f.To(typeof(GenericCollection2<>)));

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

            ServiceRegistry.Register(f=>f.Bind( typeof(IList<>)).To(typeof(List<>)));
            ServiceRegistry.Register(f=>f.To( typeof(GenericCollection3<>)));

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
       
    }

    public class GenericCollection<T>
    {
        //属性注入
        [Inject]
        public IList<T> Items { get; set; }
    }


    public class GenericCollection2<T>
    {
        public IList<T> Items { get; private set; }

        public readonly bool HasInjected;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <modelExp name="items"></modelExp>
        public GenericCollection2(IList<T> items)
        {
            Items = items;
            HasInjected = true;
        }
    }


    public class GenericCollection3<T>
    {
        public IList<T> Items { get; private set; }

        public bool HasInjected { get; private set; }

        /// <summary>
        ///方法注入
        /// </summary>
        /// <modelExp name="items"></modelExp>
        [Inject]
        public void InjectItems(IList<T> items)
        {
            Items = items;
            HasInjected = true;
        }
    }
}
