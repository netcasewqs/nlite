using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Collections;
using NLite.Reflection;
using NLite.ComponentModel;
using System.ComponentModel;
using NLite.Domain.Cfg;

namespace NLite.Domain
{
    /// <summary>
    /// 服务元数据描述接口
    /// </summary>
    public interface IServiceDescriptor : IEnumerable<IOperationDescriptor>
    {
        /// <summary>
        /// 服务标识
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; }
        /// <summary>
        /// 服务描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 服务类型
        /// </summary>
        Type ServiceType { get; }
        /// <summary>
        /// 通过操作名称获取特定的操作元数据
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        IOperationDescriptor this[string operationName] { get; }

        /// <summary>
        /// 扩展属性集合
        /// </summary>
        IDictionary<string, object> Extensions { get; }

        
    }

    /// <summary>
    /// 服务元数据描述类
    /// </summary>
    class ServiceDescriptor : IServiceDescriptor
    {
        private Dictionary<string, List<IOperationDescriptor>> cache = new Dictionary<string, List<IOperationDescriptor>>(StringComparer.OrdinalIgnoreCase);
        public string Id { get; private set; }

        /// <summary>
        /// Get service type
        /// </summary>
        public Type ServiceType { get; private set; }
        /// <summary>
        /// Get service serviceDispatcherName
        /// </summary>
        public string ServiceName { get; private set; }
        public string Description { get; private set; }
        public IDictionary<string, object> Extensions { get; private set; }

        private IDictionary<string, List<MethodInfo>> Methods;


        //internal IServiceDispatcherConfiguationItem Options;
        internal event Action<IOperationDescriptor> OnOperationDescriptorResolved;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceName"></param>
        internal ServiceDescriptor(Type serviceType, string serviceName)
        {
            Extensions = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            ServiceType = serviceType;
            ServiceName = serviceName;
            Id = serviceName + "__NLite_Domain_Service__" + serviceType.FullName;

            var descAttr = serviceType.GetAttribute<DescriptionAttribute>(true);
            if (descAttr != null)
                Description = descAttr.Description;

           

            Methods = new Dictionary<string, List<MethodInfo>>(StringComparer.OrdinalIgnoreCase);

            var allMethods = (from m in ServiceType
                                        .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public)
                                        .Where(p=>!p.HasAttribute<IgnoreAttribute>(false)
                                            && Types.Object != p.DeclaringType
                                            && !p.IsSpecialName)
                              let aliasNames = m.GetAttribute<AliasNameAttribute>(true)
                              let @override = m.GetAttribute<OverrideAttribute>(true)
                              select new { Name = m.Name, AliasName = aliasNames, Override = @override, Method = m,ParameterNames = m.GetParameters().Select(p=>p.Name).ToArray() }).ToArray();
            foreach (var item in allMethods)
            {

                if (item.Override != null)
                {
                    var aliases = item.Override.AliasNames.Split(',');
                    foreach (var alias in aliases)
                        AddMethod(alias, item.Method);
                }
                else
                {
                    if (Methods.ContainsKey(item.Name))
                    {
                        var repeatMethods = (from m in Methods[item.Name]
                                          let ps = m.GetParameters().Select(p => p.Name.ToLower()).ToArray()
                                          from p in ps
                                          where ps.Length == item.ParameterNames.Length
                                          where ps.All(s => item.ParameterNames.Contains(s))
                                          select m)
                                          .ToArray();
                        if (repeatMethods.Length > 1)
                            throw new AmbiguousMatchException(string.Format("Repeat method :[{0}] in service class :[{1}]", item.Name, ServiceType.FullName));
                    }

                    AddMethod(item.Name, item.Method);

                    if (item.AliasName != null)
                    {
                        var aliases = item.AliasName.AliasNames.Split(',');
                        foreach (var alias in aliases)
                            AddMethod(alias, item.Method);
                    }
                }

            }

           
        }

        private void AddMethod(string name, MethodInfo method)
        {
            List<MethodInfo> items;
            if (!Methods.TryGetValue(name, out items))
                items = new List<MethodInfo>(3);

            if (!items.Contains(method))
            {
                items.Add(method);
                if(!Methods.ContainsKey(name))
                    Methods.Add(name, items);
            }
        }

        /// <summary>
        /// Get OperationDescriptor by operation serviceDispatcherName
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public IOperationDescriptor this[string operationName]
        {
            get
            {
                if (!Methods.ContainsKey(operationName))
                    return null;

                var methodList = Methods[operationName];
                List<IOperationDescriptor> operationList;
                lock (cache)
                {
                    if (!cache.TryGetValue(operationName, out operationList))
                    {
                        operationList = methodList.Select(p =>
                        {
                            IOperationDescriptor operationDesc = new OperationDescriptor(p, operationName, this);
                            if (OnOperationDescriptorResolved != null)
                                OnOperationDescriptorResolved(operationDesc);
                            return operationDesc;
                        }).ToList();

                        cache[operationName] = operationList;
                    }
                }
              

                if (operationList.Count == 0)
                    throw new AmbiguousMatchException(string.Format("Not match method :[{0}] in service class :[{1}]", operationName, ServiceType.FullName));

                if (ServiceContext.Current != null)
                {
                    var req = ServiceContext.Current.Request;
                    if (operationList.Any(p => p.NameSelector != null))
                    {
                        foreach (var p in operationList)
                        {
                            if (p.NameSelector != null)
                            {
                                if (p.NameSelector.IsValidName(req, operationName, p))
                                    return p;
                            }


                        }
                        if (operationList.Count > 1)
                            throw new AmbiguousMatchException(string.Format("More than one method :[{0}] in service class :[{1}]", operationName, ServiceType.FullName));

                        throw new AmbiguousMatchException(string.Format("Not match method :[{0}] in service class :[{1}]", operationName, ServiceType.FullName));
                    }

                    if (operationList.Any(p => p.MethodSelector != null))
                    {
                        foreach (var p in operationList)
                        {
                            if (p.MethodSelector != null)
                            {
                                if (p.MethodSelector.IsValidForRequest(req, p))
                                    return p;
                            }
                        }

                        if (operationList.Count > 1)
                            throw new AmbiguousMatchException(string.Format("More than one method :[{0}] in service class :[{1}]", operationName, ServiceType.FullName));

                        throw new AmbiguousMatchException(string.Format("Not match method :[{0}] in service class :[{1}]", operationName, ServiceType.FullName));
                    }

                    foreach (var m in operationList.OrderByDescending(p => p.ParameterNames.Length))
                    {
                        if (m.ParameterNames.All(p => req.Arguments.ContainsKey(p)))
                            return m;
                    }
                }

                if (operationList.Count > 1)
                    throw new AmbiguousMatchException(string.Format("More than one  method :[{0}] in service class :[{1}]", operationName, ServiceType.FullName));

                return operationList[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IOperationDescriptor> GetEnumerator()
        {

            if (cache.Count != Methods.Count)
            {
                IOperationDescriptor od;
                foreach (var item in Methods)
                    od = this[item.Key];
            }
            return (from p in cache.Values
                     from m in p
                     select m)
                     .Distinct()
                     .GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
