using System;
using System.Collections.Generic;
using NLite.Collections;


namespace NLite.Mini.Resolving
{
    class DependencyManager
    {
        internal static readonly Dictionary<Type, IDependency> DependencyList = new Dictionary<Type, IDependency>();
        internal static readonly Dictionary<string, IDependency> StrDependencyList = new Dictionary<string, IDependency>();
        internal static readonly Dictionary<string, AppSettingDependency> appSettingDependencyList = new Dictionary<string, AppSettingDependency>();

        static readonly object Mutex = new object();

        public static IDependency Get(DependencyCreationContext ctx)
        {
            var info = Get(ctx.Id, ctx.DependencyType, ctx.Kernel, ctx.InjectMany);

            if (ctx.IsOptional)
            {
                if (ctx.DefaultValue != DBNull.Value)
                {
                    info.IsOptional = ctx.IsOptional;
                    info.DefaultValue = ctx.DefaultValue;
                }
            }
            return info;
        }

        public static IDependency Get(string id, Type dependencyType, IKernel kernel, bool injectMany)
        {
            IDependency info;

            if (!string.IsNullOrEmpty(id))
                info = GetOrCreateById(id, dependencyType, kernel);
            else
                info = GetOrCreateByType(id, dependencyType, kernel, injectMany);
            return info;
        }

        public static AppSettingDependency GetAppSettingDependency(string id, Type dependencyType)
        {
            AppSettingDependency info;
            if (!appSettingDependencyList.TryGetValue(id, out info))
            {
                info = new AppSettingDependency(id, dependencyType);
                lock (Mutex)
                    appSettingDependencyList[id] = info;
            }
            return info;
        }

        private static IDependency GetOrCreateByType(string id, Type dependencyType, IKernel kernel, bool injectMany)
        {
            IDependency info;
            if (!DependencyList.TryGetValue(dependencyType, out info))
            {
                if (injectMany)
                    info = new BatchDependency(dependencyType, kernel);
                else
                    info = new Dependency(dependencyType, kernel);
                lock (Mutex)
                    DependencyList[dependencyType] = info;
            }
            return info;
        }

        private static IDependency GetOrCreateById(string id, Type dependencyType, IKernel kernel)
        {
            IDependency info;
            if (!StrDependencyList.TryGetValue(id, out info))
            {

                var tmpInfo = new Dependency(id, dependencyType, kernel);
                lock (Mutex)
                    StrDependencyList[id] = tmpInfo;

                info = tmpInfo;

            }
            return info;
        }

        public static void Refresh(IComponentInfo info, IKernel kernel)
        {
            DependencyList.Values.ForEach(p => p.Refresh(info, kernel));
            StrDependencyList.Values.ForEach(p => p.Refresh(info, kernel));
        }

        internal static void Clear()
        {
            DependencyList.Clear();
            StrDependencyList.Clear();
            appSettingDependencyList.Clear();
        }
    }

    class DependencyCreationContext
    {
        public string Id;
        public Type DependencyType;
        public IKernel Kernel;
        public bool InjectMany;
        public bool IsOptional;
        public object DefaultValue;
    }

}
