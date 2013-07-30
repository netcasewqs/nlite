using System;
using System.Linq;
using System.Diagnostics;
using NLite.Collections;
using NLite.Reflection;
using System.Collections.Generic;
using NLite.Internal;
using NLite.Mapping.Internal;
using NLite.Collections.Internal;

namespace NLite
{
    
    #if !SILVERLIGHT
    [Serializable]
    #endif
    [DebuggerDisplay("{Id}/{Lifestyle}")]
    /// <summary>
    /// 组件元数据信息接口
    /// </summary>
    public class ComponentInfo : IComponentInfo
    {
        /// <summary>
        /// 得到组件Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 得到组件实现的所有契约
        /// </summary>
        public Type[] Contracts { get { return contracts.ToArray(); } }

        /// <summary>
        /// 得到组件的具体类型
        /// </summary>
        public Type Implementation { get; private set; }

        /// <summary>
        /// 得到或设置组件的生命周期
        /// </summary>
        public LifestyleFlags Lifestyle { get; set; }

        /// <summary>
        /// 得到或设置组件的工厂类型（该属性常常作为组件的自定义工厂）
        /// </summary>
        public string Activator { get; set; }

        /// <summary>
        /// 是否全局服务，指的是该组件是否注册到根容器中
        /// </summary>
        public bool Global { get; set; }

        /// <summary>
        /// 得到组件的工厂函数
        /// </summary>
        public Func<object> Factory { get; internal set; }

        /// <summary>
        /// 得到组件的扩展属性
        /// </summary>
        public IDictionary<string,object> ExtendedProperties
        {
            get
            {
                return extendedProperties;
            }
        }

        private string Name;
        private HashSet<Type> contracts = new HashSet<Type>();
        private IDictionary<string, object> extendedProperties = new Dictionary<string,object>();
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="factory"></param>
        /// <param name="lifestyleType"></param>
        public ComponentInfo(string id, Type contract, Func<object> factory, LifestyleFlags lifestyleType)
        {
            Guard.NotNull(contract,"contract");
            Guard.NotNull(factory,"factory");
          
            contracts.Add(contract);
            ContractService.GetContracts(contract, contracts);

            Id = id;
            if (string.IsNullOrEmpty(id))
                Id = factory.ToString() + "/" + contract.FullName;

            if (contract.IsOpenGenericType())
                Lifestyle = LifestyleType.GetGenericLifestyle(lifestyleType);
            else
                Lifestyle = LifestyleType.GetLifestyle(lifestyleType);

            Activator = ActivatorType.Factory;
            Factory = factory;
            Name = Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="factory"></param>
        public ComponentInfo(string id, Type contract, Func<object> factory) : this(id, contract, factory, LifestyleType.Default) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="factory"></param>
        public ComponentInfo(Type contract, Func<object> factory) : this(null, contract, factory) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="implementation"></param>
        /// <param name="lifestyleType"></param>
        public ComponentInfo(string id, Type contract, Type implementation, LifestyleFlags lifestyleType):this(id,contract,implementation,null,lifestyleType){}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="implementation"></param>
        /// <param name="activator"></param>
        /// <param name="lifestyleType"></param>
        public ComponentInfo(string id, Type contract, Type implementation, string activator, LifestyleFlags lifestyleType)
        {
            Guard.NotNull(implementation, "implementation");
           
            
            if (contract == null)
                contract = implementation;

            if (!contract.IsGenericType || contract.IsCloseGenericType())
                Guard.IsTrue(contract.IsAssignableFrom(implementation));

            contracts.Add(contract);
            ContractService.GetContracts(implementation,contracts);

            Id = id;
            if (string.IsNullOrEmpty(id))
                Id = implementation.FullName;

            if (contract.IsOpenGenericType())
                Lifestyle = LifestyleType.GetGenericLifestyle(lifestyleType);
            else
                Lifestyle = LifestyleType.GetLifestyle(lifestyleType);

            Activator = activator;
            if (string.IsNullOrEmpty(activator))
                Activator = ActivatorType.Default;


            Name = Id;
            Implementation = implementation;
            if (implementation != null)
            {
                Name = implementation.FullName + "/" + contract.FullName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="implementation"></param>
        public ComponentInfo(string id, Type contract, Type implementation) : this(id, contract, implementation, ActivatorType.Default, LifestyleType.Default) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="implementation"></param>
        public ComponentInfo(Type contract, Type implementation) : this(null, contract, implementation) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="implementation"></param>
        public ComponentInfo(Type implementation) : this(null, implementation) { }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ComponentInfo left, ComponentInfo right)
        {
            if (left != null)
                return left.Equals(right);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ComponentInfo left, ComponentInfo right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals((IComponentInfo)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IComponentInfo other)
        {
            if (other == null)
                return false;
            if (Implementation != other.Implementation)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
