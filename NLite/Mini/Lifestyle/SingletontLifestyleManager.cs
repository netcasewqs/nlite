using System;
using NLite.Mini.Context;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 
    /// </summary>
    public class SingletontLifestyleManager : LifestyleManagerAdapter
    {
        private volatile Object instance;

       /// <summary>
       /// 
       /// </summary>
       /// <param name="ctx"></param>
       /// <returns></returns>
        public override object Get(IComponentContext ctx)
        {
            if (instance == null)
            {
                lock (Activator)
                {
                    if (instance == null)
                    {
                        instance = base.Get(ctx);
                    }
                }
            }

            return instance;
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (instance == null) return;

            if (OnDestroying != null)
                OnDestroying(Info, instance);
            var dis = instance as IDisposable;
            if (dis != null)
                dis.Dispose();
            instance = null;

            base.Dispose(disposing);
        }
    }

    
}
