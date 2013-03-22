using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using NLite.Reflection;
using NLite.Mapping.Internal;
using NLite.Internal;

namespace NLite.Mini.Resolving
{
    /// <summary>
    /// 依赖接口
    /// </summary>
    interface IDependency
    {
        /// <summary>
        /// 得到或设置一个值来指示是否是可选依赖
        /// </summary>
        bool IsOptional { get; set; }
        /// <summary>
        /// 得到或设置缺省依赖值，只有IsOptional=true时，该值才有意义
        /// </summary>
        object DefaultValue { get; set; }
        /// <summary>
        /// 依赖类型
        /// </summary>
        Type DependencyType { get; }
        /// <summary>
        /// 是否可以通过容器得到依赖对象
        /// </summary>
        bool HasDependencied { get; }
        /// <summary>
        /// 依赖对象的提供者
        /// </summary>
        Func<object> ValueProvider { get; }
        /// <summary>
        /// 判断当前依赖的Type是否是指定类型的基类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool IsAssignableFrom(Type t);
        /// <summary>
        /// 当新组件注册时进行刷新
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="registry"></param>
        void Refresh(IComponentInfo info, IKernel kernel);
        /// <summary>
        /// 
        /// </summary>
        event Action OnRefresh;
    }

    

    abstract class DependencyBase : IDependency
    {
        public bool IsOptional { get; set; }
        public object DefaultValue { get; set; }
        public Type DependencyType { get; internal set; }
        public abstract event Action OnRefresh;
        private bool hasDependencied;
        public bool HasDependencied
        {
            get
            {
                if (!hasDependencied)
                    return IsOptional;
                return true;
            }
            set
            {
                hasDependencied = value;
            }
        }
        public Func<object> ValueProvider { get; internal set; }
        public virtual bool IsAssignableFrom(Type t) { return false; }
        public virtual void Refresh(IComponentInfo info, IKernel kernel) { }
    }

    /// <summary>
    /// 实例句柄
    /// </summary>
    public interface IInstanceHandle : IDisposable
    {
        /// <summary>
        /// 组件信息
        /// </summary>
        IComponentInfo Component { get; }
        /// <summary>
        /// 实例
        /// </summary>
        object Instance { get; }
        /// <summary>
        /// 创建该实例的内核容器
        /// </summary>
        IKernel Kernel { get; }
        /// <summary>
        /// 被引用的实例句柄
        /// </summary>
        IInstanceHandle[] ReferredList { get; }
        /// <summary>
        /// 引用的实例句柄
        /// </summary>
        IInstanceHandle[] ReferenceList { get; }
    }

    /// <summary>
    /// 实例引用管理器
    /// </summary>
    [DebuggerDisplay("{Items}")]
    public class ReferenceManager
    {
        private ReferenceManager() { }

        /// <summary>
        /// 得到单例引用管理器对象
        /// </summary>
        public static readonly ReferenceManager Instance = new ReferenceManager();

        private Dictionary<object, InstanceHandle> Items = new Dictionary<object, InstanceHandle>();
        static readonly IInstanceHandle[] EmptyArray = new IInstanceHandle[0];

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

      
       /// <summary>
       /// 注册句柄
       /// </summary>
       /// <param name="instance"></param>
       /// <param name="registry"></param>
       /// <param name="component"></param>
        public void RegisterHandle(object instance,IKernel kernel,IComponentInfo component)
        {
            Guard.NotNull(instance, "instance");
            Guard.NotNull(kernel, "kernel");
            Guard.NotNull(component, "component");

            InstanceHandle handle;
            if(!Items.TryGetValue(instance,out handle))
                Items.Add(instance,new InstanceHandle(kernel,component,instance));
        }

        /// <summary>
        /// 移除指定内核创建的所有实例句柄
        /// </summary>
        /// <param name="registry"></param>
        public void UnregisterAllByKernel(IKernel kernel)
        {
            Guard.NotNull(kernel, "kernel");
            var items = Items.Values.Where(p => p.Kernel == kernel).ToArray();
            for (int i = 0; i < items.Length; i++)
                RemoveHandle(items[i]);
        }

        /// <summary>
        /// 移除实例句柄
        /// </summary>
        /// <param name="instance"></param>
        public void UnregisterHandle(object instance)
        {
            Guard.NotNull(instance, "instance");
            InstanceHandle handle;
            if (Items.TryGetValue(instance, out handle))
                RemoveHandle(handle);
        }

        /// <summary>
        /// 获取对象句柄
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IInstanceHandle GetHandle(object instance)
        {
            Guard.NotNull(instance, "instance");
            return GetInternalHandle(instance);
        }

        private InstanceHandle GetInternalHandle(object instance)
        {
            InstanceHandle handle;
            Items.TryGetValue(instance, out handle);
            return handle;
        }

        private void RemoveHandle(InstanceHandle handle)
        {
            Items.Remove(handle);
            foreach (var item in handle.referredList.Values.ToArray())
                item.RemoveReference(handle);
            foreach (var item in handle.referenceList.Values.ToArray())
                item.UnmarkReferredBy(handle);
        }

        /// <summary>
        /// 添加对象的引用关系
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="reference"></param>
        public void RegisterReference(object owner, object reference)
        {
            Guard.NotNull(owner, "owner");
            Guard.NotNull(reference, "reference");

            var handle = GetInternalHandle(reference);
            var ownerHandle = GetInternalHandle(owner);
            if (ownerHandle != null && handle != null)
                ownerHandle.AddReference(handle);
        }

        

        /// <summary>
        /// 移除对象的引用关系
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="reference"></param>
        public void UnregisterReference(object owner, object reference)
        {
            Guard.NotNull(owner, "owner");
            Guard.NotNull(reference, "reference");

            var handle = GetInternalHandle(reference);
            if (handle != null)
            {
                var ownerHandle = GetInternalHandle(owner); ;
                if (ownerHandle != null)
                    ownerHandle.RemoveReference(handle);
            }
        }

        /// <summary>
        /// 得到指定对象的引用句柄列表
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IInstanceHandle[] GetReferenceList(object instance)
        {
            Guard.NotNull(instance, "instance");
            var handle = GetHandle(instance);
            if (handle != null)
                return handle.ReferenceList;
            return EmptyArray;
        }

        /// <summary>
        /// 得到指定对象被引用的句柄列表
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IInstanceHandle[] GetReferredList(object instance)
        {
            Guard.NotNull(instance, "instance");
            var handle = GetHandle(instance);
            if (handle != null)
                return handle.ReferredList;
            return EmptyArray;
        }

        [DebuggerDisplay("{Instance}")]
        class InstanceHandle : IInstanceHandle
        {
            public IComponentInfo Component;
            public object Instance ;
            public IKernel Kernel;
            internal Dictionary<object, InstanceHandle> referenceList = new Dictionary<object, InstanceHandle>();
            internal Dictionary<object, InstanceHandle> referredList = new Dictionary<object, InstanceHandle>();

            IComponentInfo IInstanceHandle.Component { get { return Component; } }
            object IInstanceHandle.Instance { get { return Instance; } }
            IKernel IInstanceHandle.Kernel { get { return Kernel; } }
            IInstanceHandle[] IInstanceHandle.ReferenceList
            {
                get { return referenceList.Values.ToArray(); }
            }

            IInstanceHandle[] IInstanceHandle.ReferredList
            {
                get { return referredList.Values.ToArray(); }
            }

            public InstanceHandle(IKernel kernel, IComponentInfo component, object instance)
            {
                Kernel = kernel;
                Component = component;
                Instance = instance;
            }

            public void AddReference(InstanceHandle handle)
            {
                if (referenceList.ContainsKey(handle.Instance))
                    return;
                referenceList.Add(handle.Instance, handle);
                handle.MarkReferredBy(this);
            }


            public void RemoveReference(InstanceHandle handle)
            {
                if (referenceList.ContainsKey(handle.Instance))
                {
                    referenceList.Remove(handle.Instance);
                    handle.UnmarkReferredBy(this);
                }
            }

            public void MarkReferredBy(InstanceHandle handle)
            {
                referredList.Add(handle.Instance, handle);
            }

            public void UnmarkReferredBy(InstanceHandle handle)
            {
                referredList.Remove(handle.Instance);
            }

            public void Dispose()
            {
                Component = null;
                Instance = null;
                Kernel = null;

                referenceList.Clear();
                referredList.Clear();
                referredList = null;
                referenceList = null;
            }
        }
      
    }

    [DebuggerDisplay("{Instance}")]
    class InstanceWrapper : IInstanceWrapper//,IEquatable<IInstanceHandle>
    {
        public IComponentInfo Component { get; private set; }
        public object Instance { get; private set; }
        public IKernel Kernel { get; private set; }
     
        public InstanceWrapper(IKernel kernel, IComponentInfo component, object instance)
        {
            Kernel = kernel;
            Component = component;
            Instance = instance;
        }
    }

    /// <summary>
    /// 实例包装器
    /// </summary>
    interface IInstanceWrapper
    {
        /// <summary>
        /// 组件信息
        /// </summary>
        IComponentInfo Component { get; }
        /// <summary>
        /// 实例
        /// </summary>
        object Instance { get; }
        /// <summary>
        /// 创建该实例的内核容器
        /// </summary>
        IKernel Kernel { get; }
       
    }

    static class InstanceWrapperExtensions
    {
        public static void RegisterInstanceWrapperAndReferred(this IInstanceWrapper wrapper, object owner)
        {
            var refMgr = ReferenceManager.Instance;
            refMgr.RegisterHandle(wrapper.Instance, wrapper.Kernel, wrapper.Component);
            refMgr.RegisterReference(owner, wrapper.Instance);
        }

        public static IList RegisterInstanceWrapperListAndReferred(this InstanceWrapper[] wrapperArray, Type collType, object owner)
        {

            var items = Activator.CreateInstance(typeof(List<>).MakeGenericType(TypeHelper.GetElementType(collType))) as IList;

            foreach (var w in wrapperArray)
            {
                items.Add(w.Instance);
                w.RegisterInstanceWrapperAndReferred(owner);
            }

            return items;
        }
      
    }

    interface ILazy
    {
        IKernel Kernel { get; }
        IComponentInfo Component { get; }
        event Action<object> ValueCreated;
    }
    interface ICollectonLazy
    {
        IKernel[] Kernels { get; }
        IComponentInfo[] Components { get; }
        event Action<object> ValueCreated;
    }

    static class LazyExtensions
    {
        public static void RegisterLazyAndReferred(this ILazy lazy, object owner)
        {
            Action<object> onValueCreated = null;
            onValueCreated = o =>
            {
                ReferenceManager.Instance.RegisterHandle(o, lazy.Kernel, lazy.Component);
                ReferenceManager.Instance.RegisterReference(owner, o);
                lazy.ValueCreated -= onValueCreated;
            };
            lazy.ValueCreated += onValueCreated;
        }

        

    }

    static class CollectionLazyExtensions
    {
        public static void RegisterCollectionLazyAndReferred(this ICollectonLazy collectionLazy, object owner)
        {
             Action<object> onValueCreated = null;
            onValueCreated = o =>
            {
                var values = (o as IEnumerable).Cast<object>().ToArray();
                var length = values.Length;
                ReferenceManager refMgr = ReferenceManager.Instance;
                for (int i = 0; i < length; i++)
                {
                    refMgr.RegisterHandle(values[i], collectionLazy.Kernels[i], collectionLazy.Components[i]);
                    refMgr.RegisterReference(owner, values[i]);
                }
                collectionLazy.ValueCreated -= onValueCreated;
            };
            collectionLazy.ValueCreated += onValueCreated;
        }
    }
}
