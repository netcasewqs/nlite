using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Mini.Context;
using NLite.Mini.Resolving;
using NLite.Reflection;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 注入监听器
    /// </summary>
    sealed class InjectionListener : ComponentListenerAdapter
    {
        ReinjectionManager reinjectionManager;
        internal Dictionary<IComponentContext, IMemberInjection[]> reinjectionMap;
        private static IAttributeProviderVisitor<bool> ignoreVisitor = AttributeProviderVisitorRepository.Get<bool>();
        internal InjectionListener(ReinjectionManager reinjectionManager)
        {
            this.reinjectionManager = reinjectionManager;
            reinjectionMap = reinjectionManager.reinjectionMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public override void OnMetadataRegistered(IComponentInfo info)
        {
            var componentType = info.Implementation;
            DependencyManager.Refresh(info, Kernel);
        
            if (!info.IsGenericType())
            {

                if (info.Activator == ActivatorType.Default)
                    ResolveConstructorInjection(info, componentType);
                if (componentType != null)
                    ResolveInjectionInfo(info, componentType, componentType.FullName + ":InjectionInfos");
            }

            reinjectionManager.Reinjection(info.Implementation, Kernel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="ctx"></param>
        public override void OnPreCreation(IComponentContext ctx)
        {
            if (ctx.Component.Implementation == null)
                return;

            var isGeneric = ctx.Component.IsGenericType();
            var componentType = isGeneric? ctx.Component.Implementation.MakeGenericType(ctx.GenericParameters):ctx.Component.Implementation;
            var key = componentType.FullName + ":ctorInjections";

            if (ctx.Component.ExtendedProperties.ContainsKey(key))
                return;

            ResolveConstructorInjection(ctx.Component, componentType);

            key = componentType.FullName + ":InjectionInfos";

            ResolveInjectionInfo(ctx.Component, componentType, key);
        }

        private void ResolveConstructorInjection(IComponentInfo info, Type componentType)
        {
            var key = componentType.FullName + ":ctorInjections";

            info.ExtendedProperties[key] = componentType
                .GetConstructors()
                .Where(p => !ignoreVisitor.VisitConstructor(p))
                .Select(p => AttributeProviderInspector.InspectConstructor(info, Kernel, p))
                .OrderBy(p => p.IsMarkedInjection)
                .OrderByDescending(p => p.Dependencies.Length)
                .ToArray();

        }

        private void ResolveInjectionInfo(IComponentInfo info, Type componentType, string key)
        {
            if (componentType == null)//if (bindingInfo.Implementation != null)
                return;
            new MemberInspector().Inspect(info, Kernel, componentType, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
        {
            if (ctx.Instance == null)
                return;

            var instance = ctx.Instance;
            var componentType = instance.GetType();
            var key = componentType.FullName + ":InjectionInfos";
            var ep = ctx.Component.ExtendedProperties;

            IMemberInjection[] injections = null;
            if (ep.ContainsKey(key))
                injections = ep[key] as IMemberInjection[];
            else
            {
                ResolveInjectionInfo(ctx.Component, componentType, key);
                injections = ep[key] as IMemberInjection[];
            }

            if (injections == null || injections.Length == 0)
                return;

            bool hasReinjection = false;
            foreach (var item in injections)
            {
                if (item.Reinjection)
                    hasReinjection = true;
                item.Inject(ctx);
            }


            if (hasReinjection)
            {
                lock (reinjectionMap)
                    reinjectionMap.Add(ctx
                        , injections.Where(p => p.Reinjection).ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instance"></param>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            var key = reinjectionMap.Keys.FirstOrDefault(p => p.Instance == instance);
            if (key != null)
                lock (reinjectionMap)
                    reinjectionMap.Remove(key);
        }

    }


}
