using System;
using System.Configuration;
using System.Linq;
using NLite.Internal;

namespace NLite.Mini.Resolving
{
    class AppSettingDependency : DependencyBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override event Action OnRefresh;

        public readonly string Name;
        public bool Reinjection;
        public AppSettingDependency(string name, Type dependencyType)
        {
            Name = name;
            DependencyType = dependencyType;
            HasDependencied = true;

            ValueProvider = () =>
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(name))
                {
                    return Mapper.Map(ConfigurationManager.AppSettings[name], Types.String, dependencyType);
                }
                else if (PropertyManager.Instance.Properties.Contains(name))
                {
                    Reinjection = true;
                    return Mapper.Map(GetValueByPropertyManager(), Types.String, DependencyType);
                }
                return null;
            };
        }

        private object GetValueByPropertyManager()
        {
            var o = PropertyManager.Instance.Properties[Name];
            if (o != null)
            {
                var sv = o as SerializedValue;
                if (sv != null)
                    o = sv.Deserialize(DependencyType);

                return Mapper.Map(o, o.GetType(), DependencyType);
            }

            return null;
        }
    }
    
    
}
