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

namespace NLite.Domain.Listener
{
	 /// <summary>
    /// 
    /// </summary>
     class ServiceDescriptorComponentListener : ComponentListenerAdapter
     {
         IServiceDescriptorManager ServiceDescriptorManager;

         public ServiceDescriptorComponentListener(IServiceDescriptorManager serviceDescriptorManager)
         {
             if(serviceDescriptorManager == null)
                 throw new ArgumentNullException("serviceProvider");

             ServiceDescriptorManager = serviceDescriptorManager;
         }

         Type LastService;

         public override bool OnMetadataRegistering(IComponentInfo info)
         {
             if (info.Implementation == LastService || info.ExtendedProperties.ContainsKey("instance"))
             {
                 LastService = null;
                 return true;
             }

             var t = info.Implementation;
             if (t.IsClass && !t.IsAbstract)
             {
                 var rs = ServiceDescriptorManager.Register(t);
                 if (rs != null && rs.Length > 0)
                 {
                     //Kernel.UnRegister(info.Id);
                     LastService = t;
                     foreach (var item in rs)
                     {
                         if (!Kernel.HasRegister(item.Id))
                             Kernel.Register(s => s.Named(item.Id).Bind(t).To(t).Transient());
                     }
                 }
             }

             return true;
         }
     }
}
