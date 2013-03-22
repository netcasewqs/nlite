
//using System;
//using NLite.Domain.Listener;
//namespace NLite.Domain.Cfg
//{
//    [Serializable]
//    public class ServiceDescriptorConfigurationItem:Extension<ServiceDispatchOptions>
//    {
//        ServiceDescriptorManager descriptorManager = new ServiceDescriptorManager();
//        public override void Attach(ServiceDispatchOptions cfg)
//        {
//            var listner = new ServiceDescriptorListener(descriptorManager);
//            var kerner = ServiceLocator.Current as IKernel;
//            if (kerner != null)
//            {
//                kerner.RegisterInstance(descriptorManager);
//                kerner.ListenerManager.Register(listner);
//            }

//            cfg.ServiceDescriptorManager = descriptorManager;
//            descriptorManager.Options = cfg;
//        }

//        public override void Detach(ServiceDispatchOptions cfg)
//        {
//            cfg.ServiceDescriptorManager = null;
//            descriptorManager.Options = null;
//        }
//    }
//}
