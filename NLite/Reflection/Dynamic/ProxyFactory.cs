//
// NProxy is a library for the .NET framework to create lightweight dynamic proxies.
// Copyright © Martin Tamme
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Reflection.Dynamic.Internal.Caching;
using NLite.Reflection.Dynamic.Internal.Definitions;
using NLite.Reflection.Dynamic.Internal.Emit;
using NLite.Reflection.Dynamic.Internal.Reflection;

namespace NLite.Reflection.Dynamic
{
    /// <summary>
    /// Represents the proxy factory.
    /// </summary>
    sealed class ProxyFactory : IProxyFactory
    {
        /// <summary>
        /// The type builder factory.
        /// </summary>
        private readonly ProxyTypeBuilderFactory _typeBuilderFactory;

        /// <summary>
        /// The interception filter.
        /// </summary>
        private readonly IInterceptionFilter _interceptionFilter;

        /// <summary>
        /// The proxy template cache.
        /// </summary>
        private readonly LockOnWriteCache<IProxyDefinition, IProxyTemplate> _proxyTemplateCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory"/> class.
        /// </summary>
        /// <param name="interceptionFilter">The interception filter.</param>
        public ProxyFactory(bool strongNamedAssembly = false,bool canSaveAssembly = false, IInterceptionFilter interceptionFilter = null)
        {
            _typeBuilderFactory = new ProxyTypeBuilderFactory(strongNamedAssembly, canSaveAssembly);

            if (interceptionFilter == null)
                interceptionFilter = new NonInterceptedInterceptionFilter();

            _interceptionFilter = interceptionFilter;

            _proxyTemplateCache = new LockOnWriteCache<IProxyDefinition, IProxyTemplate>();
        }

        /// <summary>
        /// Creates a proxy definition for the specified declaring type and interface types.
        /// </summary>
        /// <param name="declaringType">The declaring type.</param>
        /// <param name="interfaceTypes">The interface types.</param>
        /// <returns>The proxy definition.</returns>
        private static IProxyDefinition CreateProxyDefinition(Type declaringType, IEnumerable<Type> interfaceTypes)
        {
            if (declaringType.IsDelegate())
                return new DelegateProxyDefinition(declaringType, interfaceTypes);

            if (declaringType.IsInterface)
                return new InterfaceProxyDefinition(declaringType, interfaceTypes);

            return new ClassProxyDefinition(declaringType, interfaceTypes);
        }

        /// <summary>
        /// Generates a proxy template.
        /// </summary>
        /// <param name="proxyDefinition">The proxy definition.</param>
        /// <returns>The proxy template.</returns>
        private IProxyTemplate GenerateProxyTemplate(IProxyDefinition proxyDefinition)
        {
            var typeBuilder = _typeBuilderFactory.CreateBuilder(proxyDefinition.ParentType);
            var proxyGenerator = new ProxyGenerator(typeBuilder, _interceptionFilter);

            return proxyGenerator.GenerateProxyTemplate(proxyDefinition);
        }

        public bool IsProxyClass(Type type)
        {
            var proxyType = _proxyTemplateCache.Values.FirstOrDefault(p => p.ImplementationType == type);

            return proxyType != null;
        }

        #region IProxyFactory Members

        /// <inheritdoc/>
        public IProxyTemplate GetProxyTemplate(Type declaringType, IEnumerable<Type> interfaceTypes)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");

            //if (interfaceTypes == null)
            //    throw new ArgumentNullException("interfaceTypes");

            // Create proxy definition.
            var proxyDefinition = CreateProxyDefinition(declaringType, interfaceTypes);

            // Get or generate proxy template.
            return _proxyTemplateCache.GetOrAdd(proxyDefinition, GenerateProxyTemplate);
        }

        /// <inheritdoc/>
        public void SaveAssembly()
        {
            _typeBuilderFactory.SaveAssembly();
        }
        #endregion
    }
}