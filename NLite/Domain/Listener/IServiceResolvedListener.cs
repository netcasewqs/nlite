using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Listener
{
   

    sealed class ServiceResolvedContext :  IServiceResolvedContext
    {
        public IServiceDescriptor ServiceDescriptor { get; private set; }
        public IOperationDescriptor OperationDescriptor { get; internal set; }
        public IServiceRequest Request { get; private set; }
        public IServiceResponse Response { get; private set; }

        public bool Cancelled { get; set; }
        public object Service { get; set; }

        public ServiceResolvedContext(IServiceDescriptor serviceDescriptor, IServiceRequest req, IServiceResponse resp)
        {
            ServiceDescriptor = serviceDescriptor;
            Request = req;
            Response = resp;
        }
        
    }
}
