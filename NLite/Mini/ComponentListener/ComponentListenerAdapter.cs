using System;
using NLite.Mini.Context;
using System.Collections.Generic;
using System.Reflection;
using NLite.Reflection.Internal;

namespace NLite.Mini.Listener
{
  
    
   
    /// <summary>
    /// ×é¼þ¼àÌýÊÊÅäÆ÷
    /// </summary>
    public class ComponentListenerAdapter :  IComponentListener
    {
        /// <summary>
        /// 
        /// </summary>
        public IKernel Kernel { get; private set; }

        void IComponentListener.Init(IKernel kernel)
        {
            Kernel = kernel;
            if (kernel != null)
                Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init() { }
        /// <inheritdoc/>
        public virtual bool OnMetadataRegistering(IComponentInfo info)
        {
            return true;
        }
        /// <inheritdoc/>
        public virtual void OnMetadataRegistered(IComponentInfo info)
        {
        }

        /// <inheritdoc/>
        public virtual void OnMetadataUnregistered(IComponentInfo info)
        {
        }

        /// <inheritdoc/>
        public virtual void OnPreCreation(IComponentContext ctx)
        {
        }

        /// <inheritdoc/>
        public virtual void OnPostCreation(IComponentContext ctx)
        {
        }

        /// <inheritdoc/>
        public virtual void OnInitialization(IComponentContext ctx)
        {
        }

        /// <inheritdoc/>
        public virtual void OnPostInitialization(IComponentContext ctx)
        {
        }

        /// <inheritdoc/>
        public virtual void OnFetch(IComponentContext ctx)
        {
        }
        /// <inheritdoc/>
        public virtual void OnPreDestroy(IComponentInfo info, object instance)
        {
        }

        /// <inheritdoc/>
        public virtual void OnPostDestroy(IComponentInfo info)
        {
        }
    }
}
