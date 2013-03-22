using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    [Specification]
    public class NonServiceNameSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
        }

        protected override IServiceRequest SetupRequest()
        {
            return ServiceRequest.Create("dsfaf", "Add", new { a = 2, b = 3 });
        }
        protected override void RegiserService()
        {
            ServiceRegistry.Register<CalculteService>();
        }

        [Then]
        public void service_should_be_not_found()
        {
            Assert.IsNotNull(Resp.Exception);
            Assert.IsFalse(Resp.Success);


        }
    }

}
