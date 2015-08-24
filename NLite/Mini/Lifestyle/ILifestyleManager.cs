using System;
using NLite.Mini.Activation;
using NLite.Mini.Context;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILifestyleManager:IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        IKernel Kernel { get; }

        /// <summary>
        /// 
        /// </summary>
        IActivator Activator { get; }

        /// <summary>
        /// 
        /// </summary>
        IComponentInfo Info { get; }

        /// <summary>
        /// 
        /// </summary>
        Action<IComponentInfo, object> OnDestroying { get; }

        /// <summary>
        /// 
        /// </summary>
        Action<IComponentContext> OnFetch { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="kernel"></param>
        /// <param name="info"></param>
        /// <param name="onDestroy"></param>
        /// <param name="onFetch"></param>
        void Init(IActivator activator, IKernel kernel, IComponentInfo info, Action<IComponentInfo, object> onDestroy, Action<IComponentContext> onFetch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        object Get(IComponentContext ctx);
    }
}
