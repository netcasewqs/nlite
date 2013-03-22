/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;


using NLite.Reflection;
using NLite.Internal;

namespace NLite.Domain
{
	/// <summary>
    /// 
    /// </summary>
     class DefaultServiceDescriptorManager : IServiceDescriptorManager
     {
         static readonly IServiceDescriptor[] EmptyServiceDescriptors = new IServiceDescriptor[0];

         Dictionary<string, IServiceDescriptor> Metadatas;
         Dictionary<Type, IServiceDescriptor[]> TypeMap;
         public event Action<IServiceDescriptor> ServiceDescriptorResolved;
         public event Action<IOperationDescriptor> OperationDescriptorResolved;
        // internal IServiceDispatcherConfiguationItem Options;

         //static string serviceSuffix;
         //static int serviceStringLength;
         //static DefaultServiceDescriptorManager()
         //{
         //    serviceSuffix = PropertyManager.Get<string>("serviceSuffix");
         //    if (string.IsNullOrEmpty(serviceSuffix))
         //        serviceSuffix = "Service";
         //    serviceStringLength = serviceSuffix.Length;
         //}

         public Func<Type, string> PopulateServiceName { get; private set; }
         public DefaultServiceDescriptorManager(Func<Type,string> populateServiceName)
         {
             Guard.NotNull(populateServiceName, "typeMatch");
             PopulateServiceName = populateServiceName;
             Metadatas = new Dictionary<string, IServiceDescriptor>(StringComparer.OrdinalIgnoreCase);
             TypeMap = new Dictionary<Type, IServiceDescriptor[]>();
         }



         public IServiceDescriptor[] Register(Type t)
         {
             if (TypeMap.ContainsKey(t))
                 return EmptyServiceDescriptors;

             //string serviceName = t.Name;
             //if (!serviceName.EndsWith(serviceSuffix))
             //    return EmptyServiceDescriptors;

             //var items = new List<IServiceDescriptor>();
             //serviceName = serviceName.Substring(0, serviceName.Length - serviceStringLength).ToLower();
             //items.Add(InternalRegister(t, serviceName));
             var serviceName = PopulateServiceName(t);
             if (string.IsNullOrEmpty(serviceName))
                 return EmptyServiceDescriptors;
             var items = new List<IServiceDescriptor>();
             items.Add(InternalRegister(t, serviceName));

             var attr = t.GetAttribute<AliasNameAttribute>(false);
             if (attr != null && !string.IsNullOrEmpty(attr.AliasNames))
             {
                 foreach (var aliasName in attr.AliasNames.Split(',').Where(p => !string.IsNullOrEmpty(p)))
                 {
                     var desc = InternalRegister(t, aliasName);
                     if(desc!= null)
                         items.Add(desc);
                 }
             }

             var arr = items.ToArray();
             TypeMap[t] = arr;
             return arr;
         }

         private IServiceDescriptor InternalRegister(Type t, string serviceName)
         {
             if (Metadatas.ContainsKey(serviceName))
                 return null;
             var sd = new ServiceDescriptor(t, serviceName);
             sd.OnOperationDescriptorResolved += OperationDescriptorResolved;
             Metadatas[serviceName.ToLower()] = sd;
             if (ServiceDescriptorResolved != null)
                 ServiceDescriptorResolved(sd);

             return sd;
         }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="serviceName"></param>
         /// <returns></returns>
         public IServiceDescriptor GetServiceDescriptor(string serviceName)
         {
             IServiceDescriptor desc = null;
             Metadatas.TryGetValue(serviceName, out desc);
             return desc;
         }

         public IServiceDescriptor[] GetServiceDescriptors<T>()
         {
             var serviceType = typeof(T);
             return TypeMap[serviceType];
         }

         public IEnumerable<IServiceDescriptor> ServiceDescriptors
         {
             get { return Metadatas.Values.ToArray(); }
         }

     }

}
