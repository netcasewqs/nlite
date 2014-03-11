using System;
using System.Collections.Generic;
using NLite.Internal;
using NLite.Mini.Resolving;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettingInjectionListener:ComponentListenerAdapter
    {

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            //PropertyManager.Instance.FileChanged += new EventHandler(Instance_FileChanged);
            PropertyManager.Instance.Properties.PropertyChanged += new EventHandler<System.ComponentModel.PropertyChangedEventArgs>(Properties_PropertyChanged);
        }

        void Properties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged();
        }

        Dictionary<object, IAppSettingInjection[]> resetMap = new Dictionary<object, IAppSettingInjection[]>();

        
        void Instance_FileChanged(object sender, EventArgs e)
        {
            OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            foreach (var instance in resetMap.Keys)
            {
                foreach (var item in resetMap[instance])
                    item.Inject(instance);//item.Setter(instance, Mapper.Map(GetValueByPropertyManager(item), Types.String, item.MemberType));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
        {
            if (ctx.Component.Implementation == null
                || ctx.Instance == null)
                return;

            if (!ctx.Component.ExtendedProperties.ContainsKey("AppSettings"))
                return;

            var injections = ctx.Component.ExtendedProperties["AppSettings"] as IAppSettingInjection[];

            if (injections == null || injections.Length == 0)
                return;

            var instance = ctx.Instance;
            var members = new List<IAppSettingInjection>();
            foreach (var item in injections)
            {
                item.Inject(instance);

                if (item.Reinjection)
                    members.Add(item);
            }

            if (members.Count > 0)
            {
                lock (resetMap)
                    resetMap[instance] = members.ToArray();
            }
        }

        private object GetValueByPropertyManager(ExportInfo item)
        {
            var o = PropertyManager.Instance.Properties[item.Id];
            if (o != null)
            {
                var sv = o as SerializedValue;
                if (sv != null)
                    o = sv.Deserialize(item.MemberType);

                return Mapper.Map(o, o.GetType(), item.MemberType);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="instance"></param>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            if (resetMap.ContainsKey(instance))
                lock (resetMap)
                    resetMap.Remove(instance);
        }
    }
}
