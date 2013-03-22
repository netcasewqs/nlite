using System;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class InitializationListener:ComponentListenerAdapter
    {
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            var startable = ctx.Instance as IInitializable;
            if (startable != null)
                startable.Init();
        }

       
    }
}
