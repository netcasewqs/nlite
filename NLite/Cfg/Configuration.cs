using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Domain.Cfg;
using NLite.Domain;

namespace NLite.Cfg
{
    /// <summary>
    /// 系统配置对象
    /// </summary>
    [Serializable]
    public class Configuration : CompoistionObject<Configuration>
    {
        /// <summary>
        /// 配置DI容器
        /// </summary>
        public void ConfigureMiniContainer()
        {
            Add<MiniContainerConfigurationItem>();
        }

        /// <summary>
        /// 配置服务分发器
        /// </summary>
        /// <code language="cs"><![CDATA[
        /// var cfg = new NLite.Cfg.Configuration();
        /// cfg.Configure();
        /// cfg.ConfigureServiceDispatcher();
        /// ]]>
        /// </code>
        public void ConfigureServiceDispatcher()
        {

            ConfigureServiceDispatcher(new ServiceDispatcherConfiguationItem(ServiceDispatcher.DefaultServiceDispatcherName));
        }

        /// <summary>
        /// 配置服务分发器
        /// </summary>
        /// <param name="serviceDispatcherName">服务分发器名称</param>
        /// <exception cref="ArgumentNullException">当<paramref name="serviceDispatcherName"/>为空引用时，将抛出异常</exception>
        public void ConfigureServiceDispatcher(string serviceDispatcherName)
        {
            if (string.IsNullOrEmpty(serviceDispatcherName))
                throw new ArgumentNullException("serviceDispatcherName");
            ConfigureServiceDispatcher( new ServiceDispatcherConfiguationItem(serviceDispatcherName));
        }

        /// <summary>
        /// 配置服务分发器
        /// </summary>
        /// <param name="item"></param>
        ///  <exception cref="ArgumentNullException">当<paramref name="item"/>为空引用时，将抛出异常</exception>
        public void ConfigureServiceDispatcher(IServiceDispatcherConfiguationItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (string.IsNullOrEmpty(item.Name))
                throw new ArgumentNullException("item.Name");
            if (ServiceRegistry.HasRegister(item.Name))
                throw new RepeatRegistrationException("Service dispatcher repeat configure for name:" + item.Name);
            Add(item);
        }

    }
  
}
