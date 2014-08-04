/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NLite.Reflection;
using NLite.Collections;
using NLite.Mini.Listener;
using NLite.Internal;
using NLite.Mini;
using NLite.Mini.Context;

namespace NLite.Domain.Listener
{
	 /// <summary>
    /// 服务元数据组件监听器
    /// </summary>
     class ServiceDescriptorComponentListener : ComponentListenerAdapter
     {
         IServiceDescriptorManager ServiceDescriptorManager;
         private Kernel kernel;

         public ServiceDescriptorComponentListener(Kernel kernel, IServiceDescriptorManager serviceDescriptorManager)
         {
             Guard.NotNull(kernel, "kernel");
                 
             if(serviceDescriptorManager == null)
                 throw new ArgumentNullException("serviceProvider");

             this.kernel = kernel;
             ServiceDescriptorManager = serviceDescriptorManager;
         }

         IComponentInfo LastService;
         IServiceDescriptor[] serviceDescriptors;

         /// <inheritdoc/>
         public override void OnMetadataRegistered(IComponentInfo info)
         {
             if (LastService == info && serviceDescriptors != null && serviceDescriptors.Length > 0)
             {
                 LastService = null;

                 
                 var registration = (Kernel as Kernel) .IdStores[info.Id];
                 
                 foreach (var item in serviceDescriptors)
                 {
                     if (!Kernel.HasRegister(item.Id))
                     {
                       
                        
                         kernel.IdStores[item.Id] = registration;
                             
                     }
                 }
             }
         }

         public override bool OnMetadataRegistering(IComponentInfo info)
         {
             
             Type t = null;
             if (info.ExtendedProperties.ContainsKey("instance"))
             {
                 t = info.ExtendedProperties["instance"].GetType();
             }

             else
             {
                 t = info.Implementation;
             }

             if (t != null)
             {
                 serviceDescriptors = ServiceDescriptorManager.Register(t);
                 LastService = info;
             }

             return true;
         }
     }
}
