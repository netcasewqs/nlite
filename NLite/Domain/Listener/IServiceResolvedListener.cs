using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Listener
{
    //[Contract]
    //public interface IServiceDescriptorResolvedListener : IListener
    //{
    //    void Notify(ServiceDescriptor serviceDescriptor);
    //}

    //[Contract]
    //public interface IServiceDescriptorFoundListener : IListener
    //{
    //    void Notify(IServiceDescriptorFoundContext ctx);
    //}


    //[Contract]
    //public interface IOperationDescriptorResolvedListener : IListener
    //{
    //    void Notify(OperationDescriptor operationDescriptor);
    //}

   
    ////[Contract]
    ////public interface IOperationDescriptorFindingListener : IListener
    ////{
    ////    bool Notify(IServiceRequest req, string actionName, OperationDescriptor methodInfo);
    ////}

    //[Contract]
    //public interface IOperationDescriptorFoundListener : IListener
    //{
    //    void Notify(IOperationDescriptorFoundContext ctx);
    //}

    //[Contract]
    //public interface IServiceResolvedListener : IListener
    //{
    //    void Notify(IServiceResolvedContext ctx);
    //}

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
