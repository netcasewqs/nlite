using System;
using NLite.Mini.Resolving;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class DisposalListener:ComponentListenerAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instance"></param>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            var dis = instance as IDisposable;
            if (dis != null)
            {
                dis.Dispose();
            }
        }
    }
}
