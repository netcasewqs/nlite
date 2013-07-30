using System;
using System.Linq;
using System.Reflection;
using NLite.Mini.Context;
using NLite.Mini.Proxy;
using NLite.Reflection;
using NLite.Internal;
using NLite.Reflection.Internal;
using NLite.Mini.Resolving;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// 动态代理组件工厂
    /// </summary>
    public class ProxyActivator : AbstractActivator
    {
        private readonly IActivator Real;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="real"></param>
        public ProxyActivator(IActivator real)
        {
            Real = real;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Create(NLite.Mini.Context.IComponentContext context)
        {
            var impType = context.Component.Implementation;
            //if (impType.IsOpenGenericType())
            //    impType = impType.MakeCloseGenericType(context.GenericParameters);

            var Kernel = context.Kernel;
            var proxyFactory = Kernel.Get<IProxyFactory>();
            bool createProxy = proxyFactory != null;
           
            if (createProxy == false)
                return Real.Create(context);

            object instance = null;
            object[] args = null;

            if (proxyFactory.RequiresTargetInstance(Kernel, context.Component))
                instance = Real.Create(context);
            else
                args = GetConstructurArgs(context);

            try
            {
                instance = proxyFactory.Create(context, instance, args);
            }
            catch (Exception ex)
            {
                throw new ActivatorException("ComponentActivator: could not proxy " + impType.FullName,ex);
            }

            return instance;
        }

        private object[] GetConstructurArgs(IComponentContext context)
        {
           
            if (context.NamedArgs != null && context.NamedArgs.Count > 0)
                return context.NamedArgs.Values.ToArray();
            if (context.Args != null && context.Args.Length > 0)
                return context.Args;

            //const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var impType = context.Component.Implementation;
            var ctors = context.Component.ExtendedProperties[impType.FullName + ":ctorInjections"] as ConstructorInjection[];
            if (ctors != null && ctors.Length > 0)
            {
                var validConstructorBinding = ctors.FirstOrDefault(p => p.IsMatch);
                if (validConstructorBinding != null)
                    return validConstructorBinding.Dependencies.Select(p => p.ValueProvider()).ToArray();
            }

            return null;
        }

      
    }
}
