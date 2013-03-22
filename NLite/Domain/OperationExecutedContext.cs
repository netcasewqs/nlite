using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NLite.Domain
{
    class OperationExecutedContext : IOperationExecutedContext, IOperationExecutingContext
    {
        public Object Service { get; internal set; }
        public IServiceRequest Request { get; private set; }
        public IOperationDescriptor OperationDescriptor { get; private set; }
        public IDictionary<string, object> Arguments { get { return Request.Arguments; } }
        public IServiceResponse Response { get; set; }
        bool IOperationExecutingContext.Cancelled { get; set; }

        public OperationExecutedContext(IServiceRequest serviceContext, IOperationDescriptor operationDescriptor)
        {
            Request = serviceContext;
            OperationDescriptor = operationDescriptor;
        }
    }


}
