using System;
using NLite.Cfg;
using NLite.Domain.Listener;
using System.Collections.Generic;
using NLite.Internal;

namespace NLite.Domain
{
    /// <summary>
    /// 服务分发器配置项接口
    /// </summary>
    [Contract]
    public interface IServiceDispatcherConfiguationItem:IExtension<Configuration>
    {
        /// <summary>
        /// 节点名称，该名称必须是唯一的
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 服务分发器工厂
        /// </summary>
        Func<IServiceDispatcher> ServiceDispatcherCreator { get; }
        /// <summary>
        /// 服务元数据管理器
        /// </summary>
        ServiceDescriptorManager ServiceDescriptorManager { get; set; }
        /// <summary>
        /// 监听管理器
        /// </summary>
        IServiceDispatchListenerManager ListenManager { get; }
    }

    /// <summary>
    /// 服务元数据管理器
    /// </summary>
    public class ServiceDescriptorManager : IServiceDescriptorManager
    {
        /// <summary>
        /// 内部原始的元数据管理器
        /// </summary>
        protected readonly IServiceDescriptorManager Inner;
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public ServiceDescriptorManager(Func<Type, string> populateServiceName)
        {
            var kernel = ServiceRegistry.Current as IKernel;
            Guard.NotNull(kernel, "kernel");
            if (!kernel.HasRegister<IServiceDescriptorManager>())
            {
                Inner = new DefaultServiceDescriptorManager(populateServiceName);
                var listner = new ServiceDescriptorComponentListener(Inner);
                kernel.RegisterInstance(Inner);
                kernel.ListenerManager.Register(listner);
            }
            else
                Inner = kernel.Get<DefaultServiceDescriptorManager>();

            Inner.ServiceDescriptorResolved += OnServiceDescriptorResolved;
            Inner.OperationDescriptorResolved += OnOperationDescriptorResolved;
        }

        public virtual Func<Type, string> PopulateServiceName { get; private set; }
        /// <summary>
        /// 注册领域服务元数据
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual IServiceDescriptor[] Register(Type serviceType)
        {
            Guard.NotNull(serviceType, "serviceType");
            return Inner.Register(serviceType);
        }

        /// <summary>
        /// 得到领域服务元数据
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public virtual IServiceDescriptor GetServiceDescriptor(string serviceName)
        {
            Guard.NotNull(serviceName, "serviceName");
            return Inner.GetServiceDescriptor(serviceName);
        }

        /// <summary>
        /// 得到领域服务元数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IServiceDescriptor[] GetServiceDescriptors<T>()
        {
            return Inner.GetServiceDescriptors<T>();
        }

        /// <summary>
        /// 得到所有的领域服务元数据列表
        /// </summary>
        public virtual System.Collections.Generic.IEnumerable<IServiceDescriptor> ServiceDescriptors
        {
            get { return Inner.ServiceDescriptors; }
        }

        /// <summary>
        /// 在领域服务元数据被解析后触发
        /// </summary>
        /// <param name="sd"></param>
        protected virtual void OnServiceDescriptorResolved(IServiceDescriptor sd)
        {
        }

        /// <summary>
        /// 在操作元数据被解析后触发
        /// </summary>
        /// <param name="od"></param>
        protected virtual void OnOperationDescriptorResolved(IOperationDescriptor od)
        {
        }

        event Action<IServiceDescriptor> IServiceDescriptorManager.ServiceDescriptorResolved
        {
            add { Inner.ServiceDescriptorResolved += value; }
            remove { Inner.ServiceDescriptorResolved -= value; }
        }

        event Action<IOperationDescriptor> IServiceDescriptorManager.OperationDescriptorResolved
        {
            add { Inner.OperationDescriptorResolved += value; }
            remove { Inner.OperationDescriptorResolved -= value; }
        }
    }
    
}
