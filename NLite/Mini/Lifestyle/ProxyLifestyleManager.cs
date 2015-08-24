using System;
using NLite.Mini.Activation;
using NLite.Mini.Context;
using NLite.Internal;

namespace NLite.Mini.Lifestyle
{
    
    /// <summary>
    /// 
    /// </summary>
    public class ProxyLifestyleManager : LifestyleManagerAdapter
    {
        private readonly ILifestyleManager Real;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="real"></param>
        public ProxyLifestyleManager(ILifestyleManager real)
        {
            Guard.NotNull(real, "real");
            Real = real;
            Init(real.Activator, real.Kernel, real.Info, real.OnDestroying,real.OnFetch);
        }

        /// <inheritdoc/>
        public override void Init(NLite.Mini.Activation.IActivator activator, IKernel kernel, IComponentInfo info, Action<IComponentInfo, object> onDestroying, Action<IComponentContext> onFetch)
        {
            Guard.NotNull(activator, "activator");
            Guard.NotNull(kernel, "kernel");
            Guard.NotNull(info, "info");
            Real.Init(new ProxyActivator(activator), kernel, info, onDestroying,OnFetch);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            Real.Dispose();
        }

        /// <inheritdoc/>
        public override object Get(NLite.Mini.Context.IComponentContext ctx)
        {
            return Real.Get(ctx);
        }


    }
}
