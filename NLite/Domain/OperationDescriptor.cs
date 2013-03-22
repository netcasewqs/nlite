using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Reflection;
using NLite.Domain.Mapping;
using NLite.Domain.Listener;
using System.ComponentModel;
using NLite.Binding;

namespace NLite.Domain
{
    /// <summary>
    /// 服务操作元数据描述接口
    /// </summary>
    public interface IOperationDescriptor
    {
        /// <summary>
        /// 参数绑定集合
        /// </summary>
        ICollection<BindingInfo> Bindings { get; }
        /// <summary>
        /// 方法描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 元数据扩展集合
        /// </summary>
        IDictionary<string, object> Extensions { get; }
        /// <summary>
        /// 操作过滤器集合
        /// </summary>
        OperationFilterAttribute[] Filters { get; }
        /// <summary>
        /// 计算方法实参
        /// </summary>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        IDictionary<string, object> GetParameterValues(IDictionary<string, object> valueProvider);
        /// <summary>
        /// 方法
        /// </summary>
        MethodInfo Method { get; }
        /// <summary>
        /// 方法选择器
        /// </summary>
        OperationSelectorAttribute MethodSelector { get; }
        /// <summary>
        /// 方法名选择器
        /// </summary>
        OperationNameSelectorAttribute NameSelector { get; }
        /// <summary>
        /// 操作名称 
        /// </summary>
        string OperationName { get; }
        /// <summary>
        /// 参数名称数组
        /// </summary>
        string[] ParameterNames { get; }
        /// <summary>
        /// 服务元数据
        /// </summary>
        IServiceDescriptor ServiceDescriptor { get; }

        /// <summary>
        /// 执行方法调用
        /// </summary>
        /// <param name="serviceInstance"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object Invoke(object serviceInstance, params object[] args);
    }

    /// <summary>
    /// 服务操作元数据描述类
    /// </summary>
    class OperationDescriptor : IOperationDescriptor
    {
        /// <summary>
        /// Get service operation serviceDispatcherName
        /// </summary>
        public string OperationName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Get service method
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// Get ServiceDescriptor
        /// </summary>
        public IServiceDescriptor ServiceDescriptor { get; private set; }

        internal readonly Func Executor;
        public IDictionary<string, object> Extensions { get; private set; }

        /// <summary>
        /// Get OperationFilterAttribute array.
        /// </summary>
        public OperationFilterAttribute[] Filters { get; private set; }
        public OperationNameSelectorAttribute NameSelector { get; private set; }
        public OperationSelectorAttribute MethodSelector { get; private set; }
        public ICollection<BindingInfo> Bindings { get; private set; }

        public string[] ParameterNames { get; private set; }

        internal OperationDescriptor(MethodInfo method, string operationName, IServiceDescriptor serviceDescriptor)
        {
            Extensions = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            Method = method;
            ParameterNames = method.GetParameters().Select(p => p.Name).ToArray();
            OperationName = operationName;
            ServiceDescriptor = serviceDescriptor;
            Executor = method.GetFunc();
            NameSelector = method.GetAttribute<OperationNameSelectorAttribute>(true);
            MethodSelector = method.GetAttribute<OperationSelectorAttribute>(true);
            Filters = method.GetAttributes<OperationFilterAttribute>(true).Distinct().ToArray();
            Bindings = method.GetParameters().Select(p => new BindingInfo( p)).ToArray();

            var descAttr = Method.GetAttribute<DescriptionAttribute>(true);
            if (descAttr != null)
                Description = descAttr.Description;
        }

        public IDictionary<string, object> GetParameterValues(IDictionary<string, object> valueProvider)
        {
            var args = new Dictionary<string, object>();
            if (valueProvider == null)
                return args;
            foreach (var p in Bindings)
                args[p.Name] = p.ModelBinder.BindModel(p, valueProvider);
            return args;
        }


        public object Invoke(object serviceInstance, params object[] args)
        {
            if (serviceInstance == null)
                throw new ArgumentNullException("serviceInstance");

            return Executor(serviceInstance, args);
        }
    }


}
