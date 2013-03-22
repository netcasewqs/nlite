using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Reflection;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// 契约服务
    /// </summary>
    public static class  ContractService
    {
        /// <summary>
        /// 返回组件实现的所有契约类型
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetContracts(this Type componentType)
        {
            Guard.NotNull(componentType, "componentType");
            var contracts = new HashSet<Type>();
            GetContracts(componentType, contracts);
            return contracts;
        }

        /// <summary>
        /// 返回组件实现的所有契约类型
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="contracts"></param>
        internal static void GetContracts(Type componentType,HashSet<Type> contracts )
        {
            FindContractsFromInterfaces(componentType,contracts);
            FindContractsFromBaseClasses(componentType,contracts);
        }

        private static void FindContractsFromInterfaces(Type type, HashSet<Type> contracts)
        {
            var results = type
                .GetInterfaces()
                .Where(i=>!i.Assembly.IsSystemAssembly())
               // .Where(i => i.HasAttribute<ContractAttribute>(true))
                .ToList();
            if (results.Count > 0)
                foreach (var contract in results)
                    contracts.Add(contract);
        }

        private static void FindContractsFromBaseClasses(Type type, HashSet<Type> contracts)
        {
            if (type.Assembly.IsSystemAssembly())
                return;
            Type candidateContract = type;
            while (candidateContract != null)
            {
                if (candidateContract.HasAttribute<ContractAttribute>(true))
                    contracts.Add(candidateContract);
                candidateContract = candidateContract.BaseType;
            }

        }
    }
}
