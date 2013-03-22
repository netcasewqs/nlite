using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{

    [Specification]
    public class CreateServiceExceptionSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            public CalculteService()
            {
                throw new ServiceDispatcherException(ServiceDispatcherExceptionCode.CreateServiceException);
            }

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
        public void create_service_should_be_throw_Exception()
        {
            Assert.IsNotNull(Resp.Exception);
            Assert.IsFalse(Resp.Success);
        }
    }

 
}
