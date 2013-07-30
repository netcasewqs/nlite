using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLite.Collections;
using NLite.Mini.Context;
using NLite.Reflection;
using NLite.Mapping.Internal;

namespace NLite.Mini.Resolving
{
    /// <summary>
    /// 注入接口
    /// </summary>
    interface IMemberInjection
    {
        /// <summary>
        /// 被注入的Member,如Field,Property,Method,Constructor
        /// </summary>
        MemberInfo Member { get; }
        /// <summary>
        /// 是否支持重新注入
        /// </summary>
        bool Reinjection { get;}
        
        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="ctx"></param>
        void Inject(IComponentContext ctx);
        /// <summary>
        /// 判断当前被注入的Member的Type是否是指定类型的基类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool IsAssignableFrom(Type t);
    }

    /// <summary>
    /// AppSetting 注入接口
    /// </summary>
    interface IAppSettingInjection
    {
        /// <summary>
        /// 被注入的Member,如Field,Property,Method,Constructor
        /// </summary>
        MemberInfo Member { get; }
        /// <summary>
        /// 是否支持重新注入
        /// </summary>
        bool Reinjection { get; }

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="instance"></param>
        void Inject(object instance);
      
    }

    /// <summary>
    /// 注入基类
    /// </summary>
    abstract class InjectionBase : IMemberInjection
    {
        /// <summary>
        /// 是否支持重新注入
        /// </summary>
        public bool Reinjection { get; set; }
        /// <summary>
        /// 被注入的Member,如Field,Property,Method,Constructor
        /// </summary>
        public MemberInfo Member { get; set; }
        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="ctx"></param>
        public abstract void Inject(IComponentContext ctx);
        /// <summary>
        /// 判断当前被注入的Member的Type是否是指定类型的基类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool IsAssignableFrom(Type t) { return false; }
    }

   
    /// <summary>
    /// Field 注入
    /// </summary>
    class FieldInjection : InjectionBase
    {
        /// <summary>
        /// Field 设置器
        /// </summary>
        public Setter Setter;
        /// <summary>
        /// Field 所依赖的元数据
        /// </summary>
        public IDependency Dependency;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void Inject(IComponentContext ctx)
        {
            if (!Dependency.HasDependencied)
                return;

            var instance = ctx.Instance;
            var value = Dependency.ValueProvider();
            if (!ReferenceManager.Instance.Enabled)
            {
                Setter(instance, value);
                return;
            }

            var wrapper = value as IInstanceWrapper;
            if (wrapper != null)
            {
                Setter(instance, wrapper.Instance);
                wrapper.RegisterInstanceWrapperAndReferred(instance);
                return;
            }

            var lazy = value as ILazy;
            if (lazy != null)
            {
                lazy.RegisterLazyAndReferred(instance);
                Setter(instance, value);
                return;
            }

            var collectionLazy = value as ICollectonLazy;
            if (collectionLazy != null)
            {
                collectionLazy.RegisterCollectionLazyAndReferred(instance);
                Setter(instance, value);
                return;
            }

            var wrapperArray = value as InstanceWrapper[];
            if (wrapperArray != null)
            {
                var collType = Dependency.DependencyType;
                var items = wrapperArray.RegisterInstanceWrapperListAndReferred(collType, instance);
                Setter(instance, Mapper.Map(items, items.GetType(), collType));
                return;
            }

            Setter(instance, value);
        }

        /// <summary>
        /// 判断当前被注入的Member的Type是否是指定类型的基类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool IsAssignableFrom(Type t)
        {
            return Dependency.IsAssignableFrom(t);
        }
    }

    class PropertyInjection : InjectionBase
    {
        public Setter Setter;
        public IDependency Dependency;

        public override void Inject(IComponentContext ctx)
        {
            if (!Dependency.HasDependencied)
                return;

            var instance = ctx.Instance;
            var value = Dependency.ValueProvider();
            if (!ReferenceManager.Instance.Enabled)
            {
                Setter(instance, value);
                return;
            }

            var wrapper = value as IInstanceWrapper;
            if (wrapper != null)
            {
                Setter(instance, wrapper.Instance);
                wrapper.RegisterInstanceWrapperAndReferred(instance);
                return;
            }

            var lazy = value as ILazy;
            if (lazy != null)
            {
                lazy.RegisterLazyAndReferred(instance);
                Setter(instance, value);
                return;
            }

            var collectionLazy = value as ICollectonLazy;
            if (collectionLazy != null)
            {
                collectionLazy.RegisterCollectionLazyAndReferred(instance);
                Setter(instance, value);
                return;
            }

            var wrapperArray = value as InstanceWrapper[];
            if (wrapperArray != null)
            {
                var collType = Dependency.DependencyType;
                var items = wrapperArray.RegisterInstanceWrapperListAndReferred(collType, instance);
                Setter(instance, Mapper.Map(items, items.GetType(), collType));
                return;
            }

            Setter(instance, value);
        }

        /// <summary>
        /// 判断当前被注入的Member的Type是否是指定类型的基类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool IsAssignableFrom(Type t)
        {
             return Dependency.IsAssignableFrom(t);
        }
    }

    class MethodInjection : InjectionBase
    {
        internal Proc Method;
        internal IDependency[] Dependencies;
        public bool IsMatch = true;

        public MethodInjection(IDependency[] dependencies)
        {

            Dependencies = dependencies;
            if (dependencies != null && dependencies.Length > 0)
            {
                foreach (var item in dependencies)
                {
                    item.OnRefresh += OnFresh;
                    if (!item.HasDependencied)
                        IsMatch = false;
                }
            }
        }

        void OnFresh()
        {
            IsMatch = Dependencies.TrueForAll(p => p.HasDependencied);
        }

        public override void Inject(IComponentContext ctx)
        {
            if (Dependencies.TrueForAll(p => p.HasDependencied))
            {
                var instance = ctx.Instance;
                var tmpArgs = ReferenceManager.Instance.Enabled?
                    Dependencies.Select(p => PopulateArgument(instance, p.ValueProvider(),p.DependencyType)).ToArray()
                    : Dependencies.Select(p =>p.ValueProvider()).ToArray();
                Method(ctx.Instance, tmpArgs);

            }
            foreach (var item in Dependencies.OfType<AppSettingDependency>())
            {
                if (item.Reinjection)
                {
                    Reinjection = true;
                    break;
                }
            }
        }

        private static object PopulateArgument(object owner, object arg,Type needArgType)
        {
            var wrapper = arg as IInstanceWrapper;
            if (wrapper != null)
            {
                wrapper.RegisterInstanceWrapperAndReferred(owner);
                return wrapper.Instance;
            }

            var lazy = arg as ILazy;
            if (lazy != null)
            {
                lazy.RegisterLazyAndReferred(owner);
                return arg;
            }

            var collectionLazy = arg as ICollectonLazy;
            if (collectionLazy != null)
            {
                collectionLazy.RegisterCollectionLazyAndReferred(owner);
                return arg;
            }

            var wrapperArray = arg as InstanceWrapper[];
            if (wrapperArray != null)
            {
                var collType = needArgType;
                var items = wrapperArray.RegisterInstanceWrapperListAndReferred(collType, owner);
                return Mapper.Map(items, items.GetType(), collType);
            }

            return arg;
        }

        /// <summary>
        /// 判断当前被注入的Member的Type是否是指定类型的基类
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override bool IsAssignableFrom(Type t)
        {
            return Dependencies.Any(p => p.IsAssignableFrom(t));
        }

        
    }

    /// <summary>
    /// AppSetting注入基类
    /// </summary>
    abstract class AppSettingInjectionBase : IAppSettingInjection
    {
        /// <summary>
        /// 是否支持重新注入
        /// </summary>
        public bool Reinjection { get; set; }
        /// <summary>
        /// 被注入的Member,如Field,Property,Method,Constructor
        /// </summary>
        public MemberInfo Member { get; set; }
        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="instance"></param>
        public abstract void Inject(object  instance);
       
    }
    class AppSettingFieldInjection : AppSettingInjectionBase
    {
        public Setter Setter;
        public AppSettingDependency Dependency;

        public override void Inject(object instance)
        {
            Setter(instance, Dependency.ValueProvider());
            Reinjection = Dependency.Reinjection;
        }
    }

    class AppSettingPropertyInjection : AppSettingInjectionBase
    {
        internal Setter Setter;
        internal AppSettingDependency Dependency;

        public override void Inject(object instance)
        {
            Setter(instance, Dependency.ValueProvider());
            Reinjection = Dependency.Reinjection;
        }
    }

   
    class ConstructorInjection
    {
       
        public ConstructorInfo Member { get; set; }
        public ConstructorHandler Creator;
        public IDependency[] Dependencies;
        public bool IsMarkedInjection;
        public bool IsMatch = true;
        ReferenceManager refMgr;
        public ConstructorInjection(IDependency[] dependencies)
        {

            Dependencies = dependencies;
            refMgr = ReferenceManager.Instance;
            if (dependencies != null && dependencies.Length > 0)
            {
                foreach (var item in dependencies)
                {
                    item.OnRefresh += OnFresh;
                    if (!item.HasDependencied)
                        IsMatch = false;
                }
            }
        }

        public void Inject(IComponentContext ctx)
        {
            if (IsMatch)
            {
                if (refMgr.Enabled)
                {
                    List<object> references = new List<object>();
                    var args = Dependencies.Select(p => PopulateArgument(references, p)).ToArray();
                    
                    ctx.Instance = Creator(args);

                    refMgr.RegisterHandle(ctx.Instance, ctx.Kernel, ctx.Component);

                    foreach (var @ref in references)
                        refMgr.RegisterReference(ctx.Instance, @ref);
                }
                else
                    ctx.Instance = Creator(Dependencies.Select(p => p.ValueProvider()).ToArray());
            }
        }

        private object PopulateArgument(List<object> references, IDependency p)
        {
            var value = p.ValueProvider();
            var wrapper = value as IInstanceWrapper;
            if (wrapper != null)
            {
                refMgr.RegisterHandle(wrapper.Instance, wrapper.Kernel, wrapper.Component);
                references.Add(wrapper);
                return wrapper.Instance;
            }

            var lazy = value as ILazy;
            if (lazy != null)
            {
                PopulateLazyValue(references, lazy);
                return value;
            }

            var collectionLazy = value as ICollectonLazy;
            if (collectionLazy != null)
            {
                PopulateCollectionLazyValue(references, collectionLazy);
                return value;
            }

            var wrapperArray = value as InstanceWrapper[];
            if (wrapperArray != null)
                return PopulateWrapperArray(p, wrapperArray);

            return value;
        }

        private object PopulateWrapperArray(IDependency p, InstanceWrapper[] wrapperArray)
        {
            var collType = p.DependencyType;
            var items = Activator.CreateInstance(typeof(List<>).MakeGenericType(TypeHelper.GetElementType(collType))) as IList;

            foreach (var w in wrapperArray)
            {
                items.Add(w.Instance);
                refMgr.RegisterHandle(w.Instance, w.Kernel, w.Component);
            }
            return Mapper.Map(items, items.GetType(), collType);
        }

        private void PopulateCollectionLazyValue(List<object> references, ICollectonLazy collectionLazy)
        {
            Action<object> onValueCreated = null;
            onValueCreated = o =>
            {
                var values = (o as IEnumerable).Cast<object>().ToArray();
                var length = values.Length;
                for (int i = 0; i < length; i++)
                {
                    refMgr.RegisterHandle(values[i], collectionLazy.Kernels[i], collectionLazy.Components[i]);
                    references.Add(values[i]);
                }
                collectionLazy.ValueCreated -= onValueCreated;
            };
            collectionLazy.ValueCreated += onValueCreated;
        }

        private void PopulateLazyValue(List<object> references, ILazy lazy)
        {
            Action<object> onValueCreated = null;
            onValueCreated = a =>
            {
                refMgr.RegisterHandle(a, lazy.Kernel, lazy.Component);
                references.Add(a);
                lazy.ValueCreated -= onValueCreated;
            };
            lazy.ValueCreated += onValueCreated;
        }

        void OnFresh()
        {
            IsMatch = Dependencies.TrueForAll(p => p.HasDependencied);
        }
    }
}
