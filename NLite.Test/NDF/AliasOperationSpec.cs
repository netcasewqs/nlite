using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec
{
    [Specification]
    public class AliasOperationSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            [AliasName("do")]
            public int Add(int a, int b)
            {
                return a + b;
            }
        }

        protected override IServiceRequest SetupRequest()
        {
            return ServiceRequest.Create("Calculte", "do", new { a = 2, b = 3 });
        }

        protected override void RegiserService()
        {
            ServiceRegistry.Register<CalculteService>();
            ServiceMetadatas = ServiceManager.GetServiceDescriptors<CalculteService>();
        }

        [Then]
        public void should_be_return_5_when_2_Add_3_With_Alias_Operation_Name()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual(5, Resp.Result);
        }

        [And]
        public void operation_metadata_count_should_be_two()
        {
            var add = ServiceMetadatas[0]["add"];//因为OperationDescriptor 是Lazy Create的
            Assert.AreEqual(2, ServiceMetadatas[0].Count());
        }
    }
}
