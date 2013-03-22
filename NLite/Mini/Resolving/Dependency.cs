using System;
using NLite.Mini.Context;
using NLite.Mini.Fluent.Internal;
using NLite.Reflection;

namespace NLite.Mini.Resolving
{
    class Dependency : DependencyBase
    {
        internal readonly string Id;
        internal readonly bool HasId;
        internal IComponentContext ComponentContext;
        internal bool IsLazyType;
        internal Type[] GenericArguments;
        public override event Action OnRefresh;
        internal Dependency() { }

        public Dependency(Type dependencyType, IKernel kernel)
        {
            DependencyType = dependencyType;

            Func<object> tmpValueProvider = () => ComponentContext.LifestyleManager.Get(ComponentContext);

            if (dependencyType.IsGenericType)
            {
                var genericFieldTypeName = dependencyType.GetGenericTypeDefinition().Name;

                GenericArguments = dependencyType.GetGenericArguments();
                if (genericFieldTypeName == "Lazy`1")
                {
                    ComponentContext = kernel.GetComponentContextByNamedArgs(GenericArguments[0], null);
                    ValueProvider = Lazy.GetLazyValueProvider(GenericArguments[0], tmpValueProvider, ComponentContext.Kernel, ComponentContext.Component);
                    IsLazyType = true;
                }
                else if (genericFieldTypeName == "Lazy`2")
                {
                    ComponentContext = kernel.GetComponentContextByNamedArgs(GenericArguments[0], null);

                    ValueProvider = Lazy.GetLazyCreator(
                        GenericArguments[0]
                        , GenericArguments[1]
                        , tmpValueProvider
                        , ComponentContext.Component.GetMetadataView(GenericArguments[1])
                        ,kernel
                        , ComponentContext.Component);
                    IsLazyType = true;
                }
            }
            if (ValueProvider == null)
            {
                ComponentContext = kernel.GetComponentContextByNamedArgs(dependencyType, null);
                ValueProvider = PopulateValueProvider();
            }
            HasDependencied = ComponentContext != null;
        }

        public Dependency(string id, Type dependencyType, IKernel kernel)
        {
            DependencyType = dependencyType;
            ComponentContext = kernel.GetComponentContextByNamedArgs(id, null);
            HasDependencied = ComponentContext != null;
            Id = id;
            HasId = true;
            ValueProvider = PopulateValueProvider();
        }

        private Func<object> PopulateValueProvider()
        {

            Func<object> tmpValueProvider = () =>
            {
                if (ComponentContext != null)
                {
                    if (ReferenceManager.Instance.Enabled)
                        return new InstanceWrapper(ComponentContext.Kernel, ComponentContext.Component, ComponentContext.LifestyleManager.Get(ComponentContext));
                    return ComponentContext.LifestyleManager.Get(ComponentContext);
                }
                return IsOptional ? DefaultValue : null;
            };

            return tmpValueProvider;
        }

        public override bool IsAssignableFrom(Type t)
        {

            if (DependencyType.IsAssignableFrom(t))
                return true;
            return IsLazyType && GenericArguments[0].IsAssignableFrom(t);
        }

        public override void Refresh(IComponentInfo info, IKernel kernel)
        {
            if (HasDependencied) return;
            if (HasId)
            {
                if (info.Id == Id)
                {
                    //TODO:unit test
                    ComponentContext = kernel.GetComponentContextByNamedArgs(Id, null);
                    HasDependencied = true;
                    if (OnRefresh != null)
                        OnRefresh();
                }
            }
            else
            {
                var tmpDependencyType = IsLazyType ? GenericArguments[0] : DependencyType;
                foreach (var c in info.Contracts)
                    if (c == DependencyType)
                    {
                        ComponentContext = kernel.GetComponentContextByNamedArgs(tmpDependencyType, null);
                        HasDependencied = true;
                        if (OnRefresh != null)
                            OnRefresh();
                        break;
                    }
            }
        }

      
    }

    //class LazyWrapper1
    //{
       
    //}
}
