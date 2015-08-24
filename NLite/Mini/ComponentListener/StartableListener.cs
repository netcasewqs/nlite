using System;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class StartableListener : ComponentListenerAdapter
    {
        
        /// <inheritdoc/>
        public override void OnPostInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            var startable = ctx.Instance as IStartable;
            if (startable != null)
                startable.Start();
        }


        /// <inheritdoc/>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            var startable = instance as IStartable;
            if (startable != null)
                startable.Stop();
        }
    }
}
