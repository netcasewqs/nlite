using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    [Specification]
    public class OverrideOperationNameInServiceSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            [AliasName("add4")]
            public int Add(int a, int b)
            {
                return a - b;
            }

            [Override("Add2,add3")]
            public double Add(double a, double b)
            {
                return a + b;
            }

            public void Add() { }
        }

        protected override IServiceRequest SetupRequest()
        {
            return ServiceRequest.Create("Calculte", "Add2", new { a = 2, b = 3 });
        }

        protected override void RegiserService()
        {
            ServiceRegistry.Register<CalculteService>();
        }

        [Then]
        public void should_be_return_5_when_2_Add_3()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual((double)5.0, Resp.Result);
        }
    }


}
