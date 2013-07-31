using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Cfg;
using NLite.Domain.Cfg;

namespace NLite.Domain.Spec
{
    public abstract class ServiceBrokerWithRegisterServiceSpec : Specification<IServiceDispatcherConfiguationItem>
    {
        protected IServiceRequest Req { get; set; }
        protected IServiceResponse Resp { get; private set; }
        protected IServiceDescriptorManager ServiceManager { get; private set; }
        protected IServiceDescriptor[] ServiceMetadatas { get; set; }

        protected override void Given()
        {
            var cfg = new Configuration();
            cfg.ConfigureMiniContainer();

            SubjectUnderTest = new ServiceDispatcherConfiguationItem(ServiceDispatcher.DefaultServiceDispatcherName);
            cfg.ConfigureServiceDispatcher(SubjectUnderTest);
        }
        protected override void When()
        {
            ServiceManager = SubjectUnderTest.ServiceDescriptorManager;
            RegiserService();
            Req = SetupRequest();
        }

        protected override void Run()
        {
            Resp = ServiceDispatcher.Dispatch(Req);
            this.CaughtException = Resp.Exception;
        }

        protected abstract void RegiserService();

        abstract protected IServiceRequest SetupRequest();

    }

    
}
