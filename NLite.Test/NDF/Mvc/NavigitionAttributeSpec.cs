using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Domain.Mapping;
using NUnit.Framework;
using NLite.Domain.Listener;

namespace NLite.Domain.Spec
{
    [Specification]
    public class NavigitionAttributeSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            [RedirectToActionAttribute("do", ControllerName = "aa")]
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
            var controllerListener = new ControllerListener();
            base.SubjectUnderTest.ListenManager.Register(controllerListener);
            base.ServiceManager.ServiceDescriptorResolved += controllerListener.OnServiceDescriptorResolved;
            base.ServiceManager.OperationDescriptorResolved += controllerListener.OnOperationDescriptorResolved;
            // ServiceRegistry.Register<ControllerListener>();
            ServiceRegistry.Register<CalculteService>();
        }


        [Then]
        public void ControllName_And_ActionName_should_be_default()
        {
            Assert.IsNull(Resp.Exception);
            Assert.IsTrue(Resp.Success);
            Assert.AreEqual(5, Resp.Result);

            var redirectToAction = Req.Context["NavigationResult"] as IRedirectToActionResult;
            Assert.IsNotNull(redirectToAction);
            Assert.That("aa", Is.EqualTo(redirectToAction.ControllerName).IgnoreCase);
            Assert.That("do", Is.EqualTo(redirectToAction.ActionName).IgnoreCase);
        }
    }

}
