using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Domain.Mapping;
using NLite.Reflection;

namespace NLite.Domain.Listener
{
    /// <summary>
    /// Mvc 控制器插件监听器
    /// </summary>
    public class ControllerListener : ServiceDispatcherListener
    {
        public const string NavigationResult = "NavigationResult";
        public const string ErrorNavigation = "ErrorNavigation";
        public const string OperationNameSelector = "OperationNameSelector";

        /// <summary>
        /// 监听异常
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="operationDesc"></param>
        public override void OnExceptionFired(IServiceRequest req, IServiceResponse resp, IOperationDescriptor operationDesc)
        {
            IRedirectToErrorResult errorNav = null;
            if (operationDesc != null)
            {
                if (operationDesc.Extensions.ContainsKey(ControllerListener.ErrorNavigation))
                    errorNav = operationDesc.Extensions[ControllerListener.ErrorNavigation] as IRedirectToErrorResult;
                else if (operationDesc.ServiceDescriptor.Extensions.ContainsKey(ControllerListener.ErrorNavigation))
                    errorNav = operationDesc.ServiceDescriptor.Extensions[ControllerListener.ErrorNavigation] as IRedirectToErrorResult;
            }
            //执行失败后， 设置mvc的 controller serviceDispatcherName 和 ActionName
            if (errorNav != null)
            {
                req.Context[ControllerListener.NavigationResult] = errorNav;

                if (errorNav.IsSaveModelState)
                    resp.Result = req.Arguments.FirstOrDefault();//TODO:
            }
        }

        //TODO:
        public void OnServiceDescriptorResolved(IServiceDescriptor serviceDescriptor)
        {
            var errorNavigation = serviceDescriptor.ServiceType.GetAttribute<RedirectToErrorAttribute>(true);
            if (errorNavigation != null)
                serviceDescriptor.Extensions[ControllerListener.ErrorNavigation] = errorNavigation;
        }

        //TODO:
        public void OnOperationDescriptorResolved(IOperationDescriptor operationDescriptor)
        {
            var method = operationDescriptor.Method;
            var errorNavigation = method.GetAttribute<RedirectToErrorAttribute>(true);
            if (errorNavigation != null)
                operationDescriptor.Extensions[ControllerListener.ErrorNavigation] = errorNavigation;

            var navResult = method.GetAttribute<NavigationResultAttribute>(true);
            if (navResult != null)
                operationDescriptor.Extensions[ControllerListener.NavigationResult] = navResult;

            var actionSelector = method.GetAttribute<OperationNameSelectorAttribute>(true);
            if (actionSelector != null)
                operationDescriptor.Extensions[OperationNameSelector] = actionSelector;
        }

        public override void OnOperationDescriptorFound(IOperationDescriptorFoundContext ctx)
        {
            if (ctx.OperationDescriptor.Extensions.ContainsKey(ControllerListener.NavigationResult))
            {
                //设置mvc 的ActionResult 类型
                if (ctx.OperationDescriptor.Extensions.ContainsKey(ControllerListener.NavigationResult))
                    ctx.Request.Context[ControllerListener.NavigationResult] = ctx.OperationDescriptor.Extensions[ControllerListener.NavigationResult];
            }
        }
    }

}
