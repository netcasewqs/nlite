using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Domain.Listener;

namespace NLite.Domain.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class OperationNameSelectorAttribute : Attribute
    {
        public abstract bool IsValidName(IServiceRequest req, string actionName, IOperationDescriptor operationDesc);

        //bool IOperationDescriptorFindingListener.Notify(IServiceRequest req, string actionName, OperationDescriptor operationDesc)
        //{
        //    return IsValidName(req, actionName, operationDesc);
        //}
    }
}
