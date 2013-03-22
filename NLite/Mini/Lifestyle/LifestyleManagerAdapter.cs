using System;
using NLite.Mini.Activation;
using NLite.Mini.Context;
using NLite.Internal;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 
    /// </summary>
    public class LifestyleManagerAdapter:BooleanDisposable,ILifestyleManager
    {
        /// <summary>
        /// 
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IActivator Activator { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IComponentInfo Info { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<IComponentInfo, object> OnDestroying { get; private set; }

        public Action<IComponentContext> OnFetch { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="registry"></param>
        /// <param name="bindingInfo"></param>
        /// <param name="onDestroying"></param>
        public virtual void Init(IActivator activator, IKernel kernel, IComponentInfo info, Action<IComponentInfo, object> onDestroying, Action<IComponentContext> onFetch)
        {
            Guard.NotNull(activator, "activator");
            Guard.NotNull(kernel, "kernel");
            Guard.NotNull(info, "info");
            this.Activator = activator;
            this.Kernel = kernel;
            this.Info = info;
            OnDestroying = onDestroying;
            OnFetch = onFetch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public virtual object Get(IComponentContext ctx)
        {
            var o = Activator.Create(ctx);
            OnFetchCore(ctx);
            return o;
        }

        protected virtual void  OnFetchCore(IComponentContext ctx)
        {
            if(OnFetch != null)
                OnFetch(ctx);
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnDestroying = null;
                OnFetch = null;
            }
           
        }
    }
}
