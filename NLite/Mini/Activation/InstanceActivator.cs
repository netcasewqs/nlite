using System;
using NLite.Mini.Context;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// ÊµÀý¹¤³§
    /// </summary>
    public class InstanceActivator : AbstractActivator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object InternalCreate(IComponentContext context)
        {
            return context.Component.ExtendedProperties["instance"];
        }

    }
}
