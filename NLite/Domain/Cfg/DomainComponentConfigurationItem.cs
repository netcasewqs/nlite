using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Cfg;
using NLite.Internal;

namespace NLite.Domain.Cfg
{
    /// <summary>
    /// 领域组件配置节点
    /// </summary>
    [Serializable]
    public class DomainComponentConfigurationItem : Extension<Configuration>
    {
        private readonly Action<IServiceRegistry> registerHandler;
        /// <summary>
        /// 构造领域配置节点
        /// </summary>
        /// <param name="registerHandler"></param>
        public DomainComponentConfigurationItem(Action<IServiceRegistry> registerHandler)
        {
            Guard.NotNull(registerHandler, "registerHandler");
            this.registerHandler = registerHandler;
        }

        /// <summary>
        /// 配置领域组件
        /// </summary>
        /// <param name="owner"></param>
        public override void Attach(Configuration owner)
        {
            registerHandler(ServiceRegistry.Current);
        }
      
    }
}
