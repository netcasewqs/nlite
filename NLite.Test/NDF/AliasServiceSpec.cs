using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    [Specification]
    public class AliasServiceSpec : ServiceBrokerWithRegisterServiceSpec
    {
        [AliasName("DemoCalculator,DemoCalculator2")]
        class CalculteService
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
        }

        protected override IServiceRequest SetupRequest()
        {
            return ServiceRequest.Create("DemoCalculator2", "Add", new { a = 2, b = 3 });
        }

        protected override void RegiserService()
        {
            ServiceRegistry.Register<CalculteService>();
            ServiceMetadatas = ServiceManager.GetServiceDescriptors<CalculteService>();
        }

        [Then]
        public void should_be_return_5_when_2_Add_3_With_ServiceAlias()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual(5, Resp.Result);
        }

        [And]
        public void service_metadata_count_should_be_two()
        {
            Assert.AreEqual(3, ServiceMetadatas.Length);
        }
    }
}
