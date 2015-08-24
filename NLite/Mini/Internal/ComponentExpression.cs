using System;
using System.Linq;
using System.Diagnostics;
using NLite.Collections;
using NLite.Reflection;
using System.Collections.Generic;
using NLite.Mini.Fluent;
using NLite.Internal;
using NLite.Mini.Lifestyle;
using NLite.Mapping.Internal;
using NLite.Mini.Context;
using NLite.Collections.Internal;
namespace NLite.Mini.Fluent.Internal
{
    [DebuggerDisplay("{Component}/{Component.Lifestyle}")]
     #if !SILVERLIGHT
    [Serializable]
    #endif
    class ComponentRegistration
    {
        public IComponentInfo Component;
        public ILifestyleManager LifestyleManager;
        public ComponentContext ComponentContext;

        public ComponentRegistration(IComponentInfo info, ILifestyleManager lifestyleManager)
        {
            Component = info;
            LifestyleManager = lifestyleManager;
            if (!info.IsGenericType())
            {
                ComponentContext = new ComponentContext(lifestyleManager.Kernel,null) { LifestyleManager  = lifestyleManager, Component = info };
            }
        }
    }

    static class ComponentInfoExtensions
    {
        public static TMetadata GetMetadataView<TMetadata>(this IComponentInfo componentInfo)
        {
            return 
                PopulateComponentMetadata(componentInfo)
                .GetMappingView<TMetadata>();
        }

        private static IPropertySet PopulateComponentMetadata(IComponentInfo componentInfo)
        {
            object tmpMatadataProps;
            if (!componentInfo.ExtendedProperties.TryGetValue("isPopulateMetadata", out tmpMatadataProps))
            {
                var mappings = componentInfo.Implementation.GetMappings();
                componentInfo.ExtendedProperties["isPopulateMetadata"] = mappings;
                return mappings;
            }

            return tmpMatadataProps as IPropertySet;
        }

        public static object GetMetadataView(this IComponentInfo componentInfo, Type viewType)
        {
            return 
                PopulateComponentMetadata(componentInfo)
                .GetMappingView(viewType);
        }
    }

     #if !SILVERLIGHT
    [Serializable]
    #endif
    class ComponentExpression : IComponentExpression, IComponentInfo
    {
        public string Id { get; internal set; }
        public Type[] Contracts { get { return contracts.ToArray(); } }
        public Type Implementation { get; private set; }

        public string Activator { get; set; }
        public LifestyleFlags Lifestyle { get; set; }
        public IDictionary<string, object> ExtendedProperties
        {
            get
            {
                return extendedProperties;
            }
        }
        public Func<object> Factory { get; private set; }

        public IServiceRegistry Registry { get; set; }

        private HashSet<Type> contracts;

        //private volatile bool isPopulateMetadata = false;
        private IDictionary<string, object> extendedProperties = new Dictionary<string,object>();

        public ComponentExpression()
        {
            Activator = ActivatorType.Default;
            Lifestyle = LifestyleType.Default;
            contracts = new HashSet<Type>();
        }



        public IComponentExpression Named(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id == null");
            Id = id;

            return this;
        }


        #region INamedExpression Members

        public IComponentExpression Bind(Type serive)
        {
            contracts.Add(serive);
            ContractService.GetContracts(serive,contracts);
            return this;
        }

        public IComponentExpression Bind<TService>()
        {
            return Bind(typeof(TService));
        }


        public IComponentExpression Bind(Type serive, string id)
        {
            contracts.Add(serive);
            ContractService.GetContracts(serive, contracts);
            return Named(id);
        }

        public IComponentExpression Bind<TService>(string id)
        {
            return Bind(typeof(TService), id);
        }

        #endregion

        #region IServiceExpression Members

        public IComponentExpression To(Type type)
        {
            Trace.Assert(type != null, "implemention == null");
            Implementation = type;
            ContractService.GetContracts(type, contracts);
            if (contracts.Count == 0)
                contracts.Add(type);
            return this;
        }

        public IComponentExpression To<TImplementation>()
        {

            return To(typeof(TImplementation));
        }

     

        #endregion

        #region IImplementationExpression Members
        IComponentExpression IComponentExpression.Lifestyle(LifestyleFlags lifestyle)
        {
            Lifestyle = lifestyle;
            return this;
        }

        IComponentExpression IComponentExpression.Activator(string activator)
        {
            Activator = activator;
            return this;
        }

        public IComponentExpression Add(string key, object value)
        {
            ExtendedProperties[key] = value;
            return this;
        }

        #endregion

        

        public override bool Equals(object obj)
        {
            return Equals((IComponentInfo)obj);
        }

        public bool Equals(IComponentInfo other)
        {
            if (other == null)
                return false;
            if (Implementation != other.Implementation)
                return false;
            if (this.Factory != other.Factory)
                return false;
            if (this.Activator != other.Activator)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #region IFactoryExpression Members

      
        //IFactoryExpression IFactoryExpression.Lifestyle(LifestyleFlags lifestyle)
        //{
        //    Lifestyle = lifestyle;
        //    return this;
        //}



        //IFactoryExpression IFactoryExpression.Add(string key, object value)
        //{
        //    ExtendedProperties[key]= value;
        //    return this;
        //}

        #endregion


        IComponentExpression IComponentExpression.Factory(Func<object> factory)
        {
            this.Factory = factory;
            Activator = ActivatorType.Factory;
            return this;
        }
    }

}
