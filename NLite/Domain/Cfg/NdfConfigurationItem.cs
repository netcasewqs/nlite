using System;
using NLite.Cfg;
using NLite.Domain.Listener;
using NLite.Internal;

namespace NLite.Domain.Cfg
{
    /// <summary>
    /// 服务分发器配置节点
    /// </summary>
    [Serializable]
    public class ServiceDispatcherConfiguationItem : CompoistionObject<ServiceDispatcherConfiguationItem>, IExtension<Configuration>, IServiceDispatcherConfiguationItem
    {
        /// <summary>
        /// 监听管理器
        /// </summary>
        public IServiceDispatchListenerManager ListenManager { get; private set; }

        /// <summary>
        /// 计算服务名称
        /// </summary>
        public Func<Type, string> PopulateServiceName { get; private set; }
        /// <summary>
        /// 服务元数据管理器
        /// </summary>
        public ServiceDescriptorManager ServiceDescriptorManager { get; set; }
        /// <summary>
        /// 服务分发器工厂
        /// </summary>
        public Func<IServiceDispatcher> ServiceDispatcherCreator { get; set; }
        /// <summary>
        /// 节点名称，该名称必须是唯一的
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// 构造服务分发器
        /// </summary>
        /// <param name="serviceDispatcherName"></param>
        /// <param name="populateServiceName">检查特定的类型是否是服务类型,如果是返回服务名称，否则返回null</param>
        public ServiceDispatcherConfiguationItem(string serviceDispatcherName,Func<Type, string> populateServiceName) 
        {
            if (string.IsNullOrEmpty(serviceDispatcherName))
                throw new ArgumentNullException("serviceDispatcherName");
            Guard.NotNull(populateServiceName," populateServiceName");

            PopulateServiceName = populateServiceName;
            Name = serviceDispatcherName;
            ListenManager = new ServiceDispatchListenerManager();
            ServiceDispatcherCreator = ()=>new DefaultServiceDispatcher(this,ServiceLocator.Current);
          
        }

        /// <summary>
        /// 构造服务分发器
        /// </summary>
        /// <param name="serviceDispatcherName"></param>
        public ServiceDispatcherConfiguationItem(string serviceDispatcherName):this(serviceDispatcherName,ServiceDispatcher.GetServiceNameByDefault)
        {
        }

        

        /// <summary>
        /// 安装服务分发器配置
        /// </summary>
        /// <param name="cfg"></param>
        public void Attach(Configuration cfg)
        {
            var kernel = ServiceLocator.Current as IKernel;
            Guard.NotNull(kernel, "kernel");
            kernel.Register(s => s.Bind<IServiceDispatcherConfiguationItem>(Name).Factory(() => this));
            ServiceDescriptorManager = new ServiceDescriptorManager(PopulateServiceName);
        }

        public void ConfigureMvc()
        {
            ListenManager.Register(new ControllerListener());
        }

        /// <summary>
        /// 卸载服务分发器配置
        /// </summary>
        /// <param name="cfg"></param>
        public void Detach(Configuration cfg)
        {
            Clear();
        }

    }

   

}
