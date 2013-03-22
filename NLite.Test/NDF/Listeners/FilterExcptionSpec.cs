using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Domain.Listener;
using NUnit.Framework;

namespace NLite.Domain.Spec
{

    [Specification]
    public class FilterExcptionSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class DemoFilterAttribute : OperationFilterAttribute
        {
            public override void OnOperationExecuting(IOperationExecutingContext ctx)
            {
                throw new InvalidOperationException();
            }
        }

        class CalculteService
        {
            [DemoFilter]
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
            Assert.IsNotNull(Resp.Exception);
            Assert.IsFalse(Resp.Success);
        }
    }

}
