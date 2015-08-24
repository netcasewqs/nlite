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
        /// <inheritdoc/>
        public IKernel Kernel { get; private set; }

        /// <inheritdoc/>
        public IActivator Activator { get; private set; }

        /// <inheritdoc/>
        public IComponentInfo Info { get; private set; }
        /// <inheritdoc/>
        public Action<IComponentInfo, object> OnDestroying { get; private set; }
        /// <inheritdoc/>
        public Action<IComponentContext> OnFetch { get; private set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public virtual object Get(IComponentContext ctx)
        {
            var o = Activator.Create(ctx);
            OnFetchCore(ctx);
            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        protected virtual void  OnFetchCore(IComponentContext ctx)
        {
            if(OnFetch != null)
                OnFetch(ctx);
        }


        /// <inheritdoc/>
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
