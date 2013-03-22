using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Domain.Listener;

namespace NLite.Domain.Spec
{
    [Specification]
    public class FilterAttributeSpec : ServiceBrokerWithRegisterServiceSpec
    {
        static bool OnOperationExecuting, OnOperationExecuted;

        class EmptyFilterAttribute : OperationFilterAttribute { }
        class DemoFilterAttribute : OperationFilterAttribute
        {
            public override void OnOperationExecuting(IOperationExecutingContext ctx)
            {
                FilterAttributeSpec.OnOperationExecuting = true;
            }

            public override void OnOperationExecuted(IOperationExecutedContext ctx)
            {
                FilterAttributeSpec.OnOperationExecuted = true;
            }
        }

        class CalculteService
        {
            [DemoFilter]
            [EmptyFilter]
            public int Add(int a, int b)
            {
                return a + b;
            }


        }

        protected override IServiceRequest SetupRequest()
        {
            return ServiceRequest.Create("Calculte", "Add", new { a = 2, b = 3 });
        }

        protected override void RegiserService()
        {
            ServiceRegistry.Register<CalculteService>();
        }

        [Then]
        public void filter_should_be_executed()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual(5, Resp.Result);

            Assert.IsTrue(OnOperationExecuting);
            Assert.IsTrue(OnOperationExecuted);

        }
    }

}
