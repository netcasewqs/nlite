using System;
using System.Collections.Generic;
using NLite.Mini.Context;

namespace NLite.Mini.Resolving
{
    class ReinjectionManager
    {
        internal Dictionary<IComponentContext, IInjection[]> reinjectionMap = new Dictionary<IComponentContext, IInjection[]>();


        public void Reinjection(Type type, IKernel kernel)
        {
            // 重组
            foreach (var ctx in reinjectionMap.Keys)
            {
                var reInfo = reinjectionMap[ctx];
                bool supportReinjection = false;

                foreach (var item in reInfo)
                {
                    if (item.IsAssignableFrom(type))
                    {
                        supportReinjection = true;
                        item.Inject(ctx);
                    }
                }

                if (supportReinjection)
                {
                    var serviceReinjectedNotification = ctx.Instance as IServiceReinjectedNotification;
                    if (serviceReinjectedNotification != null)
                        serviceReinjectedNotification.OnReinjected();
                }
            }
        }
    }
}
