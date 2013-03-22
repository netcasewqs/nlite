using System;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class StartableListener : ComponentListenerAdapter
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            var startable = ctx.Instance as IStartable;
            if (startable != null)
                startable.Start();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="instance"></param>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            var startable = instance as IStartable;
            if (startable != null)
                startable.Stop();
        }
    }
}
