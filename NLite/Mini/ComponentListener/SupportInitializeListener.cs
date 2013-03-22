#if !SILVERLIGHT
using System;
using System.ComponentModel;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class SupportInitializeListener:ComponentListenerAdapter
    {
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            var supportInit = ctx.Instance as ISupportInitialize;
            if (supportInit != null)
            {
                supportInit.BeginInit();
                supportInit.EndInit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            var supportInit = ctx.Instance as ISupportInitialize;
            if (supportInit != null)
            {
                supportInit.BeginInit();
                supportInit.EndInit();
            }
        }
    }
}
#endif