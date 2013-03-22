using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Mini.Context;
using NLite.Mini.Fluent.Internal;
using NLite.Reflection;

namespace NLite.Mini.Resolving
{
    class BatchDependency : DependencyBase
    {
        public Type SubElementType;
        List<IComponentContext> Contexts = new List<IComponentContext>();
        Func<Func<object>[]> LazyCreators;
        public override event Action OnRefresh;
        public BatchDependency(Type dependencyType, IKernel kernel)
        {
            DependencyType = dependencyType;

            if (dependencyType == Types.String)
                throw new InjectManyException();

            if (dependencyType.IsArray)
            {
                //Lazy<Component>[],Lazy<Component,Metadata>[],Component[]
                PopulateArray(dependencyType, kernel);
                return;
            }

            if (!dependencyType.IsGenericType)
                throw new InjectManyException();

            var genericFieldTypeName = dependencyType.GetGenericTypeDefinition().Name;
            if (genericFieldTypeName == "Lazy`1")
            {
                //Lazy<IEnumerable<TComponent>>
                PopulateLazyCollection(dependencyType, kernel);
                return;
            }


            SubElementType = TypeHelper.GetElementType(dependencyType);
            if (SubElementType == null)
                throw new InjectManyException();

            //IEnumerable<Component>,IEnumerable<Lazy<Component>>,IEnumerable<Lazy<Component,Metadata>>,IEnumerable<Component<ComponentTwo>>
            if (!SubElementType.IsGenericType)//IEnumerable<Component>
            {
                PopulateContexts(kernel);
                ValueProvider = () =>
                {
                    if (ReferenceManager.Instance.Enabled)
                        return Contexts.Select(p => new InstanceWrapper(p.Kernel, p.Component, p.LifestyleManager.Get(p))).ToArray();

                    var items = Contexts.Select(p => p.LifestyleManager.Get(p));
                    return Mapper.Map(items, items.GetType(), dependencyType);

                };
            }
            else
            {

                var genericArgs = SubElementType.GetGenericArguments();
                genericFieldTypeName = SubElementType.GetGenericTypeDefinition().Name;

                if (genericFieldTypeName == "Lazy`1")//IEnumerable<Lazy<Component>>
                {
                    SubElementType = genericArgs[0];
                    PopulateContexts(kernel);

                    ValueProvider = () =>
                    {
                        var items = Contexts.Select(p => 
                            Lazy
                                .GetLazyValueProvider(
                                    SubElementType
                                    , () => p.LifestyleManager.Get(p)
                                    , p.Kernel
                                    , p.Component)());
                        return Mapper.Map(items, items.GetType(), dependencyType);
                    };
                }
                else if (genericFieldTypeName == "Lazy`2")//IEnumerable<Lazy<Component,Metadata>>
                {
                    SubElementType = genericArgs[0];
                    PopulateContexts(kernel);
                    var metadataType = genericArgs[1];
                    ValueProvider = () =>
                    {
                        var items = Contexts.Select(p => Lazy.GetLazyCreator(
                                                                            SubElementType
                                                                            , metadataType
                                                                            , () => p.LifestyleManager.Get(p)
                                                                            , p.Component.GetMetadataView(metadataType)
                                                                            , p.Kernel
                                                                            , p.Component)());
                        return Mapper.Map(items, items.GetType(), dependencyType);
                    };
                }
                else if (genericArgs[0].IsCloseGenericType())
                {
                    PopulateContexts(kernel);
                    ValueProvider = () =>
                    {
                        if (ReferenceManager.Instance.Enabled)
                            return Contexts.Select(p => new InstanceWrapper(p.Kernel, p.Component, p.LifestyleManager.Get(p))).ToArray();

                        var items = Contexts.Select(p => p.LifestyleManager.Get(p));
                        return Mapper.Map(items, items.GetType(), dependencyType);
                    };
                }
                else
                    throw new InjectManyException();
            }
        }


        public override bool IsAssignableFrom(Type t)
        {
            return SubElementType != null && SubElementType.IsAssignableFrom(t);
        }

        internal static Type GetElementType(Type type)
        {
            if (!Types.IEnumerable.IsAssignableFrom(type))
            {
                if (type.IsGenericType)
                {
                    var genericName = type.GetGenericTypeDefinition().Name;

                    if (genericName == "Lazy`1")
                        type = type.GetGenericArguments()[0];
                    else if (genericName == "Lazy`2")
                        type = type.GetGenericArguments()[0];
                    return GetElementType(type);
                }
                return null;
            }

            var enumerableType = type;
            var subElementType = TypeHelper.GetElementType(type, null);
            if (subElementType.IsGenericType)
            {
                var genericName = subElementType.GetGenericTypeDefinition().Name;

                if (genericName == "Lazy`1")
                    subElementType = subElementType.GetGenericArguments()[0];
                else if (genericName == "Lazy`2")
                    subElementType = subElementType.GetGenericArguments()[0];
            }
            return subElementType;
        }

        //Lazy<IEnumerable<TComponent>>
        private void PopulateLazyCollection(Type dependencyType, IKernel kernel)
        {
            var genericArgs = dependencyType.GetGenericArguments();
            var collectionType = genericArgs[0];
            SubElementType = GetElementType(collectionType);

            PopulateContexts(kernel);

            ValueProvider = () => CollectionLazy.GetLazyValueProvider(
                collectionType
                , () =>Mapper.Map(
                                Contexts.Select(p => p.LifestyleManager.Get(p)).ToArray()
                                , SubElementType.MakeArrayType()
                                , collectionType)
                 , Contexts.Select(s=>s.Kernel).ToArray()
                 , Contexts.Select(s=>s.Component).ToArray())();
        }

        ////Lazy<Component>[],Lazy<Component,Metadata>[],Component[]
        private void PopulateArray(Type dependencyType, IKernel kernel)
        {
            var arrayElementType = SubElementType = dependencyType.GetElementType();
            if (!arrayElementType.IsGenericType)//Component[]
            {
                PopulateContexts(kernel);
                ValueProvider = () =>
                {
                    if (ReferenceManager.Instance.Enabled)
                        return Contexts.Select(p => new InstanceWrapper(p.Kernel, p.Component, p.LifestyleManager.Get(p))).ToArray();

                    var items = Contexts.Select(p => p.LifestyleManager.Get(p));
                    return Mapper.Map(items, items.GetType(), dependencyType);
                };
                return;
            }


            var genericName = arrayElementType.GetGenericTypeDefinition().Name;
            if (genericName == "Lazy`1" || genericName == "Lazy`2")
            {
                //return registry.HasRegister(arrayElementType.GetGenericArguments()[0]);
                var genericArgs = arrayElementType.GetGenericArguments();
                SubElementType = genericArgs[0];
                PopulateContexts(kernel);

                if (genericName == "Lazy`1")//Lazy<Component>[]
                {
                    LazyCreators = () => Contexts.Select(p => 
                        Lazy
                            .GetLazyValueProvider(
                                SubElementType
                                , () => p.LifestyleManager.Get(p)
                                , p.Kernel
                                , p.Component)).ToArray();
                    ValueProvider = () =>
                    {
                        var items = LazyCreators().Select(p => p());
                        return Mapper.Map(items, items.GetType(), dependencyType);
                    };
                }
                else if (genericName == "Lazy`2")//Lazy<Component,Metadata>[]
                {
                    LazyCreators = () => Contexts.Select(p => Lazy.GetLazyCreator(
                                                                                SubElementType
                                                                                , genericArgs[1]
                                                                                , () => p.LifestyleManager.Get(p)
                                                                                , p.Component.GetMetadataView(genericArgs[1])
                                                                                , p.Kernel
                                                                                , p.Component))
                                                .ToArray();
                    ValueProvider = () =>
                    {
                        var items = LazyCreators().Select(p => p());
                        return Mapper.Map(items, items.GetType(), dependencyType);
                    };
                }
            }
        }

        private void PopulateContexts(IKernel kernel)
        {
            if (SubElementType == null)
                throw new InjectManyException();

            var tmpCtxs = kernel.GetComponentContextList(SubElementType);
            if (tmpCtxs.Length > 0)
            {
                Contexts.Clear();
                Contexts.AddRange(tmpCtxs);
                HasDependencied = true;
            }
        }

        public override void Refresh(IComponentInfo info, IKernel kernel)
        {
            //if (HasDependencied) return;

            foreach (var c in info.Contracts)
                if (c == SubElementType)
                {
                    var old = HasDependencied;
                    PopulateContexts(kernel);
                    if (OnRefresh != null && HasDependencied && !old)
                        OnRefresh();

                    break;
                }
        }
    }
}
