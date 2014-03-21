using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Mini.Context;
using System.Reflection;
using NLite.DynamicProxy;
using NLite.Reflection;

namespace NLite.Mini.Proxy
{
    class EmitProxyFactory : IProxyFactory
    {
        public object Create(IComponentContext ctx, object instance, params object[] constructorArguments)
        {
            if (instance != null)
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

                var matchMethods = (ctx.Component.ExtendedProperties["methods"] as MethodInfo[]).Select(p=>p.Name).ToArray();

                //return EmitProxy.Proxy(instance, m => matchMethods.Contains(m), OnInterceptor, interfaces.ToArray());

                var baseType = contracts.FirstOrDefault();
                var baseInterfaces = contracts.Where(i => i.IsInterface).ToArray();
                var interceptor =  new DefaultInvocationHandler(instance, matchMethods);

                var proxy = NLite.DynamicProxy.ProxyFactory.CreateProxy(baseType,
                   interceptor,
                   baseInterfaces);

                return proxy;

            }
            throw new NotImplementedException();
        }

        public bool RequiresTargetInstance(IKernel registry, IComponentInfo model)
        {
            return true;
        }

    }
}
