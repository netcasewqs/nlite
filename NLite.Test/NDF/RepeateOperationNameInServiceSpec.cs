using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    [Specification]
    public class RepeateOperationNameInServiceSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            public int Add(int a, int b)
            {
                return a + b;
            }

            public double Add(double a, double b)
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
        public void operation_name_should_be_not_repeation()
        {
            Assert.IsNotNull(CaughtException);


        }
    }

}
