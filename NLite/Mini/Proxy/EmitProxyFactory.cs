using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Mini.Context;
using System.Reflection;
using NLite.Reflection;
using NLite.Interceptor;
using NLite.Reflection.Dynamic.Internal.Emit;
using NLite.Reflection.Dynamic;
using NLite.Mini.Activation;

namespace NLite.Mini.Proxy
{
    class EmitProxyFactory : IProxyFactory
    {
        public object Create(IComponentContext ctx, object instance, params object[] constructorArguments)
        {
            var contracts = ctx.Component.Contracts.Where(i => !string.IsNullOrEmpty(i.FullName)).ToArray();

            if (ctx.GenericParameters != null)
            {
                for (int i = 0; i < contracts.Length; i++)
                {
                    if (contracts[i].IsOpenGenericType() && contracts[i].GetGenericArguments().Length == ctx.GenericParameters.Length)
                    {
                        contracts[i] = contracts[i].MakeGenericType(ctx.GenericParameters);
                    }
                }
            }

            var matchMethods = (ctx.Component.ExtendedProperties["methods"] as MethodInfo[]).Select(p => p.Name).ToArray();

            var baseInterfaces = contracts.Where(i => i.IsInterface).ToArray();

            var invocationHandler = new SimpleInvocationHandler(matchMethods);

            var impType = ctx.Component.Implementation;

            if (impType.IsAbstract || impType.IsInterface)
                throw new ActivatorException(
                   string.Format(Mini_Resources.TypeAbstract,
                       impType.FullName,
                       ctx.Component.Contracts[0].FullName));

            if (impType.IsOpenGenericType())
                impType = impType.MakeCloseGenericType(ctx.GenericParameters);


            var proxy = NLite.Reflection.Proxy.NewProxyInstance(impType, baseInterfaces,
               invocationHandler, constructorArguments
               );

            return proxy;
        }

        public bool RequiresTargetInstance(IKernel registry, IComponentInfo model)
        {
            return false;
        }

        class SimpleInvocationHandler : NLite.Reflection.IInvocationHandler
        {
            private string[] methods;
            private InvocationHandler InnerHandler;
            public SimpleInvocationHandler(string[] methods)
            {
                this.methods = methods;
                InnerHandler = new InvocationHandler();
            }


            public object Invoke(object target, MethodInfo methodInfo, object[] parameters)
            {
                if (!methods.Contains(methodInfo.Name))
                {
                    return methodInfo.Invoke(target, parameters);
                }

                return InnerHandler.Invoke(target, methodInfo, parameters);
            }
        }

    }


}
