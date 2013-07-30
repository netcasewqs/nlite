using System;
using System.Linq;
using System.Reflection;
using NLite.Mini.Resolving;
using NLite.Reflection;
using NLite.Internal;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class MemberExportListener:ComponentListenerAdapter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            if (ctx.Component.Implementation == null
                || ctx.Instance == null)
                return;

            if (!ctx.Component.ExtendedProperties.ContainsKey("MembersRegistered"))
                return;

            var exports = ctx.Component.ExtendedProperties["MembersRegistered"] as IExportInfo[];

            if (exports == null)
                return;

            foreach (var item in exports)
                item.Execute(ctx);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instance"></param>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            if (info.ExtendedProperties.ContainsKey("MembersRegistered"))
            {
                var exports = info.ExtendedProperties["MembersRegistered"] as IExportInfo[];
                foreach (var item in exports)
                    ServiceRegistry.Current.UnRegister(item.Id);
            }
        }
    }
}
