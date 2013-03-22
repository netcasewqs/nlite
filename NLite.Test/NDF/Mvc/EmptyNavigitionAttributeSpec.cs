using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Domain.Listener;

namespace NLite.Domain.Spec
{

    [Specification]
    public class EmptyNavigitionAttributeSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            //[Navigation(ControllerName="aa",ActionName="do")]
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
            ServiceLocator.Get<IServiceDispatcherConfiguationItem>().ListenManager.Register(new ControllerListener());
            // ServiceRegistry.Register<ControllerListener>();
            ServiceRegistry.Register<CalculteService>();
        }

        [Then]
        public void ActionResult_should_be_null()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual(5, Resp.Result);
            Assert.IsFalse(Req.Context.ContainsKey("NavigationResult"));
        }
    }

}
