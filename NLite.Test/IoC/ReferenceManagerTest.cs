using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Mini.Resolving;
using NLite.Mini.Context;

namespace NLite.Test.IoC
{
    [TestFixture]
    public class ReferenceManagerTest
    {
        class A
        {
            [Inject]
            public B B;
            [Inject]
            public D D;
        }

        class B
        {
            [Inject]
            public C C;
        }

        class C
        {
            [Inject]
            public D D;
        }

        class D
        {
        }

        //[Test]
        //public void Test()
        //{
        //    var d = new D();
        //    var c = new C { D = d };
        //    var b = new B { C = c };
        //    var a = new A { B = b, D = d };

        //    ReferenceManager.Instance.RegisterHandle(CreateComponentContext(d));
        //    ReferenceManager.Instance.RegisterHandle(CreateComponentContext(c));
        //    ReferenceManager.Instance.RegisterHandle(CreateComponentContext(b));
        //    ReferenceManager.Instance.RegisterHandle(CreateComponentContext(a));

        //    ReferenceManager.Instance.AddReference(c, CreateComponentContext(d));
        //    Assert.AreSame(ReferenceManager.Instance.GetReferenceList(c).First().Instance, d);
        //    Assert.AreSame(ReferenceManager.Instance.GetReferredList(d).First().Instance, c);
        //    ReferenceManager.Instance.AddReference(b, CreateComponentContext(c));


        //    ReferenceManager.Instance.AddReference(a, CreateComponentContext(b));
        //    ReferenceManager.Instance.AddReference(a, CreateComponentContext(d));

        //    ReferenceManager.Instance.RemoveHandle(d);

        //}

        private IComponentContext CreateComponentContext(object o)
        {
            return new ComponentContext(null, null) { Instance = o };
        }
    }
}
