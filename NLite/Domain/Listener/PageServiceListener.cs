//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NLite.Reflection;
//using NLite.Domain.Http;

//namespace NLite.Domain.Listener
//{
//    public class PageServiceListener : IServiceDescriptorResolvedListener, IOperationDescriptorFoundListener
//    {
//        public const string PageServiceFlagKey = "_NLite_Domain_Http_PageService_";
//        public static readonly object ValidPlaceHold = new object();

//        void IServiceDescriptorResolvedListener.Notify(ServiceDescriptor serviceDescriptor)
//        {
//            var pageAttribute = serviceDescriptor.ServiceType.GetAttribute<PageAttribute>(true);
//            if (pageAttribute != null)
//                serviceDescriptor.Extensions[PageServiceListener.PageServiceFlagKey] = ValidPlaceHold;
//        }

//        void IOperationDescriptorFoundListener.Notify(IOperationDescriptorFoundContext ctx)
//        {
//            if (ctx.ServiceDescriptor.Extensions.ContainsKey(PageServiceFlagKey))
//            {
//                if (ctx.OperationDescriptor.Method.ReturnType != Types.Void)
//                    throw new ServiceDispatcherException(ServiceDispatcherExceptionCode.OperationNotFound)
//                {
//                    ServiceName = ctx.Request.ServiceName,
//                    OperationName = ctx.Request.OperationName
//                };
//                else
//                    ctx.Request.Context[PageServiceFlagKey] = ValidPlaceHold;
//            }
//        }

//    }
//}
