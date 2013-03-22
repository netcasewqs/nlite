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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="registry"></param>
        /// <param name="bindingInfo"></param>
        /// <param name="onDestroying"></param>
        public override void Init(NLite.Mini.Activation.IActivator activator, IKernel kernel, IComponentInfo info, Action<IComponentInfo, object> onDestroying, Action<IComponentContext> onFetch)
        {
            Guard.NotNull(activator, "activator");
            Guard.NotNull(kernel, "kernel");
            Guard.NotNull(info, "info");
            Real.Init(new ProxyActivator(activator), kernel, info, onDestroying,OnFetch);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Real.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public override object Get(NLite.Mini.Context.IComponentContext ctx)
        {
            return Real.Get(ctx);
        }


    }
}
