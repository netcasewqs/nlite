using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using NLite.Collections;
using NLite.Mini.Activation;
using NLite.Mini.Context;
using NLite.Mini.Lifestyle;
using NLite.Reflection;
using NLite.Mini.Proxy;
using NLite.Mini.Listener;
using NLite.Internal;
using NLite.Mini.Resolving;
using NLite.Mini.Fluent.Internal;
using NLite.Messaging;

namespace NLite.Mini
{
   /// <summary>
   /// Mini容器微内核
   /// </summary>
    [DebuggerDisplay("{IdStores.Values}")]
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public sealed class Kernel:BooleanDisposable, IKernel,ILazyServiceLocator
    {
        internal readonly IDictionary<string, ComponentRegistration> IdStores;
        readonly IDictionary<Type, List<ComponentRegistration>> TypeStores;
        readonly IComponentListener Listner;
        IActivatorFactory ActivatorRegistry;
        ILifestyleManagerFactory LifestyleManagerRegistry;
        ReinjectionManager reinjectionManager = new ReinjectionManager();

        /// <summary>
        /// 构造缺省内核容器
        /// </summary>
        public Kernel():this(
            new ComponentListenManager()
            , new LifestyleManagerFactory()
            , new ActivatorFactory()
            , ClassLoader.Current)
        {
        }

        /// <summary>
        /// 构造内核容器
        /// </summary>
        /// <param name="listenManager">组件监听管理器</param>
        /// <param name="lifestyleManagerRegistry">生命周期管理器注册表</param>
        /// <param name="activatorFactory">组件工厂的工厂</param>
        /// <param name="classLoader">类型加载器</param>
        public Kernel(
            IComponentListenerManager listenManager,
            ILifestyleManagerFactory lifestyleManagerRegistry,
            IActivatorFactory activatorFactory,
            IClassLoader classLoader)
        {
            if (listenManager == null)
                throw new ArgumentNullException("listenManager");
            if (lifestyleManagerRegistry == null)
                throw new ArgumentNullException("lifestyleManagerRegistry");
            if (activatorFactory == null)
                throw new ArgumentNullException("activatorFactory");
            if (classLoader == null)
                throw new ArgumentNullException("classLoader");

            ListenerManager = listenManager;
            LifestyleManagerRegistry = lifestyleManagerRegistry;
            ActivatorRegistry = activatorFactory;

            IdStores = new Dictionary<string, ComponentRegistration>(StringComparer.OrdinalIgnoreCase);
            TypeStores = new Dictionary<Type, List<ComponentRegistration>>();

            Listner = ListenerManager as IComponentListener;

            RegisterInstance("ServiceLocator", typeof(IServiceLocator), this);
            RegisterInstance("ServiceRegistry", typeof(IServiceRegistry), this);


            RegisterInstance(AppDomain.CurrentDomain.Id.ToString() + ":" + classLoader.GetType(), typeof(IClassLoader), classLoader);

            ListenerManager.Init(this);

            RegisterListners();
            ListenerManager.Enabled = true;
        }


        //注册默认监听器
        private void RegisterListners()
        {
            //ListenerManager.Register(new AopListener());//Aop监听器
            //ListenerManager.Register(new DisposalListener());//Dispose监听器
            //ListenerManager.Register(new InitializationListener());//初始化监听器
            //ListenerManager.Register(new SupportInitializeListener());//Support初始化监听器
            //ListenerManager.Register(new StartableListener());//启动停止监听器
            //ListenerManager.Register(new SubscribeListener());//消息总线的订阅监听器
            ListenerManager.Register(new InjectionListener(reinjectionManager));//注入监听器
            //ListenerManager.Register(new MemberExportListener());//组件成员导出与注入监听器
            //ListenerManager.Register(new AppSettingInjectionListener());//AppSetting注入监听器
        }

      
        /// <summary>
        /// 父容器
        /// </summary>
        public IKernel Parent { get; set; }

        /// <summary>
        /// 组件监听管理器
        /// </summary>
        public IComponentListenerManager ListenerManager { get; private set; }
        
        /// <summary>
        /// 是否注册了含有组件Id的组件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasRegister(string id)
        {
            Guard.NotNullOrEmpty(id, "id");

            var flag= HasRegisterExcludeParent(id);
            if (flag) return true;

            return Parent != null ? Parent.HasRegister(id) : false;
        }

        private bool HasRegisterExcludeParent(string id)
        {
            return IdStores.ContainsKey(id);
        }
        /// <summary>
        /// 是否注册了实现给定契约接口的组件
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public bool HasRegister(Type contract)
        {
            Guard.NotNull(contract, "contract");

            var flag = HasRegisterExcludeParent(contract);
            if (flag) return true;

            return Parent != null ? Parent.HasRegister(contract) : false;
        }

        private bool HasRegisterExcludeParent(Type contract)
        {
            if (TypeStores.ContainsKey(contract))
                return true;
            if (!contract.IsGenericType)
                return false;

            var openGenericType = contract.GetGenericTypeDefinition();
            if(!TypeStores.ContainsKey(openGenericType))
            {
                return TypeStores.Keys.FirstOrDefault(p =>
                    {
                        return p.Assembly == openGenericType.Assembly
                            && p.Namespace == openGenericType.Namespace
                            && p.Name == openGenericType.Name;
                    }) != null;
            }
            return true;
        }

        /// <summary>
        /// 通过组件元数据注册组件到容器中
        /// </summary>
        /// <param name="componentInfo"></param>
        /// <returns></returns>
        public IServiceRegistry Register(IComponentInfo componentInfo)
        {
            Guard.NotNull(componentInfo, "componentInfo");
            
            if (HasRegisterExcludeParent(componentInfo.Id))
                throw new RepeatRegistrationException(
                       String.Format("There is a component already registered for the given key {0}", componentInfo.Id));

            if (!Listner.OnMetadataRegistering(componentInfo) || IdStores.ContainsKey(componentInfo.Id))
                return this;

            var activator = CreateActivator(componentInfo);

            var lifestyleMgr = CreateLifestyleManager(componentInfo, activator);

            var pair = new ComponentRegistration(componentInfo, lifestyleMgr);
            lock (IdStores)
            {
                IdStores[componentInfo.Id] = pair;

                if (componentInfo.Contracts.Length > 0)
                {
                    foreach (var contract in componentInfo.Contracts)
                    {
                        if (!TypeStores.ContainsKey(contract))
                            TypeStores[contract] = new List<ComponentRegistration>();

                        TypeStores[contract].Add(pair);
                    }
                }
            }
            Listner.OnMetadataRegistered(componentInfo);
            pair.LifestyleManager = CreateProxyLifestyleManager(componentInfo, pair.LifestyleManager);

            return this;
        }

       

        private ILifestyleManager CreateLifestyleManager(IComponentInfo info, IActivator activator)
        {
            var lifestyleMgr = LifestyleManagerRegistry.Create(info.Lifestyle);
            if (info.ExtendedProperties.ContainsKey("proxy"))
                lifestyleMgr = new ProxyLifestyleManager(lifestyleMgr);

            lifestyleMgr.Init(activator, this, info, Listner.OnPreDestroy,Listner.OnFetch);
            return lifestyleMgr;
        }

        private ILifestyleManager CreateProxyLifestyleManager(IComponentInfo info, ILifestyleManager lifestyleMgr)
        {
            if (info.ExtendedProperties.ContainsKey("proxy"))
                lifestyleMgr = new ProxyLifestyleManager(lifestyleMgr);

            return lifestyleMgr;
        }

        private IActivator CreateActivator(IComponentInfo info)
        {
            var activator = ActivatorRegistry.Create(info.Activator);

            var delegateActivator = activator as DelegateActivator;
            if (delegateActivator != null)
                delegateActivator.Creator = (ctx) => info.Factory();
            return activator;
        }

        internal static IComponentInfo CreateComponentInfo(string id, Type contract, Type implementation, string activator, LifestyleFlags lifestyleType)
        {
            var info = new ComponentInfo(id, contract, implementation, activator, lifestyleType);
            return info;
        }

        ComponentContext NewCreateContext(IDictionary<string, object> args, IComponentInfo info, params Type[] genericParameters)
        {
            return new ComponentContext(this, info, args, genericParameters);
        }

        ComponentContext NewCreateContext(IComponentInfo info, object[] args, params Type[] genericParameters)
        {
            return new ComponentContext(this, info, args, genericParameters);
        }

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IServiceRegistry RegisterInstance(string id, Type contract, object instance)
        {
            //Guard.NotNull(contract, "contract");
            Guard.NotNull(instance, "instance");

            if ( contract != null && !contract.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException("!contract.IsAssignableFrom(instance.GetType())");

            if (string.IsNullOrEmpty(id))
                id = instance.GetType().FullName;

            if (HasRegisterExcludeParent(id))
                throw new RepeatRegistrationException(
                       String.Format("There is a component already registered for the given key {0}", id));

            var info = CreateComponentInfo(id, contract, instance.GetType(), ActivatorType.Instance, LifestyleFlags.Singleton);
            info.ExtendedProperties["instance"]= instance;

            if (!Listner.OnMetadataRegistering(info))
                return this;

            var activator = ActivatorRegistry.Create(info.Activator);

            var lifestyleMgr = CreateLifestyleManager(info, activator);
            lifestyleMgr.Init(activator, this, info, Listner.OnPreDestroy,Listner.OnFetch);

            Listner.OnMetadataRegistering(info);
            var pair = new ComponentRegistration(info, lifestyleMgr);

            lock (IdStores)
            {
                IdStores[id] = pair;

                foreach (var c in info.Contracts)
                {
                    if (!TypeStores.ContainsKey(c))
                        TypeStores[c] = new List<ComponentRegistration>();
                    TypeStores[c].Add(pair);
                }
            }

            Listner.OnMetadataRegistered(info);

            pair.LifestyleManager = CreateProxyLifestyleManager(info, pair.LifestyleManager);

            Listner.OnPostCreation(new ComponentContext(this, null) {  Component = pair.Component, Instance = instance});

            return this;
        }

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IServiceRegistry RegisterInstance(string id, object instance)
        {
            Guard.NotNull(instance, "instance");
            return RegisterInstance(id, instance.GetType(), instance);
        }

        /// <summary>
        /// 将组件实例所依赖的其它组件都通过容器自动注入进来
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IServiceRegistry Compose(object instance)
        {
            Guard.NotNull(instance, "instance");
            var info = new ComponentInfo(instance.GetType());

            if (!Listner.OnMetadataRegistering(info))
                return this;

            Listner.OnMetadataRegistered(info);
            var ctx = new ComponentContext(this, null) {  Instance = instance, Component = info};
            Listner.OnPostCreation(ctx);
            Listner.OnInitialization(ctx);
            Listner.OnPostInitialization(ctx);
            //重组
            reinjectionManager.Reinjection(info.Implementation, this);
            Listner.OnMetadataUnregistered(info);
            return this;
        }

        /// <summary>
        /// 通过契约类型注销相应的组件
        /// </summary>
        /// <param name="contract"></param>
        public void UnRegister(Type contract)
        {
            Guard.NotNull(contract, "contract");
            if (HasRegisterExcludeParent(contract))
            {
                var pairs = TypeStores[contract];
                TypeStores.Remove(contract);

                var ids = (from item in IdStores.Keys
                           from p in pairs
                           from t in TypeStores.Values.SelectMany(c=>c)
                           where t.Component != p.Component 
                                && IdStores[item].Component == p.Component
                                && t.Component == IdStores[item].Component
                           select item).ToArray();

                if (ids.Length > 0)
                    foreach (var id in ids)
                        IdStores.Remove(id);

                foreach (var pair in pairs)
                {
                    pair.Component.ExtendedProperties.Clear();
                   
                    pair.LifestyleManager.Dispose();
                }
            }
            else if(Parent != null)
            {
                Parent.UnRegister(contract);
            }
            
        }

        /// <summary>
        /// 通过组件Id注销组件
        /// </summary>
        /// <param name="id"></param>
        public void UnRegister(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            if (HasRegisterExcludeParent(id))
            {
                var pair = IdStores[id];

                pair.Component.ExtendedProperties.Clear();
                IdStores.Remove(id);

                var contracts = (from item in TypeStores.Keys
                            from c in pair.Component.Contracts
                            from p in TypeStores[item]
                            where item == c && pair.Component == p.Component
                            select new { Contract = c,Pair = p}).ToArray();

                if (contracts.Length > 0)
                {
                    foreach (var contract in contracts)
                    {
                        var pairs = TypeStores[contract.Contract];
                        pairs.Remove(contract.Pair);
                        if (pairs.Count == 0)
                            TypeStores.Remove(contract.Contract);
                    }
                }

                pair.LifestyleManager.Dispose();
            }
            else if (Parent != null)
            {
                Parent.UnRegister(id);
            }
        }


        #region IServiceLocator Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Get(string id, IDictionary<string, object> args)
        {
            Guard.NotNullOrEmpty(id, "id");
            return GetByNamedArgs(id, args);
        }

        private object GetByNamedArgs(string id, IDictionary<string, object> args)
        {
            Guard.NotNullOrEmpty(id, "id");
            var ctx = GetComponentContextByNamedArgs(id, args);
            return ctx != null ? ctx.LifestyleManager.Get(ctx) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Get(Type contract, IDictionary<string, object> args)
        {
            Guard.NotNull(contract, "contract");
            return GetByNamedArgs(contract, args);
        }

        private object GetByNamedArgs(Type contract, IDictionary<string, object> args)
        {
            lock (contract)
            {
                var ctx = GetComponentContextByNamedArgs(contract, args);
                if (ctx != null)
                    return ctx.LifestyleManager.Get(ctx);
                else
                    return AutoRegisterAndGet(contract, args);
            }
        }

        private object AutoRegisterAndGet(Type contract, IDictionary<string, object> args)
        {
            if (contract.IsAbstract || contract.IsInterface)
                return null;
            
            var info = new ComponentInfo(contract.FullName, contract, contract);

            if (!Listner.OnMetadataRegistering(info))
                return null;

            var activator = CreateActivator(info);

            var lifestyleMgr = CreateLifestyleManager(info, activator);

            var pair = new ComponentRegistration(info, lifestyleMgr);
            IdStores[info.Id] = pair;

            if (info.Contracts.Length > 0)
            {
                foreach (var c in info.Contracts)
                {
                    if (!TypeStores.ContainsKey(c))
                        TypeStores[c] = new List<ComponentRegistration>();

                    TypeStores[c].Add(pair);
                }
            }
            pair.ComponentContext = NewCreateContext(args, info, GetGenericParameters(contract));

         
            pair.LifestyleManager = CreateProxyLifestyleManager(info, pair.LifestyleManager);
            pair.ComponentContext.LifestyleManager = pair.LifestyleManager;
            Listner.OnMetadataRegistered(info);
            return pair.LifestyleManager.Get(pair.ComponentContext);
        }

        private object AutoRegisterAndGet(Type contract, params object[] args)
        {
            if (contract.IsAbstract || contract.IsInterface)
                return null;

            var info = new ComponentInfo(contract.FullName, contract, contract);

            if (!Listner.OnMetadataRegistering(info))
                return null;

            var activator = CreateActivator(info);

            var lifestyleMgr = CreateLifestyleManager(info, activator);

            var pair = new ComponentRegistration(info, lifestyleMgr);
            IdStores[info.Id] = pair;

            if (info.Contracts.Length > 0)
            {
                foreach (var c in info.Contracts)
                {
                    if (!TypeStores.ContainsKey(c))
                        TypeStores[c] = new List<ComponentRegistration>();

                    TypeStores[c].Add(pair);
                }
            }
            pair.ComponentContext = NewCreateContext(info,args, GetGenericParameters(contract));
            pair.LifestyleManager = CreateProxyLifestyleManager(info, pair.LifestyleManager);
            pair.ComponentContext.LifestyleManager = lifestyleMgr;
            Listner.OnMetadataRegistered(info);

            return pair.LifestyleManager.Get(pair.ComponentContext);
        }

        private static Type[] GetGenericParameters(Type contract)
        {
            if (contract.IsGenericType)
                return contract.GetGenericArguments();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IComponentContext GetComponentContextByNamedArgs(Type contract, IDictionary<string, object> args)
        {
            Guard.NotNull(contract, "contract");
            var paris = GetComponentPairList(contract);

            if (paris != null)
            {
                var pair = paris[0];
                ComponentContext ctx;
                if (pair.ComponentContext == null)
                {
                    ctx = NewCreateContext(args, pair.Component, GetGenericParameters(contract));
                    ctx.LifestyleManager = pair.LifestyleManager;
                }
                else
                {
                    ctx = pair.ComponentContext;
                    ctx.NamedArgs = args;
                }
               
                return ctx;
            }

            return Parent != null ? Parent.GetComponentContextByNamedArgs(contract, args) : null;
        }

        public IComponentContext GetComponentContextByOptionArgs(Type contract, params object[] args)
        {
            Guard.NotNull(contract, "contract");
            var paris = GetComponentPairList(contract);

            if (paris != null)
            {
                var pair = paris[0];
                ComponentContext ctx;
                if (pair.ComponentContext == null)
                {
                    ctx = NewCreateContext(pair.Component, args, GetGenericParameters(contract));
                    ctx.LifestyleManager = pair.LifestyleManager;
                }
                else
                {
                    ctx = pair.ComponentContext;
                    ctx.Args = args;
                }
                return ctx;
            }

            return Parent != null ? Parent.GetComponentContextByOptionArgs(contract, args) : null;
        }

        public IComponentContext[] GetComponentContextList(Type contract)
        {
            Guard.NotNull(contract, "contract");
            var result = new List<IComponentContext>();
            var paris = GetComponentPairList(contract);

            if (paris != null)
            {
                foreach (var pair in paris)
                {
                    var ctx = NewCreateContext(pair.Component, null, GetGenericParameters(contract));
                    ctx.LifestyleManager = pair.LifestyleManager;
                    result.Add(ctx);
                }
            }

            if (Parent != null)
            {
                var parentResult = Parent.GetComponentContextList(contract);
                if (parentResult != null && parentResult.Length > 0)
                    result.AddRange(parentResult);
            }

            return result.ToArray();
        }
        private List<ComponentRegistration> GetComponentPairList(Type contract)
        {
            List<ComponentRegistration> pairs = null;

            if (TypeStores.TryGetValue(contract, out pairs))
                return pairs;
            
            if (!contract.IsCloseGenericType())
                return null;

            var type = contract.GetGenericTypeDefinition();
            if (!TypeStores.TryGetValue(type, out pairs))
            {
                if (contract.IsInterface)
                {
                    foreach (var key in TypeStores.Keys.ToArray())
                    {
                        if (key.Name == contract.Name)
                        {
                            TypeStores.TryGetValue(key, out pairs);
                            return pairs;
                        }
                    }
                }
            }

            return pairs;
        }

        public IComponentContext GetComponentContextByNamedArgs(string id, IDictionary<string, object> args)
        {
            Guard.NotNullOrEmpty(id, "id");

            ComponentRegistration pair;
            if (IdStores.TryGetValue(id, out pair))
            {
                //var ctx = NewCreateContext(args, pair.Component,null);
                //ctx.LifestyleManager = pair.LifestyleManager;
                //return ctx;
                pair.ComponentContext.NamedArgs = args;
                return pair.ComponentContext;
            }
            return Parent != null ? Parent.GetComponentContextByNamedArgs(id, args) : null;
        }

        public IComponentContext GetComponentContextByOptionArgs(string id, params object[] args)
        {
            Guard.NotNullOrEmpty(id, "id");
            ComponentRegistration pair;
            if (IdStores.TryGetValue(id, out pair))
            {
                //var ctx = NewCreateContext(pair.Component,args, null);
                //ctx.LifestyleManager = pair.LifestyleManager;
                //return ctx;
                pair.ComponentContext.Args = args;
                return pair.ComponentContext;
            }
            return Parent != null ? Parent.GetComponentContextByOptionArgs(id, args) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Get(string id, params object[] args)
        {
            Guard.NotNullOrEmpty(id, "id");
            return GetByOptionArgs(id, args);
        }

        private object GetByOptionArgs(string id, object[] args)
        {
            var ctx = GetComponentContextByOptionArgs(id, args);
            return ctx != null ? ctx.LifestyleManager.Get(ctx) : null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Get(Type contract, params object[] args)
        {
            Guard.NotNull(contract, "contract");
            return GetByOptionArgs(contract, args);
        }

        private object GetByOptionArgs(Type contract, object[] args)
        {
            var ctx = GetComponentContextByOptionArgs(contract, args);
            return ctx != null ? ctx.LifestyleManager.Get(ctx) : AutoRegisterAndGet(contract, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object Get(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            return GetByOptionArgs(id, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public object Get(Type contract)
        {
            Guard.NotNull(contract, "contract");
            return GetByOptionArgs(contract, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Lazy<T> LazyGet<T>(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            Func<T> func = () => string.IsNullOrEmpty(id) ? (T)Get(typeof(T)) : (T)Get(id);
            return new Lazy<T>(func);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Lazy<T, TMetadata> LazyMetadataGet<T, TMetadata>(string id)
        {
            Guard.NotNullOrEmpty(id, "id");
            var o = LazyMetadataGetExcludeParent<T, TMetadata>(id);
            if (o != null) return o;
            return Parent != null ? Parent.LazyMetadataGet<T, TMetadata>(id):null;
        }

        private Lazy<T, TMetadata> LazyMetadataGetExcludeParent<T, TMetadata>(string id)
        {
            var contractType = typeof(T);
            Func<T> func = null;

            if (!string.IsNullOrEmpty(id) && HasRegisterExcludeParent(id))
                func = () => (T)Get(id);
            else if (HasRegisterExcludeParent(contractType))
                func = () => (T)Get(contractType);
            else
                return null;

            var info = string.IsNullOrEmpty(id) ? TypeStores[typeof(T)][0].Component : IdStores[id].Component;
            return new Lazy<T, TMetadata>(func, info.GetMetadataView<TMetadata>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public IEnumerable<object> GetAll(Type contract)
        {
            Guard.NotNull(contract, "contract");
            return 
                GetAllExcludeParent(contract)
                .Union(Parent != null ? Parent.GetAll(contract) : new object[0])
                .ToArray();

        }

        private IEnumerable<object> GetAllExcludeParent(Type contract)
        {
            var result = new List<object>();
            if (!HasRegisterExcludeParent(contract))
                return result;

            var ctx = new ComponentContext(this, GetGenericParameters(contract));

            int count = TypeStores[contract].Count;

            for (var i = 0; i < count; i++)
                result.Add(TypeStores[contract][i].LifestyleManager.Get(ctx.Init(TypeStores[contract][i].Component)));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>()
        {
            return 
                GetAllExcludeParent<T>()
                .Union(Parent != null ? Parent.GetAll<T>() : new T[0])
                .ToArray();
        }

        private IEnumerable<T> GetAllExcludeParent<T>()
        {
            var result = new List<T>();
            var contract = typeof(T);
            if (!HasRegisterExcludeParent(contract))
                return result;

            var ctx = new ComponentContext(this, GetGenericParameters(contract));

            foreach (var item in TypeStores[contract])
                result.Add(Get<T>(ctx, item));
            return result;
        }

        private static T Get<T>(ComponentContext ctx, ComponentRegistration item)
        {
            return (T)item.LifestyleManager.Get(ctx.Init(item.Component));
        }

        static Lazy<T> LazyGet<T>(ComponentContext ctx, ComponentRegistration item)
        {
            return new Lazy<T>(() => Get<T>(ctx, item));
        }

        static Lazy<T, TMetadata> LazyGet<T, TMetadata>(ComponentContext ctx, ComponentRegistration item)
        {
            return new Lazy<T, TMetadata>(() => Get<T>(ctx, item), item.Component.GetMetadataView<TMetadata>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Lazy<T>> LazyGetAll<T>()
        {
            return 
                LazyGetAllExcludeParent<T>()
                .Union(Parent != null ? Parent.LazyGetAll<T>() : new Lazy<T>[0])
                .ToArray();
        }

        private IEnumerable<Lazy<T>> LazyGetAllExcludeParent<T>()
        {
            var result = new List<Lazy<T>>();

            var contract = typeof(T);
            if (!HasRegisterExcludeParent(contract))
                return result;

            var ctx = new ComponentContext(this, GetGenericParameters(contract));

            foreach (var item in TypeStores[contract])
                result.Add(LazyGet<T>(ctx, item));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <returns></returns>
        public IEnumerable<Lazy<T, TMetadata>> LazyMetadataGetAll<T, TMetadata>()
        {
            return 
                LazyMetadataGetAllExcludeParent<T, TMetadata>()
                .Union(Parent != null ? Parent.LazyMetadataGetAll<T,TMetadata>(): new Lazy<T,TMetadata>[0])
                .ToArray();
        }

        private IEnumerable<Lazy<T, TMetadata>> LazyMetadataGetAllExcludeParent<T, TMetadata>()
        {
            var result = new List<Lazy<T, TMetadata>>();
            var contract = typeof(T);
            if (!HasRegisterExcludeParent(contract))
                return result;

            var ctx = new ComponentContext(this, GetGenericParameters(contract));

            foreach (var item in TypeStores[contract])
                result.Add(LazyGet<T, TMetadata>(ctx, item));
            return result;
        }

        #endregion

        #region IServiceProvider Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            Guard.NotNull(serviceType, "serviceType");

            return Get(serviceType);
        }

        #endregion


        #region IDisposable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            ReferenceManager.Instance.UnregisterAllByKernel(this);

            //TODO:
            var items = (from item in TypeStores.Values
                         from i in item
                         select i.LifestyleManager).ToArray();

            if (items != null && items.Length > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] != null)
                        items[i].Dispose();
                }
            }

            ListenerManager.Dispose();
            DependencyManager.Clear();

            TypeStores.Clear();
            IdStores.Clear();
        }

        #endregion



        IEnumerator<IComponentInfo> IEnumerable<IComponentInfo>.GetEnumerator()
        {
            var items = new HashSet<IComponentInfo>();
            foreach (var m in TypeStores.Values)
                foreach (var n in m)
                    items.Add(n.Component);
            foreach (var m in IdStores.Values)
                items.Add(m.Component);
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this as IKernel).GetEnumerator();
        }
    }
}
