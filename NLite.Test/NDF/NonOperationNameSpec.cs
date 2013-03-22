using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{

    [Specification]
    public class NonOperationNameSpec : ServiceBrokerWithRegisterServiceSpec
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
            return ServiceRequest.Create("Calculte", "wwd", new { a = 2, b = 3 });
        }

        protected override void RegiserService()
        {
            ServiceRegistry.Register<CalculteService>();
            ServiceMetadatas = ServiceManager.GetServiceDescriptors<CalculteService>();
        }


        [Then]
        public void action_should_be_not_found()
        {
            Assert.IsNotNull(Resp.Exception);
            Assert.IsFalse(Resp.Success);

        }

        [And]
        public void operation_metadata_count_should_be_one()
        {
            Assert.AreEqual(1, ServiceMetadatas[0].Count());
        }
    }

}
