using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Domain.Mapping;
using NLite.Cfg;
using NLite.Domain.Listener;
using System.Reflection;
using NLite.Domain.Cfg;

namespace NLite.Domain.Spec
{
    public class ActionNameSelectorSpec:TestBase
    {
        class DemoService
        {
            [MultiButton("b1")]
            public void Action1()
            {
                Console.WriteLine("action1");
            }

            [MultiButton("b2")]
            public void Action2()
            {
                Console.WriteLine("action2");
            }
        }

        class MultiButtonAttribute : OperationNameSelectorAttribute
        {

            public string Name { get; set; }
            public MultiButtonAttribute(string name)
            {
                this.Name = name;
            }

            public override bool IsValidName(IServiceRequest req, string actionName, IOperationDescriptor operationDesc)
            {
                if (string.IsNullOrEmpty(this.Name))
                    return false;

                return req.Arguments.ContainsKey(this.Name);
            }
        }

        protected override void OnInit()
        {
            Options.ListenManager.Register(new ControllerListener());
            ServiceRegistry.Register<DemoService>();
        }

        [Test]
        //[ExpectedException(typeof(AmbiguousMatchException))]
        public void Should_be_not_match_b2_With_Action2()
        {
            var resp = ServiceDispatcher.Dispatch(ServiceRequest.Create("demo", "action1", new { b2 = 0 }));
            //if (resp.Exception != null)
            //    throw resp.Exception;
        }

        [Test]
        public void Should_be__match_b1_With_Action1()
        {
            ServiceDispatcher.Dispatch(ServiceRequest.Create("demo", "action1", new { b1 = 0 }));
        }
    }

    public class ActionName2SelectorSpec : TestBase
    {
        class DemoService
        {
            [AliasName("action")]
            [MultiButton("b1")]
            public void Action1()
            {
                Console.WriteLine("action1");
            }

             [AliasName("action")]
            [MultiButton("b2")]
            public void Action2()
            {
                Console.WriteLine("action2");
            }
        }

        class MultiButtonAttribute : OperationNameSelectorAttribute
        {

            public string Name { get; set; }
            public MultiButtonAttribute(string name)
            {
                this.Name = name;
            }

            public override bool IsValidName(IServiceRequest req, string actionName, IOperationDescriptor operationDesc)
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return false;
                }

                var flag = req.Arguments.ContainsKey(this.Name);

                return flag;
            }
        }

        protected override void OnInit()
        {
            Options.ListenManager.Register(new ControllerListener());
            ServiceRegistry.Register<DemoService>();
        }

        [Test]
        public void Should_be_not_match_b2_With_Action2()
        {
            ServiceDispatcher.Dispatch(ServiceRequest.Create("demo", "action", new { b2 = 0 }));
        }

        [Test]
        public void Should_be__match_b1_With_Action1()
        {
          

            ServiceDispatcher.Dispatch(ServiceRequest.Create("demo", "action", new { b1 = 0 }));
        }
    }
}
