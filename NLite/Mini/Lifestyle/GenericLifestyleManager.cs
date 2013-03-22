using System;
using System.Collections.Generic;
using NLite.Mini.Activation;
using NLite.Mini.Context;
using NLite.Internal;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLifestyleManager"></typeparam>
    public class GenericLifestyleManager<TLifestyleManager> : ILifestyleManager where TLifestyleManager : ILifestyleManager, new()
    {
        /// <summary>
        /// 
        /// </summary>
        protected IDictionary<int, TLifestyleManager> cache = new Dictionary<int, TLifestyleManager>();

        #region ILifestyleManager Members
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
        /// <param name="onDestroyed"></param>
        public virtual void Init(IActivator activator, IKernel kernel, IComponentInfo info, Action<IComponentInfo, object> onDestroyed, Action<IComponentContext> onFetch)
        {
            Guard.NotNull(activator, "activator");
            Guard.NotNull(kernel, "kernel");
            Guard.NotNull(info, "info");
            this.Activator = activator;
            this.Kernel = kernel;
            this.Info = info;
            OnDestroying = onDestroyed;
            OnFetch = onFetch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        
        
        public  virtual object Get(IComponentContext ctx)
        {
            int key = 123;
            foreach (var item in ctx.GenericParameters)
                key = key ^ item.TypeHandle.Value.GetHashCode();

            TLifestyleManager lifestyleMgr;

            lock (cache)
            {
                if (!cache.TryGetValue(key, out lifestyleMgr))
                {

                    {
                        lifestyleMgr = new TLifestyleManager();
                        cache[key] = lifestyleMgr;
                    }
                    lifestyleMgr.Init(Activator, ctx.Kernel, ctx.Component, OnDestroying, OnFetch);
                }
            }

            var o = lifestyleMgr.Get(ctx);
            ctx.Instance = o;
            OnFetchCore(ctx);

            return o;
        }


        protected virtual void OnFetchCore(IComponentContext ctx)
        {
            if (OnFetch != null)
                OnFetch(ctx);
        }


        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (var item in cache.Values)
                item.Dispose();
        }

        #endregion
    }
}
