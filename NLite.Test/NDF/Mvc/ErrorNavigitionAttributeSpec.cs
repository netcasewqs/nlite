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
    public class ErrorNavigitionAttributeSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteService
        {
            [RedirectToActionAttribute("do", ControllerName = "aa")]
            [RedirectToError(ControllerName = "home", ActionName = "error")]
            public int Add(int a, int b)
            {
                throw new NLiteException();
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

            ServiceRegistry.Register<CalculteService>();
        }


        [Then]
        public void ControllName_And_ActionName_should_be_default()
        {
            Assert.IsNotNull(Resp.Exception);
            Assert.IsFalse(Resp.Success);

            var redirectToError = Req.Context["NavigationResult"] as IRedirectToErrorResult;
            Assert.IsNotNull(redirectToError);
            Assert.That("home", Is.EqualTo(redirectToError.ControllerName).IgnoreCase);
            Assert.That("error", Is.EqualTo(redirectToError.ActionName).IgnoreCase);
        }
    }

}
