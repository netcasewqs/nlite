using NLite;

namespace NLite.Test.IoC
{
    public class A
    {
        public A(B b)
        {
        }
        
        public A (){}
    }

    public class B
    {
        public B(C b)
        {
        }
    }

    public class C
    {
        public C()
        {
        }

        public int I; 
        public C(int i)
        {
            I = i;
        }
    }

   

    public class _A
    {
        public _B B { get; private set; }
        public _A(_B b)
        {
            B = b;
        }

        //public _A() { }
    }

    public class _B
    {
        //public _B(_A b)
        //{
        //}

        [Inject]
        public _A A { get; set; }
    }

   [Contract]
    interface IParameterConstructorInterface
    {
        int Id { get; }
        string Name { get; }
        IPerson Person { get; }
    }

    class ParameterConstructorClass : IParameterConstructorInterface
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public IPerson Person { get; private set; }

        public ParameterConstructorClass(int id, string name, IPerson person)
        {
            Id = id;
            Name = name;
            Person = person;
        }
    }

  

    class FieldInjectionModel
    {
        [Inject]
        IPerson person;

        public IPerson Person { get { return person; } }
    }

    [Contract]
    interface IPerson
    {
        string Name { get; set; }
    }

    class Person : IPerson
    {
        public string Name { get; set; }
        public static bool HasVisited;


        public Person()
        {
            HasVisited = true;
        }
    }

    [Contract]
    interface IHorse
    {
    }

    [Component]
    class RedHorse : IHorse
    {
    }

    class BlackHorse : IHorse
    {
    }

    class Person2 : IPerson
    {
        public string Name { get; set; }
        public IHorse Horse { get; set; }

        public Person2(IHorse horse)
        {
            Horse = horse;
        }

        public Person2() { }
    }

    class Person3 : IPerson
    {
        public string Name { get; set; }

        private IHorse horse;
        public bool HasVisited { get; private set; }

        [Inject]
        public IHorse Horse
        {
            get { return horse; }
            set
            {
                horse = value;
                HasVisited = true;
            }
        }

        public Person3() { }

        public Person3(IHorse horse)
        {
            Horse = horse;
        }

      
    }

    [Component]
    class Person4 : IPerson
    {
        public string Name { get; set; }

        public IHorse Horse { get; private set; }
        public bool HasVisited { get; private set; }

        public Person4()
        {
        }

        [Inject]
        void SetHorse(IHorse horse)
        {
            Horse = horse;
            HasVisited = true;
        }
    }
    
    class Person5 : IPerson
    {
        public string Name { get; set; }

        public IHorse Horse { get; private set; }
        public A A {get;private set;}
        
        public bool HasVisited { get; private set; }
        
        [Inject]
        public Person5(IHorse horse,A a)
        {
        	Horse = horse;
        	A = a;
        	HasVisited = true;
        }

        [Inject]
        public Person5(IHorse horse)
        {
            Horse = horse;
        }

        public Person5() { }
    }

    class Person6 : IPerson
    {
        public string Name { get; set; }

        public IHorse Horse { get; private set; }
        public A A {get;private set;}
        
        public bool HasVisited { get; private set; }
        
         [Inject]
        public Person6(string name,IHorse horse)
        {
        	Name = name;
        	Horse = horse;
        }
        
         [Inject]
        public Person6(string name,IHorse horse,A a)
        {
        	Name = name;
        	Horse = horse;
        	A = a;
            HasVisited = true;
        }
        
        [Inject]
        public Person6(IHorse horse,A a)
        {
        	Horse = horse;
        	A = a;
        
        }

        [Inject]
        public Person6(IHorse horse)
        {
            Horse = horse;
        }

         [Inject]
        public Person6() { }

        
    }
     
     
    interface ILazyPerson { }
    class LazyPerson : ILazyPerson
    {
        public string Name { get; set; }

        public static bool HasVisited;

        public LazyPerson()
        {
            HasVisited = true;
        }
    }
    
    

    class StartablePerson : IPerson, IStartable
    {
        public string Name { get; set; }

        public bool HasStarted;
        public bool HasStopped;

        public void Start()
        {
            HasStarted = true;
        }

        public void Stop()
        {
            HasStopped = true;
        }
    }
}
