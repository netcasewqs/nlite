using NUnit.Framework;
using NLite.Cfg;
using NLite.Domain.Cfg;
using System.Linq;
using System.Linq.Expressions;
using System;

namespace NLite.Domain.Spec
{
    [TestFixture]
    public class TestBase
    {
        protected Configuration cfg = new Configuration();
        protected IServiceDispatcherConfiguationItem Options;

        [SetUp]
        public void Init()
        {
            cfg.Configure();
            Options = new ServiceDispatcherConfiguationItem(ServiceDispatcher.DefaultServiceDispatcherName);
            cfg.ConfigureServiceDispatcher(Options);
            OnInit();
        }

        protected virtual void OnInit() { }

        [TearDown]
        public void TearDown()
        {
            cfg.Clear();
        }

        class User
        {
            public int Id { get; set; }
        }
        [Test]
        public void GetById()
        {
            var items = Enumerable.Range(1, 10).Select(p => new User { Id = p }).AsQueryable();

            var idPram = Expression.Constant(4,typeof(int));
          
            var idProp = typeof(User).GetProperty("Id");

            var mParam = Expression.Parameter(typeof(User), "p");

            Expression propertyAccessExpression = Expression.PropertyOrField(mParam, "Id"); //Expression.MakeMemberAccess(modelExp, idProp);

            //var containExp = LambdaExpression.Lambda<Func<User, bool>>(Expression.Equal(idExp, argsExp));
            var eq = Expression.Equal(propertyAccessExpression, idPram);
            Console.WriteLine(eq);

            var eq2 = Expression.Lambda<Func<User, bool>>(eq, mParam);
           Console.WriteLine(eq2);

           var u = items.FirstOrDefault(eq2);
           Console.WriteLine(u.Id);
        }

        [Test]
        public void Contains()
        {
            var items = Enumerable.Range(1, 10).Select(p => new User { Id = p }).AsQueryable();

            var argsExp = Expression.Constant(new int[]{1,2}, typeof(int[]));
            var modelExp = Expression.Parameter(typeof(User), "p");

            Expression idExp = Expression.PropertyOrField(modelExp, "Id"); 

            var containExp = Expression.Call(typeof(Enumerable), "Contains", new [] { typeof(int) }, argsExp, idExp);
            Console.WriteLine(containExp);

            var eq2 = Expression.Lambda<Func<User, bool>>(containExp, modelExp);
            Console.WriteLine(eq2);

            var u = items.Where(eq2).ToArray();
            Console.WriteLine(u[1].Id);
        }
    }
}
