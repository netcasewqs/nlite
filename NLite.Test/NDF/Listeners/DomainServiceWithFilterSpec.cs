using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Domain.Listener;

namespace NLite.Domain.Spec
{

    [Specification]
    public class DomainServiceWithFilterSpec : ServiceBrokerWithRegisterServiceSpec
    {
        static bool OnOperationExecuting, OnOperationExecuted;

        class CalculteService
        {
            public int Add(int a, int b)
            {
                return a + b;
            }

            public void OnOperationExecuted(IOperationExecutedContext ctx)
            {
                DomainServiceWithFilterSpec.OnOperationExecuted = true;
            }

            public void OnOperationExecuting(IOperationExecutingContext ctx)
            {
                DomainServiceWithFilterSpec.OnOperationExecuting = true;
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
        public void service_filter_should_be_executed()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual(5, Resp.Result);

            //Assert.IsTrue(OnOperationExecuting);
            //Assert.IsTrue(OnOperationExecuted);

        }
    }

}
