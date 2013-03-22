using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace NLite.Domain.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class OperationSelectorAttribute : Attribute
    {
        public abstract bool IsValidForRequest(IServiceRequest req, IOperationDescriptor operationDesc);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AcceptVerbsAttribute : OperationSelectorAttribute
    {
        public ICollection<string> Verbs
        {
            get;
            private set;
        }

        public AcceptVerbsAttribute(params string[] verbs)
        {
            Verbs = new ReadOnlyCollection<string>(verbs);
        }

        public override bool IsValidForRequest(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            string httpMethodOverride = req.Arguments["_method"] as string;
            return this.Verbs.Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpDeleteAttribute : OperationSelectorAttribute
    {
        private static readonly AcceptVerbsAttribute _innerAttribute = new AcceptVerbsAttribute("Delete");
        public override bool IsValidForRequest(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            return _innerAttribute.IsValidForRequest(req, operationDesc);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpGetAttribute : OperationSelectorAttribute
    {
        private static readonly AcceptVerbsAttribute _innerAttribute = new AcceptVerbsAttribute("Get");
        public override bool IsValidForRequest(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            return _innerAttribute.IsValidForRequest(req, operationDesc);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpPostAttribute : OperationSelectorAttribute
    {
        private static readonly AcceptVerbsAttribute _innerAttribute = new AcceptVerbsAttribute("Post");
        public override bool IsValidForRequest(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            return _innerAttribute.IsValidForRequest(req, operationDesc);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpPutAttribute : OperationSelectorAttribute
    {
        private static readonly AcceptVerbsAttribute _innerAttribute = new AcceptVerbsAttribute("Put");
        public override bool IsValidForRequest(IServiceRequest req, IOperationDescriptor operationDesc)
        {
            return _innerAttribute.IsValidForRequest(req, operationDesc);
        }
    }
}
