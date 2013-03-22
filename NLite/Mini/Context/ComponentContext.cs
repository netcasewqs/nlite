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
        /// <summary>
        /// 
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ILifestyleManager LifestyleManager { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IComponentInfo Component { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, object> NamedArgs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object[] Args { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        public Type[] GenericParameters { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="gernericParameters"></param>
        public ComponentContext(IKernel kernel, Type[] gernericParameters)
        {
            Kernel = kernel;
            GenericParameters = gernericParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <returns></returns>
        public IComponentContext Init(IComponentInfo info)
        {
            Component = info;
            
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="bindingInfo"></param>
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
        /// <param name="registry"></param>
        /// <param name="bindingInfo"></param>
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
