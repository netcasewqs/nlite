﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Domain.Mapping;
using NLite.Domain.Listener;

namespace NLite.Domain.Spec
{

    public class InheribeNavigationResultAttributeSpec : ServiceBrokerWithRegisterServiceSpec
    {
        class CalculteBaseService
        {
            [JsonResult]
            public virtual int Add(int a, int b)
            {
                return a + b;
            }
        }

        class CalculteService : CalculteBaseService
        {
            public override int Add(int a, int b)
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

            ServiceRegistry.Register<CalculteService>();
        }

        [Then]
        public void should_inherite_super_Attribute()
        {
            Assert.IsTrue(Resp.Success);
            Assert.IsNotNull(Resp.Result);
            Assert.IsNotNull(Req.Context["NavigationResult"]);
            Assert.IsInstanceOf<IJsonResult>(Req.Context["NavigationResult"]);
        }
    }

}
