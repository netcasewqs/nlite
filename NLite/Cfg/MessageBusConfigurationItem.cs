using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Messaging;

namespace NLite.Cfg
{
    /// <summary>
    /// 消息总线配置节点
    /// </summary>
    public class MessageBusConfigurationItem : Extension<Configuration>
    {
        /// <summary>
        /// 安装消息总线
        /// </summary>
        /// <param name="owner"></param>
        public override void Attach(Configuration owner)
        {
            ServiceRegistry.Register(s => s.Bind<IMessageBus>("SimpleBus").To<SimpleBus>().Singleton());
        }

        /// <summary>
        /// 卸载消息总线
        /// </summary>
        /// <param name="owner"></param>
        public override void Detach(Configuration owner)
        {
            ServiceRegistry.Current.UnRegister("SimpleBus");
        }
    }
}
