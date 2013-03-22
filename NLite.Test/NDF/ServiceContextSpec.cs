using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    [Specification]
    public class ServiceContextSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            public int Add(int a, int b)
            {
                Assert.IsNotNull(ServiceContext.Current);
                ServiceContext.Current.Context["AA"] = "AA";
                return a + b;
            }
        }

        protected override IServiceRequest SetupRequest()
        {
            Assert.IsNull(ServiceContext.Current);
            return ServiceRequest.Create("Calculte", "Add", new { a = 2, b = 3 });
        }

        protected override void RegiserService()
        {
            Assert.IsNull(ServiceContext.Current);
            ServiceRegistry.Register<CalculteService>();
        }

        [Then]
        public void filter_should_be_executed()
        {
            Assert.IsNull(ServiceContext.Current);
            Assert.AreEqual("AA", Req.Context["AA"]);
            Assert.IsTrue(Resp.Success);
            Assert.IsNotNull(Resp.Result);
        }


    }

}
