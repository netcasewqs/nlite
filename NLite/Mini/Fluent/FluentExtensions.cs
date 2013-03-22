using System;
using NLite.Reflection;
using NLite.Collections;
using NLite.Internal;
using NLite.Mini.Fluent;
using NLite.Mini.Fluent.Internal;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    public static class FluentExtensions
    {

        public static IServiceRegistry Register(this IServiceRegistry kernel, Action<IComponentExpression> handler)
        {
            Guard.NotNull(kernel, "kernel");
            Guard.NotNull(handler, "handler");
          
            var component = new ComponentExpression { Registry = kernel };
            handler(component);

            InternalRegister(kernel, component);

            return kernel;
        }

        private static void InternalRegister(IServiceRegistry kernel, ComponentExpression component)
        {
            if (component.Implementation == null && component.Factory == null)
                throw new ArgumentNullException("Implementation == null && Factory ==null");

            if (string.IsNullOrEmpty(component.Id))
            {
                if (component.Implementation != null)
                    component.Id = component.Implementation.FullName;
                else
                    component.Id = component.Factory.ToString() + "/" + component.Contracts[0].FullName;
            }

            if (component.Implementation != null && component.Implementation.IsOpenGenericType())
                component.Lifestyle = LifestyleType.GetGenericLifestyle(component.Lifestyle);

            kernel.Register(component);
        }

        //public static void Register(this IServiceRegistry registry, Action<IComponentExpression> handler)
        //{
        //    if (handler == null)
        //        throw new ArgumentNullException("handler == null");

        //    var component = new ComponentExpression { Registry = registry };
        //    handler(component);

        //    InternalRegister(registry, component);
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="registry"></param>
        ///// <param name="handlers"></param>
        ///// <returns></returns>
        //public static IServiceRegistry Register(this IServiceRegistry registry, params Action<IBindingExpression>[] handlers)
        //{
        //    if (handlers != null && handlers.Length > 0)
        //        handlers.ForEach(exp => Register(registry, exp));
        //    return registry;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="registry"></param>
        ///// <param name="handlers"></param>
        ///// <returns></returns>
        //public static IServiceRegistry Register(this IServiceRegistry registry, params Action<IComponentExpression>[] handlers)
        //{
        //    if (handlers != null && handlers.Length > 0)
        //        handlers.ForEach(exp => Register(registry, exp));
        //    return registry;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //public static IComponentExpression Singleton(this IComponentExpression exp)
        //{
        //    return exp.Lifestyle(LifestyleFlags.Singleton);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static IComponentExpression Singleton(this IComponentExpression exp)
        {
            Guard.NotNull(exp, "exp");
            return exp.Lifestyle(LifestyleFlags.Singleton);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //public static IFactoryExpression Transient(this IFactoryExpression exp)
        //{
        //    return exp.Lifestyle(LifestyleFlags.Transient);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static IComponentExpression Transient(this IComponentExpression exp)
        {
            Guard.NotNull(exp, "exp");
            return exp.Lifestyle(LifestyleFlags.Transient);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //public static IFactoryExpression Thread(this IFactoryExpression exp)
        //{
        //    return exp.Lifestyle(LifestyleFlags.Thread);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static IComponentExpression Thread(this IComponentExpression exp)
        {
            Guard.NotNull(exp, "exp");
            return exp.Lifestyle(LifestyleFlags.Thread);
        }
    }
}
