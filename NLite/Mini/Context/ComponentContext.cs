using System;
using System.Collections.Generic;
using NLite.Mini.Lifestyle;

namespace NLite.Mini.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class ComponentContext : IComponentContext
    {
        /// <inheritdoc/>
        public IKernel Kernel { get; private set; }

        /// <inheritdoc/>
        public ILifestyleManager LifestyleManager { get; internal set; }

        /// <inheritdoc/>
        public IComponentInfo Component { get; internal set; }

        /// <inheritdoc/>
        public IDictionary<string, object> NamedArgs { get; set; }

        /// <inheritdoc/>
        public object[] Args { get;  set; }

        /// <inheritdoc/>
        public Type[] GenericParameters { get; internal set; }

        /// <inheritdoc/>
        public object Instance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="gernericParameters"></param>
        public ComponentContext(IKernel kernel, Type[] gernericParameters)
        {
            Kernel = kernel;
            GenericParameters = gernericParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public IComponentContext Init(IComponentInfo info)
        {
            Component = info;
            
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="info"></param>
        /// <param name="args"></param>
        /// <param name="genericParameters"></param>
        public ComponentContext(IKernel kernel, IComponentInfo info, IDictionary<string, object> args, Type[] genericParameters)
        {
            Kernel = kernel;
            Component = info;
            NamedArgs = args;
            GenericParameters = genericParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="info"></param>
        /// <param name="args"></param>
        /// <param name="genericParameters"></param>
        public ComponentContext(IKernel kernel, IComponentInfo info, object[] args, Type[] genericParameters)
        {
            Kernel = kernel;
            Component = info;
            Args = args;
            GenericParameters = genericParameters;
        }
    }
}
