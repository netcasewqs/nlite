using System;
using System.Collections.Generic;
using NLite.Mini.Context;
using System.Reflection;
using NLite.Reflection;

namespace NLite.Mini.Resolving
{
    class ReinjectionManager
    {
        internal Dictionary<IComponentContext, IMemberInjection[]> reinjectionMap = new Dictionary<IComponentContext, IMemberInjection[]>();

        public void Reinjection(Type type, IKernel kernel)
        {
            // 重组
            foreach (var ctx in reinjectionMap.Keys)
            {
                var reInfo = reinjectionMap[ctx];
                bool supportReinjection = false;

                List<string> memberNames = null;
                foreach (var item in reInfo)
                {
                    if (item.IsAssignableFrom(type))
                    {
                        supportReinjection = true;

                        if (memberNames == null)
                            memberNames = new List<string>();
                        memberNames.Add(item.Member.Name);

                        item.Inject(ctx);
                    }
                }

                if (supportReinjection)
                {
                    var serviceReinjectedNotification = ctx.Instance as IServiceReinjectedNotification;
                    if (serviceReinjectedNotification != null)
                        serviceReinjectedNotification.OnReinjected(memberNames.ToArray());
                    else if(ctx.Component.ExtendedProperties.ContainsKey("ReinjectedNotification"))
                    {

                        var pair = (KeyValuePair<int, Delegate>)ctx.Component.ExtendedProperties["ReinjectedNotification"];
                        if (pair.Value != null)
                        {
                            if (pair.Key == 1)
                            {
                                pair.Value.DynamicInvoke(ctx.Instance, memberNames.ToArray());
                            }
                            else
                            {
                                pair.Value.DynamicInvoke(ctx.Instance);
                            }

                        }
                    }
                }
            }
        }
    }
}
