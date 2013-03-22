using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLite.Collections;
using NLite.Mini.Context;
using NLite.Reflection;
using NLite.Internal;
using NLite.Reflection.Internal;
using NLite.Mini.Activation.Internal;
using NLite.Mini.Resolving;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// 缺省组件工厂
    /// </summary>
    public class DefaultActivator : AbstractActivator
    {
        private static readonly object Mutex = new object();

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object InternalCreate(IComponentContext context)
        {
            return CreateInstance(context);
        }


        internal object CreateInstance(IComponentContext context)
        {
            ConstructorInfo constructorInfo = null;
            var impType = context.Component.Implementation;

            if (impType.IsAbstract || impType.IsInterface)
                throw ExceptionManager.HandleAndWrapper<ActivatorException>(
                   string.Format(Mini_Resources.TypeAbstract,
                       impType.FullName,
                       context.Component.Contracts[0].FullName));

            if (impType.IsOpenGenericType())
                impType = impType.MakeCloseGenericType(context.GenericParameters);

            if (context.NamedArgs != null && context.NamedArgs.Count > 0)
            {
                constructorInfo = impType.FindEligibleConstructor(context.NamedArgs);
                if (impType.IsValueType)
                    return Activator.CreateInstance(impType);
                context.Instance = constructorInfo.FastInvoke(context.NamedArgs.Values.ToArray());
            }
            else if (context.Args != null && context.Args.Length > 0)
            {
                constructorInfo = impType.FindEligibleConstructor(context.Args);
                if (impType.IsValueType)
                    return Activator.CreateInstance(impType);
                context.Instance = constructorInfo.FastInvoke(context.Args);
            }
            else
            {
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                var ctors = context.Component.ExtendedProperties[impType.FullName + ":ctorInjections"] as ConstructorInjection[];
                if (ctors != null && ctors.Length > 0)
                {
                    var validConstructorBinding = ctors.FirstOrDefault(p => p.IsMatch);
                    if (validConstructorBinding == null)
                    {
                        throw ExceptionManager.HandleAndWrapper<ActivatorException>(
                         string.Format(Mini_Resources.NoConstructorsAvailable, impType, bindingFlags));
                    }

                    validConstructorBinding.Inject(context);
                }
                else
                {
                      if (impType.IsValueType)
                          return Activator.CreateInstance(impType);

                    
                      throw ExceptionManager.HandleAndWrapper<ActivatorException>(
                        string.Format(Mini_Resources.NoConstructorsAvailable, impType, bindingFlags));
                }

               
            }

            return context.Instance;
        }
    }
}
