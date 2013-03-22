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


namespace NLite.Mini.Resolving
{
    interface IMemberInspector
    {
        void InspectField(IComponentInfo ctx, IKernel kernel, FieldInfo f);
        void InspectProperty(IComponentInfo ctx, IKernel kernel, PropertyInfo p);
        void InspectMethod(IComponentInfo ctx, IKernel kernel, MethodInfo m);
    }

    class ParameterInspector
    {
        public static IDependency Inspect(IComponentInfo ctx, IKernel kernel, ParameterInfo p)
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
                                     && kernel.HasRegister(TypeHelper.GetElementType(p.ParameterType))),
                IsOptional = p.IsOptional,
                DefaultValue = p.DefaultValue,
            };
            return DependencyManager.Get(dependencyCreationCtx);
        }
      
    }

    class ConstructorInspector 
    {
        public static ConstructorInjection Inspect(IComponentInfo ctx, IKernel kernel, ConstructorInfo ctor)
        {
            var item = new ConstructorInjection(ctor.GetParameters().Select(p => ParameterInspector.Inspect(ctx, kernel, p)).ToArray())
            {
                Member = ctor,
                Creator = ctor.GetCreator(),
                IsMarkedInjection = ctor.HasAttribute<InjectAttribute>(false),
            };
            return item;
        }

    }

    class InjectionInspector :  IMemberInspector
    {
        List<IInjection> injections = new List<IInjection>();
        List<IAppSettingInjection> appSettingInjections = new List<IAppSettingInjection>();

        public IInjection[] Injections
        {
            get { return injections.ToArray(); }
        }

        public IAppSettingInjection[] AppSettingInjections
        {
            get { return appSettingInjections.ToArray(); }
        }

        public void InspectField(IComponentInfo ctx, IKernel kernel, FieldInfo f)
        {
            var settingAttr = f.GetAttribute<SettingAttribute>(false);
            if (settingAttr != null && !string.IsNullOrEmpty(settingAttr.Name))
            {
                appSettingInjections.Add(new AppSettingFieldInjection
                {
                    Member = f,
                    Setter = f.ToMemberSetter(),//通过Emit的方式进行注入
                    Dependency = DependencyManager.GetAppSettingDependency(settingAttr.Name, f.FieldType),
                });
                return;
            }

            var manyAttr = f.GetAttribute<InjectManyAttribute>(true);
            if (manyAttr != null)
            {
                injections.Add(new FieldInjection//字段注入元数据
                {
                    Member = f,
                    Reinjection = manyAttr != null ? manyAttr.Reinjection : true,
                    Setter = f.ToMemberSetter(),//通过Emit的方式进行注入,
                    Dependency = DependencyManager.Get(null, f.FieldType, kernel, true)
                });
                return;
            }

            var att = f.GetAttribute<InjectAttribute>(true);
           if (att != null)
            {
                injections.Add(new FieldInjection//字段注入元数据
                {
                    Member = f,
                    Reinjection = att != null ? att.Reinjection : true,
                    Setter = f.ToMemberSetter(),//通过Emit的方式进行注入,
                    Dependency = DependencyManager.Get(att != null ? att.Id : string.Empty, f.FieldType, kernel, false)
                });
                return;
            }
        }

        public void InspectProperty(IComponentInfo ctx, IKernel kernel, PropertyInfo p)
        {
            var settingAttr = p.GetAttribute<SettingAttribute>(false);
            if (settingAttr != null && !string.IsNullOrEmpty(settingAttr.Name))
            {
                appSettingInjections.Add(new AppSettingPropertyInjection
                {
                    Member = p,
                    Setter = p.ToMemberSetter(),//通过Emit的方式进行注入
                    Dependency = DependencyManager.GetAppSettingDependency(settingAttr.Name, p.PropertyType),
                });
                return;
            }

            var manyAttr = p.GetAttribute<InjectManyAttribute>(true);
            if (manyAttr != null)
            {
                injections.Add(new FieldInjection//字段注入元数据
                {
                    Member = p,
                    Reinjection = manyAttr != null ? manyAttr.Reinjection : true,
                    Setter = p.ToMemberSetter(),//通过Emit的方式进行注入,
                    Dependency = DependencyManager.Get(null, p.PropertyType, kernel, true)
                });

                return;
            }

            var att = p.GetAttribute<InjectAttribute>(true);
            if (att != null)
            {
                injections.Add(new PropertyInjection
                {
                    Member = p,
                    Reinjection = att != null ? att.Reinjection : true,
                    Setter = p.ToMemberSetter(),
                    Dependency = DependencyManager.Get(att != null ? att.Id : string.Empty, p.PropertyType, kernel, false)
                });
                return;
            }

        }

        public void InspectMethod(IComponentInfo ctx, IKernel kernel, MethodInfo m)
        {
            var att = m.GetAttribute<InjectAttribute>(false);
            if (att == null)
                return;

            var ps = m.GetParameters();
            List<IDependency> dependencyList = new List<IDependency>(ps.Length);
            foreach (var p in ps)
            {
                if (p.ParameterType.IsByRef || p.IsRetval || p.IsOut)
                    return;
                dependencyList.Add(ParameterInspector.Inspect(ctx, kernel, p));
            }

            var id = att != null ? att.Id : string.Empty;

            injections.Add(new MethodInjection(dependencyList.ToArray())//方法注入元数据
            {
                Member = m,
                Method = DynamicMethodFactory.GetProc(m),
            });
        }
    }

    class MemberExportInspector : IMemberInspector
    {
        static readonly Dictionary<int, Type> Actions;
        static readonly Dictionary<int, Type> Funcs;
        static int MaxParameterLength;

        static MemberExportInspector()
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
        List<IExportInfo> items = new List<IExportInfo>();

        public IExportInfo[] Exports
        {
            get { return items.ToArray(); }
        }

        public void InspectField(IComponentInfo ctx, IKernel kernel, FieldInfo f)
        {
            var attr = f.GetAttribute<ComponentAttribute>(true);
            if (attr == null)
                return;

            items.Add( new FieldExportInfo
            {
                Id = attr != null ? attr.Id : null,
                Field = f,
                Getter = DynamicMethodFactory.GetGetter(f)
            });
        }

        public void InspectProperty(IComponentInfo ctx, IKernel kernel, PropertyInfo p)
        {
            var attr = p.GetAttribute<ComponentAttribute>(true);
            if (attr == null)
                return;

            items.Add(new PropertyExportInfo
            {
               Id = attr != null ? attr.Id : null,
               Property = p,
               Getter = DynamicMethodFactory.GetGetter(p)
            });
        }

        public void InspectMethod(IComponentInfo ctx, IKernel kernel, MethodInfo m)
        {
            var attr = m.GetAttribute<ComponentAttribute>(true);
            if (attr == null)
                return;

            Type contract = attr.Contract;

            if (contract != null)
                Check(contract);
            else
                contract = PopulateMethodContract(m);

            items.Add( new MethodExportInfo
            {
                Id = attr != null ? attr.Id : Guid.NewGuid().ToString(),
                Contract = contract,
                Method = m
            });
        }

        private static void Check(Type contract)
        {
            if (!contract.IsSubclassOf(Types.Delegate))
                throw ExceptionManager.HandleAndWrapper<MemberExportException>(Mini_Resources.ContractTypeShouldBeDelegateType);//契约必须是委托类型
        }

        private static Type PopulateMethodContract(MethodInfo m)
        {
            var args = m.GetParameters();
            if (args.Length > MaxParameterLength)
                throw ExceptionManager.HandleAndWrapper<MemberExportException>(Mini_Resources.MethodParametersToolMany);
            if (args.Any(p => p.IsOut || p.ParameterType.IsByRef || p.IsRetval))
                throw ExceptionManager.HandleAndWrapper<MemberExportException>(Mini_Resources.MethodNotAllowRefOrOutParameters);

            if (m.ReturnType == Types.Void)
                return Actions[args.Length]
                    .MakeGenericType(args.GetParameterTypes());

            var genericArgsType = args.Select(p => p.ParameterType).ToList();
            genericArgsType.Add(m.ReturnType);

            return Funcs[args.Length]
                .MakeGenericType(genericArgsType.ToArray());
        }
    }

    class SubscribeInspector
    {
        List<SubscribeProvider> items = new List<SubscribeProvider>();
        public SubscribeProvider[] Providers
        {
            get { return items.ToArray(); }
        }
        public void InspectMethod(IComponentInfo ctx, IKernel kernel, MethodInfo m)
        {
            var attr = m.GetAttribute<SubscribeAttribute>(true);
            if (attr == null)
                return;
            var ps = m.GetParameters();
            var length = ps.Length;
            if (length > 2)
                return;
            var returnType = m.ReturnType;
            if (returnType == Types.Void)
            {
                switch (length)
                {
                    case 0:
                        items.Add(new ActionSubscribeProvider(m, attr.Topic, attr.Mode));
                        break;
                    case 1:
                        items.Add(new Action1SubscribeProvider(m, ps, attr.Topic, attr.Mode));
                        break;
                    case 2:
                        items.Add(new Action2SubscribeProvider(m, ps, attr.Topic, attr.Mode));
                        break;
                }
            }
            else
            {
                switch (length)
                {
                    case 0:
                        items.Add(new FuncSubscribeProvider(m, attr.Topic, attr.Mode));
                        break;
                    case 1:
                        items.Add(new Func1SubscribeProvider(m, ps, attr.Topic, attr.Mode));
                        break;
                    case 2:
                        items.Add(new Func2SubscribeProvider(m, ps, attr.Topic, attr.Mode));
                        break;
                }
            }
        }
    }

    class MemberInspector
    {

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags FieldFlags = BindingFlags.SetField | Flags;
        private const BindingFlags PropertyFlags = BindingFlags.SetProperty | Flags;

        MemberExportInspector exportInspector = new MemberExportInspector();
        InjectionInspector injectionInspector = new InjectionInspector();
        SubscribeInspector subscriberInspector = new SubscribeInspector();


        public void Inspect(IComponentInfo info, IKernel kernel, Type componentType, string injectionKey)
        {
            if (!componentType.IsSystemAssemblyOfType())
            {
                InspectFields(info, kernel, componentType);
                InspectPropeties(info, kernel, componentType);
                InspectMethods(info, kernel, componentType);
            }

            info.ExtendedProperties[injectionKey] = injectionInspector.Injections;
            info.ExtendedProperties["MembersRegistered"] = exportInspector.Exports;
            info.ExtendedProperties["AppSettings"] = injectionInspector.AppSettingInjections;
            info.ExtendedProperties["subscribeProviders"] = subscriberInspector.Providers;
        }

      
        private void InspectFields(IComponentInfo ctx, IKernel kernel, Type componentType)
        {
            var fields = componentType
                .GetFields(FieldFlags)
                .Where(p => p.DeclaringType == componentType
                    || (!p.DeclaringType.IsSystemAssemblyOfType()))
                .Where(p => !p.Name.EndsWith("k__BackingField"));

            foreach (var f in fields)
            {
                exportInspector.InspectField(ctx, kernel, f);
               
                if (f.HasAttribute<IgnoreAttribute>(false))
                    continue;
                injectionInspector.InspectField(ctx, kernel, f);
            }
        }

        private void InspectPropeties(IComponentInfo ctx, IKernel kernel, Type componentType)
        {
            var props = componentType
                .GetProperties(PropertyFlags)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => p.DeclaringType == componentType
                    || (!p.DeclaringType.IsSystemAssemblyOfType()));

            foreach (var p in props)
            {
                exportInspector.InspectProperty(ctx, kernel, p);
              
                if (p.HasAttribute<IgnoreAttribute>(false))
                    continue;
                injectionInspector.InspectProperty(ctx, kernel, p);
            }
        }

        private void InspectMethods(IComponentInfo ctx, IKernel kernel, Type componentType)
        {
            var methods = componentType
                .GetMethods(Flags)
                .Where(p=>!p.IsSpecialName)
                .Where(p => p.DeclaringType == componentType 
                    || (!p.DeclaringType.IsSystemAssemblyOfType())) ;
            foreach (var m in methods)
            {
                //if (m.IsSpecialName)//Property
                //    continue;
                //if (m.DeclaringType == Types.Object)
                //    continue;

                exportInspector.InspectMethod(ctx, kernel, m);

                subscriberInspector.InspectMethod(ctx, kernel, m);

                if (m.ReturnType != Types.Void || m.HasAttribute<IgnoreAttribute>(false))
                    continue;
                injectionInspector.InspectMethod(ctx, kernel, m);
            }
        }
    }
}