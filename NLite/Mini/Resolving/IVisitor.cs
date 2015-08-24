using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Mini.Context;
using NLite.Reflection;
using NLite.Collections;
using NLite.Mini.Listener;
using NLite.Messaging;
using NLite.Internal;


namespace NLite.Mini.Resolving
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAttributeProviderVisitor<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        TResult VisitAssembly(Assembly asm);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        TResult VisitType(Type t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctor"></param>
        /// <returns></returns>
        TResult VisitConstructor(ConstructorInfo ctor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        TResult VisitParameter(ParameterInfo p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        TResult VisitField(FieldInfo f);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        TResult VisitProperty(PropertyInfo p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        TResult VisitMethod(MethodInfo m);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class AttributeProviderVisitorRepository
    {
        private static IDictionary<RuntimeTypeHandle, object> visitors = new Dictionary<RuntimeTypeHandle, object>();

        static AttributeProviderVisitorRepository()
        {
            visitors[Types.Boolean.TypeHandle] = new DefaultIgnoreVisitor();
            visitors[typeof(SettingAttribute).TypeHandle] = new DefaultSettingAttributeVisitor();
            visitors[typeof(ComponentAttribute).TypeHandle] = new DefaultComponentAttributeVisitor();
            visitors[typeof(InjectAttribute).TypeHandle] = new DefaultInjectAttributeVisitor();

            visitors[typeof(InjectManyAttribute).TypeHandle] = new DefaultInjectManyAttributeVisitor();
            visitors[typeof(SubscribeAttribute).TypeHandle] = new DefaultSubscribeAttributeVisitor();
            visitors[typeof(InjectedNotificationAttribute).TypeHandle] = new DefaultInjectedNotificationAttributeVisitor();
        }

        /// <summary>
        /// 
        /// </summary>
        public static int Count { get { return visitors.Count; } }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IAttributeProviderVisitor<T> Get<T>()
        {
            var key = typeof(T).TypeHandle;
            object value;
            visitors.TryGetValue(key, out value);

            return value as IAttributeProviderVisitor<T>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        public static void Add<T>(IAttributeProviderVisitor<T> visitor)
        {
            Guard.NotNull(visitor, "visitor");

            lock (visitors)
            {
                visitors[typeof(T).TypeHandle] = visitor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Remove<T>()
        {
            var key = typeof(T).TypeHandle;
            lock (visitors)
            {
                return visitors.Remove(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<IAttributeProviderVisitor<object>> Items
        {
            get
            {
                return visitors.Values.Cast<IAttributeProviderVisitor<object>>().ToArray();
            }
        }

        class DefaultIgnoreVisitor : IAttributeProviderVisitor<bool>
        {
            public bool VisitAssembly(Assembly asm)
            {
                return asm.IsSystemAssembly();
            }

            public bool VisitType(Type t)
            {
                return t.IsSystemAssemblyOfType();
            }

            public bool VisitConstructor(ConstructorInfo ctor)
            {
                return ctor.HasAttribute<IgnoreAttribute>(false);
            }

            public bool VisitParameter(ParameterInfo p)
            {
                return p.HasAttribute<IgnoreAttribute>(false);
            }

            public bool VisitField(FieldInfo f)
            {
                return f.HasAttribute<IgnoreAttribute>(true)
                        || f.DeclaringType.IsSystemAssemblyOfType()
                        || f.Name.EndsWith("k__BackingField")
                        || f.IsStatic
                        || f.IsInitOnly;
            }

            public bool VisitProperty(PropertyInfo p)
            {
                return p.HasAttribute<IgnoreAttribute>(true)
                       || p.DeclaringType.IsSystemAssemblyOfType()
                       || !p.CanWrite
                       || p.GetIndexParameters().Length > 0
                       || (p.GetSetMethod() != null && p.GetSetMethod().IsStatic)
                       || (p.GetSetMethod(true) != null && p.GetSetMethod(true).IsStatic)
                       ;
            }

            public bool VisitMethod(MethodInfo m)
            {
                return m.HasAttribute<IgnoreAttribute>(true)
                    || m.DeclaringType.IsSystemAssemblyOfType()
                    || m.IsStatic
                    //|| m.ReturnType != Types.Void
                    //|| m.GetParameters().Length == 0
                    || m.IsSpecialName;
            }
        }

        class DefaultSettingAttributeVisitor : IAttributeProviderVisitor<SettingAttribute>
        {
            public SettingAttribute VisitAssembly(Assembly asm)
            {
                throw new NotImplementedException();
            }

            public SettingAttribute VisitType(Type t)
            {
                throw new NotImplementedException();
            }

            public SettingAttribute VisitConstructor(ConstructorInfo ctor)
            {
                throw new NotImplementedException();
            }

            public SettingAttribute VisitParameter(ParameterInfo p)
            {
                return p.GetAttribute<SettingAttribute>(true);
            }

            public SettingAttribute VisitField(FieldInfo f)
            {
                return f.GetAttribute<SettingAttribute>(true);
            }

            public SettingAttribute VisitProperty(PropertyInfo p)
            {
                return p.GetAttribute<SettingAttribute>(true);
            }

            public SettingAttribute VisitMethod(MethodInfo m)
            {
                throw new NotImplementedException();
            }
        }

        class DefaultComponentAttributeVisitor : IAttributeProviderVisitor<ComponentAttribute>
        {
            public ComponentAttribute VisitAssembly(Assembly asm)
            {
                throw new NotImplementedException();
            }

            public ComponentAttribute VisitType(Type t)
            {
                return t.GetAttribute<ComponentAttribute>(true);
            }

            public ComponentAttribute VisitConstructor(ConstructorInfo ctor)
            {
                throw new NotImplementedException();
            }

            public ComponentAttribute VisitParameter(ParameterInfo p)
            {
                throw new NotImplementedException();
            }

            public ComponentAttribute VisitField(FieldInfo f)
            {
                return f.GetAttribute<ComponentAttribute>(true);
            }

            public ComponentAttribute VisitProperty(PropertyInfo p)
            {
                return p.GetAttribute<ComponentAttribute>(true);
            }

            public ComponentAttribute VisitMethod(MethodInfo m)
            {
                return m.GetAttribute<ComponentAttribute>(true);
            }
        }

        class DefaultInjectAttributeVisitor : IAttributeProviderVisitor<InjectAttribute>
        {
            public InjectAttribute VisitAssembly(Assembly asm)
            {
                throw new NotImplementedException();
            }

            public InjectAttribute VisitType(Type t)
            {
                throw new NotImplementedException();
            }

            public InjectAttribute VisitConstructor(ConstructorInfo ctor)
            {
                return ctor.GetAttribute<InjectAttribute>(false);
            }

            public InjectAttribute VisitParameter(ParameterInfo p)
            {
                return p.GetAttribute<InjectAttribute>(true);
            }

            public InjectAttribute VisitField(FieldInfo f)
            {
                return f.GetAttribute<InjectAttribute>(true);
            }

            public InjectAttribute VisitProperty(PropertyInfo p)
            {
                return p.GetAttribute<InjectAttribute>(true);
            }

            public InjectAttribute VisitMethod(MethodInfo m)
            {
                return m.GetAttribute<InjectAttribute>(true);
            }
        }

        class DefaultInjectManyAttributeVisitor : IAttributeProviderVisitor<InjectManyAttribute>
        {
            public InjectManyAttribute VisitAssembly(Assembly asm)
            {
                throw new NotImplementedException();
            }

            public InjectManyAttribute VisitType(Type t)
            {
                throw new NotImplementedException();
            }

            public InjectManyAttribute VisitConstructor(ConstructorInfo ctor)
            {
                throw new NotImplementedException();
            }

            public InjectManyAttribute VisitParameter(ParameterInfo p)
            {
                return p.GetAttribute<InjectManyAttribute>(true);
            }

            public InjectManyAttribute VisitField(FieldInfo f)
            {
                return f.GetAttribute<InjectManyAttribute>(true);
            }

            public InjectManyAttribute VisitProperty(PropertyInfo p)
            {
                return p.GetAttribute<InjectManyAttribute>(true);
            }

            public InjectManyAttribute VisitMethod(MethodInfo m)
            {
                throw new NotImplementedException();
            }
        }

        class DefaultSubscribeAttributeVisitor : IAttributeProviderVisitor<SubscribeAttribute>
        {

            public SubscribeAttribute VisitAssembly(Assembly asm)
            {
                throw new NotImplementedException();
            }

            public SubscribeAttribute VisitType(Type t)
            {
                throw new NotImplementedException();
            }

            public SubscribeAttribute VisitConstructor(ConstructorInfo ctor)
            {
                throw new NotImplementedException();
            }

            public SubscribeAttribute VisitParameter(ParameterInfo p)
            {
                throw new NotImplementedException();
            }

            public SubscribeAttribute VisitField(FieldInfo f)
            {
                throw new NotImplementedException();
            }

            public SubscribeAttribute VisitProperty(PropertyInfo p)
            {
                throw new NotImplementedException();
            }

            public SubscribeAttribute VisitMethod(MethodInfo m)
            {
                return m.GetAttribute<SubscribeAttribute>(true);
            }
        }

        class DefaultInjectedNotificationAttributeVisitor : IAttributeProviderVisitor<InjectedNotificationAttribute>
        {
            public InjectedNotificationAttribute VisitAssembly(Assembly asm)
            {
                throw new NotImplementedException();
            }

            public InjectedNotificationAttribute VisitType(Type t)
            {
                throw new NotImplementedException();
            }

            public InjectedNotificationAttribute VisitConstructor(ConstructorInfo ctor)
            {
                throw new NotImplementedException();
            }

            public InjectedNotificationAttribute VisitParameter(ParameterInfo p)
            {
                throw new NotImplementedException();
            }

            public InjectedNotificationAttribute VisitField(FieldInfo f)
            {
                throw new NotImplementedException();
            }

            public InjectedNotificationAttribute VisitProperty(PropertyInfo p)
            {
                throw new NotImplementedException();
            }

            public InjectedNotificationAttribute VisitMethod(MethodInfo m)
            {
                if (m.IsGenericMethod)
                    return null;

                var args = m.GetParameters();
                if (args.Length > 1)
                    return null;

                if (args.Length == 1 && args[0].ParameterType != typeof(string[]))
                    return null;

               return m.GetAttribute<InjectedNotificationAttribute>(true);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMetadataAttributeVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<IGrouping<Type, Attribute>> VisitType(Type type);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MetadataAttributeVisitor
    {
        private static IMetadataAttributeVisitor metadataAttributeVisitor = new DefaultMetadataAttributeVisitor();

        /// <summary>
        /// 
        /// </summary>
        public static IMetadataAttributeVisitor Current
        {
            get
            {
                if (metadataAttributeVisitor == null)
                    metadataAttributeVisitor = new DefaultMetadataAttributeVisitor();
                return metadataAttributeVisitor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visitor"></param>
        public static void SetMetadataAttributeVisitor(IMetadataAttributeVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");
        }
        class DefaultMetadataAttributeVisitor : IMetadataAttributeVisitor
        {
            public IEnumerable<IGrouping<Type,Attribute>> VisitType(Type type)
            {
                return from item in type.GetCustomAttributes(false).Cast<Attribute>()
                                 let itemType = item.GetType()
                                 where itemType.HasAttribute<MetadataAttributeAttribute>(true)
                                 group item by itemType into g
                                 select g;
            }
        }
    }

    class AttributeProviderInspector
    {
        List<IMemberInjection> injections = new List<IMemberInjection>();
        List<IAppSettingInjection> appSettingInjections = new List<IAppSettingInjection>();
        List<ISubscribeInfoFactoryProvider> providers = new List<ISubscribeInfoFactoryProvider>();
        List<IExportInfo> exports = new List<IExportInfo>();

        static IAttributeProviderVisitor<SettingAttribute> settingAttributeVisitor = AttributeProviderVisitorRepository.Get<SettingAttribute>();
        static IAttributeProviderVisitor<InjectAttribute> injectAttributeVisitor = AttributeProviderVisitorRepository.Get<InjectAttribute>();
        static IAttributeProviderVisitor<InjectManyAttribute> injectManyAttributeVisitor = AttributeProviderVisitorRepository.Get<InjectManyAttribute>();
        static IAttributeProviderVisitor<ComponentAttribute> componentAttributeVisitor = AttributeProviderVisitorRepository.Get<ComponentAttribute>();
        static IAttributeProviderVisitor<SubscribeAttribute> subscribeAttributeVisitor = AttributeProviderVisitorRepository.Get<SubscribeAttribute>();
        static IAttributeProviderVisitor<InjectedNotificationAttribute> callbackAttributeVisitor = AttributeProviderVisitorRepository.Get<InjectedNotificationAttribute>();

        static readonly Dictionary<int, Type> Actions;
        static readonly Dictionary<int, Type> Funcs;
        static int MaxParameterLength;

        static AttributeProviderInspector()
        {
#if SDK4
                MaxParameterLength = 8;
#else
            MaxParameterLength = 4;
#endif
            Actions = new Dictionary<int, Type>(MaxParameterLength + 1);
            Funcs = new Dictionary<int, Type>(MaxParameterLength + 1);

            Actions[0] = typeof(Action);
            Actions[1] = typeof(Action<>);
            Actions[2] = typeof(Action<,>);
            Actions[3] = typeof(Action<,,>);
            Actions[4] = typeof(Action<,,,>);

            Funcs[0] = typeof(Func<>);
            Funcs[1] = typeof(Func<,>);
            Funcs[2] = typeof(Func<,,>);
            Funcs[3] = typeof(Func<,,,>);
            Funcs[4] = typeof(Func<,,,,>);

#if SDK4
                Actions[5] = typeof(Action<,,,,,>);
                Actions[6] = typeof(Action<,,,,,,>);
                Actions[7] = typeof(Action<,,,,,,,>);
                Actions[8] = typeof(Action<,,,,,,,,>);

                Funcs[5] = typeof(Func<,,,,,>);
                Funcs[6] = typeof(Func<,,,,,,>);
                Funcs[7] = typeof(Func<,,,,,,,>);
                Funcs[8] = typeof(Func<,,,,,,,,>);
#endif
        }

        private static IMemberInjection CreateFieldInjection(IKernel kernel, FieldInfo f, InjectAttribute att)
        {
            return new FieldInjection//字段注入元数据
            {
                Member = f,
                Reinjection = att != null ? att.Reinjection : true,
                Setter = f.ToMemberSetter(),//通过Emit的方式进行注入,
                Dependency = DependencyManager.Get(att != null ? att.Id : string.Empty, f.FieldType, kernel, false)
            };
        }

        private static IMemberInjection CreateFieldInjection(IKernel kernel, FieldInfo f, InjectManyAttribute manyAttr)
        {
            return new FieldInjection//字段注入元数据
            {
                Member = f,
                Reinjection = manyAttr != null ? manyAttr.Reinjection : true,
                Setter = f.ToMemberSetter(),//通过Emit的方式进行注入,
                Dependency = DependencyManager.Get(null, f.FieldType, kernel, true)
            };
        }

        private static IAppSettingInjection CreateFieldInjection(FieldInfo f, SettingAttribute settingAttr)
        {
            return new AppSettingFieldInjection
            {
                Member = f,
                Setter = f.ToMemberSetter(),//通过Emit的方式进行注入
                Dependency = DependencyManager.GetAppSettingDependency(settingAttr.Name, f.FieldType),
            };
        }

        private static IMemberInjection CreatePropertyInjection(IKernel kernel, PropertyInfo p, InjectAttribute att)
        {
            return new PropertyInjection
            {
                Member = p,
                Reinjection = att != null ? att.Reinjection : true,
                Setter = p.ToMemberSetter(),
                Dependency = DependencyManager.Get(att != null ? att.Id : string.Empty, p.PropertyType, kernel, false)
            };
        }

        private static IMemberInjection CreatePropertyInjection(IKernel kernel, PropertyInfo p, InjectManyAttribute manyAttr)
        {
            return new PropertyInjection//字段注入元数据
            {
                Member = p,
                Reinjection = manyAttr != null ? manyAttr.Reinjection : true,
                Setter = p.ToMemberSetter(),//通过Emit的方式进行注入,
                Dependency = DependencyManager.Get(null, p.PropertyType, kernel, true)
            };
        }

        private static IAppSettingInjection CreatePropertyInjection(PropertyInfo p, SettingAttribute settingAttr)
        {
            return new AppSettingPropertyInjection
            {
                Member = p,
                Setter = p.ToMemberSetter(),//通过Emit的方式进行注入
                Dependency = DependencyManager.GetAppSettingDependency(settingAttr.Name, p.PropertyType),
            };
        }

        private static IMemberInjection CreateMethodInjection(IComponentInfo ctx, IKernel kernel, MethodInfo m, InjectAttribute att)
        {
            var ps = m.GetParameters();
            List<IDependency> dependencyList = new List<IDependency>(ps.Length);
            foreach (var p in ps)
            {
                if (p.ParameterType.IsByRef || p.IsRetval || p.IsOut)
                    return null;
                dependencyList.Add(AttributeProviderInspector.InspectParameter(ctx, kernel, p));
            }

            var id = att != null ? att.Id : string.Empty;
            var injection = new MethodInjection(dependencyList.ToArray())//方法注入元数据
            {
                Member = m,
                Method = DynamicMethodFactory.GetProc(m),
                Reinjection = att.Reinjection,
            };

            return injection;
        }

        private static ISubscribeInfoFactoryProvider CreateSubscribeInfoFactoryProvider(MethodInfo m, SubscribeAttribute attr)
        {
            var ps = m.GetParameters();
            var length = ps.Length;
            if (length > 2)
                return null;
            var returnType = m.ReturnType;
            if (returnType == Types.Void)
            {
                switch (length)
                {
                    case 0:
                        if(m.IsStatic)
                            return new StaticActionSubscribeProvider(m, attr.Topic, attr.Mode);
                        return new ActionSubscribeProvider(m, attr.Topic, attr.Mode);
                    case 1:
                        if (m.IsStatic) return new StaticAction1SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                        return new Action1SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                    case 2:
                        if( m.IsStatic) return  new StaticAction2SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                        return new Action2SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                }
            }
            else
            {
                switch (length)
                {
                    case 0:
                        if(m.IsStatic)
                            return new StaticFuncSubscribeProvider(m, attr.Topic, attr.Mode);
                        return new FuncSubscribeProvider(m, attr.Topic, attr.Mode);
                    case 1:
                        if (m.IsStatic)
                            return new StaticFunc1SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                        return new Func1SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                    case 2:
                        if (m.IsStatic)
                            return new StaticFunc2SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                        return new Func2SubscribeProvider(m, ps, attr.Topic, attr.Mode);
                }
            }

            return null;
        }

        private static InjectAttribute GetInjectAttribute(FieldInfo f)
        {
            //return f.GetAttribute<InjectAttribute>(true);
            return injectAttributeVisitor.VisitField(f);
        }

        private static InjectManyAttribute GetInjectManyAttribute(FieldInfo f)
        {
            //return f.GetAttribute<InjectManyAttribute>(true);
            return injectManyAttributeVisitor.VisitField(f);
        }

        private static SettingAttribute GetSettingAttribute(FieldInfo f)
        {
            //return f.GetAttribute<SettingAttribute>(false);
            return settingAttributeVisitor.VisitField(f);
        }

        private static InjectAttribute GetInjectAttribute(PropertyInfo f)
        {
            //return f.GetAttribute<InjectAttribute>(true);
            return injectAttributeVisitor.VisitProperty(f);
        }

        private static InjectManyAttribute GetInjectManyAttribute(PropertyInfo f)
        {
            //return f.GetAttribute<InjectManyAttribute>(true);
            return injectManyAttributeVisitor.VisitProperty(f);
        }

        private static SettingAttribute GetSettingAttribute(PropertyInfo f)
        {
            //return f.GetAttribute<SettingAttribute>(false);
            return settingAttributeVisitor.VisitProperty(f);
        }

        private static SubscribeAttribute GetSubscribeAttribute(MethodInfo m)
        {
            //return m.GetAttribute<SubscribeAttribute>(true);
            return subscribeAttributeVisitor.VisitMethod(m);
        }

        private static InjectAttribute GetInjectAttribute(MethodInfo m)
        {
            return injectAttributeVisitor.VisitMethod(m);
            //return m.GetAttribute<InjectAttribute>(false);
        }

        private static void Check(Type contract)
        {
            if (!contract.IsSubclassOf(Types.Delegate))
                throw new MemberExportException(Mini_Resources.ContractTypeShouldBeDelegateType);//契约必须是委托类型
        }

        private static Type PopulateMethodContract(MethodInfo m)
        {
            var args = m.GetParameters();
            if (args.Length > MaxParameterLength)
                throw  new MemberExportException(Mini_Resources.MethodParametersToolMany);
            if (args.Any(p => p.IsOut || p.ParameterType.IsByRef || p.IsRetval))
                throw new MemberExportException(Mini_Resources.MethodNotAllowRefOrOutParameters);

            if (m.ReturnType == Types.Void)
                return Actions[args.Length]
                    .MakeGenericType(args.GetParameterTypes());

            var genericArgsType = args.Select(p => p.ParameterType).ToList();
            genericArgsType.Add(m.ReturnType);

            return Funcs[args.Length]
                .MakeGenericType(genericArgsType.ToArray());
        }

        public IMemberInjection[] Injections
        {
            get { return injections.ToArray(); }
        }

        public IAppSettingInjection[] AppSettingInjections
        {
            get { return appSettingInjections.ToArray(); }
        }

        public ISubscribeInfoFactoryProvider[] SubscribeInfoFactoryProviders
        {
            get { return providers.ToArray(); }
        }

        public IExportInfo[] Exports
        {
            get { return exports.ToArray(); }
        }

        public void InspectField(IComponentInfo ctx, IKernel kernel, FieldInfo f)
        {
            var settingAttr = GetSettingAttribute(f);
            if (settingAttr != null && !string.IsNullOrEmpty(settingAttr.Name))
            {
                appSettingInjections.Add(CreateFieldInjection(f, settingAttr));
                return;
            }

            var manyAttr = GetInjectManyAttribute(f);
            if (manyAttr != null)
            {
                injections.Add(CreateFieldInjection(kernel, f, manyAttr));
                return;
            }

            var injectAttr = GetInjectAttribute(f);
            if (injectAttr != null)
            {
                injections.Add(CreateFieldInjection(kernel, f, injectAttr));
                return;
            }

            //var componentAttr =  f.GetAttribute<ComponentAttribute>(true);
            var componentAttr = componentAttributeVisitor.VisitField(f);
            if (componentAttr != null)
            {
                exports.Add(new FieldExportInfo
                {
                    Id = componentAttr != null ? componentAttr.Id : null,
                    Field = f,
                    Getter = DynamicMethodFactory.GetGetter(f)
                });
                return;
            }
        }

        public void InspectProperty(IComponentInfo ctx, IKernel kernel, PropertyInfo p)
        {
            var settingAttr = GetSettingAttribute(p);
            if (settingAttr != null && !string.IsNullOrEmpty(settingAttr.Name))
            {
                appSettingInjections.Add(CreatePropertyInjection(p, settingAttr));
                return;
            }

            var manyAttr = GetInjectManyAttribute(p);
            if (manyAttr != null)
            {
                injections.Add(CreatePropertyInjection(kernel, p, manyAttr));
                return;
            }

            var injectAttr = GetInjectAttribute(p);
            if (injectAttr != null)
            {
                injections.Add(CreatePropertyInjection(kernel, p, injectAttr));
                return;
            }

            //var componentAttr = p.GetAttribute<ComponentAttribute>(true);
            var componentAttr = componentAttributeVisitor.VisitProperty(p);
            if (componentAttr != null)
            {
                exports.Add(new PropertyExportInfo
                {
                    Id = componentAttr != null ? componentAttr.Id : null,
                    Property = p,
                    Getter = DynamicMethodFactory.GetGetter(p)
                });
                return;
            }
        }

        internal static ISubscribeInfoFactoryProvider[] InspectSubscribeMethods(Type subscreberType)
        {
            return subscreberType.GetMethods(MemberInspector.Flags | BindingFlags.Static)
                .Where(m => !m.DeclaringType.IsSystemAssemblyOfType())
                .Where(m => !m.IsSpecialName)
                .Select(m=>new { Attribute = GetSubscribeAttribute(m),Method = m})
                .Where (p =>p.Attribute != null)
                .Select(p=> CreateSubscribeInfoFactoryProvider(p.Method,p.Attribute))
                .ToArray()
                ;
        }

        public void InspectMethod(IComponentInfo ctx, IKernel kernel, MethodInfo m)
        {
            //var componentAttr = m.GetAttribute<ComponentAttribute>(true);
            var componentAttr = componentAttributeVisitor.VisitMethod(m);
            if (componentAttr != null)
            {
                Type contract = componentAttr.Contract;

                if (contract != null)
                    Check(contract);
                else
                    contract = PopulateMethodContract(m);

                exports.Add(new MethodExportInfo
                {
                    Id = componentAttr != null ? componentAttr.Id : Guid.NewGuid().ToString(),
                    Contract = contract,
                    Method = m
                });
                return;
            }

            if (m.ReturnType != Types.Void)
                return;

            var injectAttr = GetInjectAttribute(m);
            if (injectAttr != null)
            {
                var injection = CreateMethodInjection(ctx, kernel, m, injectAttr);
                if (injection != null)
                {
                    injections.Add(injection);
                }
                return;
            }

            var subscribeAttr = GetSubscribeAttribute(m);
            if (subscribeAttr != null)
            {
                var item = CreateSubscribeInfoFactoryProvider(m, subscribeAttr);
                if (item != null)
                    providers.Add(item);
                return;
            }

            var callbackAttr = callbackAttributeVisitor.VisitMethod(m);
            if (callbackAttr != null)
            {

                var parameterCount = m.GetParameters().Length;
                var callback =parameterCount == 1 ?
                    Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(m.DeclaringType,typeof(string[])), m)
                    : Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(m.DeclaringType),m);

                ctx.ExtendedProperties["ReinjectedNotification"] = new KeyValuePair<int,Delegate>(parameterCount,callback);
            }
        }

        public static IDependency InspectParameter(IComponentInfo ctx, IKernel kernel, ParameterInfo p)
        {
            var settingAttr = p.GetAttribute<SettingAttribute>(false);
            if (settingAttr != null && !string.IsNullOrEmpty(settingAttr.Name))
                return DependencyManager.GetAppSettingDependency(settingAttr.Name, p.ParameterType);

            var patt = p.GetAttribute<InjectAttribute>(false);
            var dependencyCreationCtx = new DependencyCreationContext
            {
                Id = patt != null ? patt.Id : string.Empty,
                DependencyType = p.ParameterType,
                Kernel = kernel,
                InjectMany = p.HasAttribute<InjectManyAttribute>(false)
                                || (p.ParameterType.IsCollectionTypeExcludeStringAndDictionaryType()
                                     && !TypeHelper.GetElementType(p.ParameterType).IsSystemAssemblyOfType()/*kernel.HasRegister(TypeHelper.GetElementType(p.ParameterType))*/),
                IsOptional = p.IsOptional,
                DefaultValue = p.DefaultValue,
            };
            return DependencyManager.Get(dependencyCreationCtx);
        }

        public static ConstructorInjection InspectConstructor(IComponentInfo ctx, IKernel kernel, ConstructorInfo ctor)
        {
            var item = new ConstructorInjection(ctor.GetParameters().Select(p => AttributeProviderInspector.InspectParameter(ctx, kernel, p)).ToArray())
            {
                Member = ctor,
                Creator = ctor.GetCreator(),
                IsMarkedInjection = ctor.HasAttribute<InjectAttribute>(false),
            };
            return item;
        }
    }

    class MemberInspector
    {
        internal const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags FieldFlags = BindingFlags.SetField | Flags;
        private const BindingFlags PropertyFlags = BindingFlags.SetProperty | Flags;

        AttributeProviderInspector injectionInspector;

        private IAttributeProviderVisitor<bool> ignoreVisitor;

        public void Inspect(IComponentInfo info, IKernel kernel, Type componentType, string injectionKey)
        {
            injectionInspector = new AttributeProviderInspector();
            ignoreVisitor = AttributeProviderVisitorRepository.Get<bool>();

            Guard.NotNull(ignoreVisitor, "ignoreVisitor");

            if(!ignoreVisitor.VisitType(componentType))//if (!componentType.IsSystemAssemblyOfType())
            {
                InspectFields(info, kernel, componentType);
                InspectPropeties(info, kernel, componentType);
                InspectMethods(info, kernel, componentType);
            }

            info.ExtendedProperties[injectionKey] = injectionInspector.Injections;
            info.ExtendedProperties["MembersRegistered"] = injectionInspector.Exports;
            info.ExtendedProperties["AppSettings"] = injectionInspector.AppSettingInjections;
            info.ExtendedProperties["subscribeProviders"] = injectionInspector.SubscribeInfoFactoryProviders;

            injectionInspector = null;
        }

        private void InspectFields(IComponentInfo ctx, IKernel kernel, Type componentType)
        {
            componentType.GetFields(FieldFlags)
                .Where(p => !ignoreVisitor.VisitField(p))
                .ForEach(p => injectionInspector.InspectField(ctx, kernel, p));
        }

        private void InspectPropeties(IComponentInfo ctx, IKernel kernel, Type componentType)
        {
            componentType.GetProperties(PropertyFlags)
                .Where(p => !ignoreVisitor.VisitProperty(p))
                .ForEach(p => injectionInspector.InspectProperty(ctx, kernel, p));
        }

        private void InspectMethods(IComponentInfo ctx, IKernel kernel, Type componentType)
        {
            componentType.GetMethods(Flags)
                .Where(p => !ignoreVisitor.VisitMethod(p))
                .ForEach(p => injectionInspector.InspectMethod(ctx, kernel, p));
        }
    }
}
