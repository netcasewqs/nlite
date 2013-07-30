using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Cfg
{
    /// <summary>
    /// Mini容器配置节点
    /// </summary>
    public class MiniContainerConfigurationItem : Extension<Configuration>
    {
        /// <summary>
        /// 配置DI容器
        /// </summary>
        /// <param name="cfg"></param>
        public override void Attach(Configuration cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException("cfg");
            NLiteEnvironment.Refresh();
        }

        /// <summary>
        /// 卸载DI容器
        /// </summary>
        /// <param name="cfg"></param>
        public override void Detach(Configuration cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException("cfg");
            var container = ServiceRegistry.Current as IKernel;
            if (container != null)
                container.Dispose();
        }
    }

    /// <summary>
    /// 异常配置节点
    /// </summary>
    public class ExceptionConfigurationItem : Extension<Configuration>
    {
        private readonly Action<IServiceRegistry> registerHandler;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerHandler"></param>
        public ExceptionConfigurationItem(Action<IServiceRegistry> registerHandler)
        {
            this.registerHandler = registerHandler;
        }

        /// <summary>
        /// 配置异常
        /// </summary>
        /// <param name="cfg"></param>
        public override void Attach(Configuration cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException("cfg");

            var serviceRegistry = ServiceRegistry.Current;
            if (serviceRegistry == null)
                throw new InvalidOperationException("No config DI container");

            if (registerHandler != null)
                registerHandler(serviceRegistry);

            if (!serviceRegistry.HasRegister<IExceptionCode>())
                serviceRegistry.Register<ExceptionCode>();
            //if (!serviceRegistry.HasRegister<IExceptionRender>())
            //    serviceRegistry.Register<DebugExceptionRender>();
            if (!serviceRegistry.HasRegister<UnknowExceptionResolver>())
                serviceRegistry.Register<UnknowExceptionResolver>();
            if (!serviceRegistry.HasRegister<IExceptionHandler>())
                serviceRegistry.RegisterInstance<IExceptionHandler, ExceptionHandler>(new ExceptionHandler());
        }

        /// <summary>
        /// 卸载异常配置
        /// </summary>
        /// <param name="cfg"></param>
        public override void Detach(Configuration cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException("cfg");
            base.Detach(cfg);
        }
    }
}
