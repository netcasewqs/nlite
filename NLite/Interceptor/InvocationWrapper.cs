using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.DynamicProxy
{
    class InvocationWrapper : NLite.Interceptor.IInvocationExceptionContext, NLite.Interceptor.IInvocationExecutingContext
    {
        internal NLite.DynamicProxy.InvocationInfo innerInvocation;

        public object Target { get; set; }

        public System.Reflection.MethodInfo Method
        {
            get { return innerInvocation.TargetMethod; }
        }

        public object[] Arguments
        {
            get { return innerInvocation.Arguments; }
        }

        public Exception Exception { get; set; }

        public bool ExceptionHandled { get; set; }

        public object Result { get; set; }
    }
}
