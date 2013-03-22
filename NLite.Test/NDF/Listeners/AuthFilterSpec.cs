using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Domain.Listener;

namespace NLite.Domain.Spec
{
    [Specification]
    public class AuthFilterSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class AuthFilterAttribute : OperationFilterAttribute
        {
            string[] Roles;

            public AuthFilterAttribute(string roles)
            {
                if (string.IsNullOrEmpty(roles))
                    Roles = new string[0];
                else
                    Roles = roles.Split(',');
            }

            public override void OnOperationExecuting(IOperationExecutingContext ctx)
            {
                if (NLiteEnvironment.Session["UserRole"] == null)
                    ctx.Cancelled = true;
            }
        }

        class CalculteService
        {

            [AuthFilter("admin")]
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

        protected override void When()
        {
            base.When();
        }

        [Then]
        public void filter_should_be_executed()
        {
            Assert.IsFalse(Resp.Success);
            Assert.IsNull(Resp.Result);
        }
    }

}
