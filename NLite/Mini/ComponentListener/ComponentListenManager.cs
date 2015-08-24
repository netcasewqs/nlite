using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Internal;
using NLite.Collections.Internal;
using NLite.Mini.Context;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class ComponentListenManager:ListenerManager<IComponentListener>,IComponentListener,IComponentListenerManager
    {

        private static readonly Type AdapterType = typeof(ComponentListenerAdapter);
        /// <summary>
        /// 
        /// </summary>
        public IKernel Kernel { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Enabled { get; set; }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="listner"></param>
        protected override void OnAfterRegister(IComponentListener listner)
        {
            listner.Init(Kernel);
            
            var methods = listner.GetType().GetMethods().Where(m=>m.DeclaringType != AdapterType);
            var type = listner.GetType();
            foreach (var m in methods)
            {
                if (m.DeclaringType != type)
                    continue;
                switch (m.Name)
                {
                    case "OnMetadataRegistering":
                        MetadataRegistering += listner.OnMetadataRegistering;
                        break;
                    case "OnMetadataRegistered":
                        MetadataRegistered += listner.OnMetadataRegistered;
                        break;
                    case "OnPostCreation":
                        PostCreation += listner.OnPostCreation;
                        break;
                    case "OnFetch":
                        Fetch += listner.OnFetch;
                        break;
                    case "OnPreCreation":
                        PreCreation += listner.OnPreCreation;
                        break;
                    case "OnInitialization":
                        Initialization += listner.OnInitialization;
                        break;
                    case "OnPostInitialization":
                        PostInitialization += listner.OnPostInitialization;
                        break;
                    case "OnPreDestroy":
                        PreDestroy += listner.OnPreDestroy;
                        break;
                    case "OnPostDestroy":
                        PostDestroy += listner.OnPostDestroy;
                        break;
                }
            }
        }

        /// <inheritdoc/>
        /// <param name="listner"></param>
        protected override void OnAfterUnRegister(IComponentListener listner)
        {
            var methods = listner.GetType().GetMethods();
            var type = listner.GetType();
            foreach (var m in methods)
            {
                if (m.DeclaringType != type)
                    continue;
                switch (m.Name)
                {
                    case "OnMetadataRegistering":
                        MetadataRegistering -= listner.OnMetadataRegistering;
                        break;
                    case "OnMetadataRegistered":
                        MetadataRegistered -= listner.OnMetadataRegistered;
                        break;
                    case "OnPostCreation":
                        PostCreation -= listner.OnPostCreation;
                        break;
                    case "OnFetch":
                        Fetch -= listner.OnFetch;
                        break;
                    case "OnPreCreation":
                        PreCreation -= listner.OnPreCreation;
                        break;
                    case "OnInitialization":
                        Initialization -= listner.OnInitialization;
                        break;
                    case "OnPostInitialization":
                        PostInitialization -= listner.OnPostInitialization;
                        break;
                    case "OnPreDestroy":
                        PreDestroy -= listner.OnPreDestroy;
                        break;
                    case "OnPostDestroy":
                        PostDestroy -= listner.OnPostDestroy;
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        public virtual void Init(IKernel kernel)
        {
            Kernel = kernel;
            if (kernel != null)
                Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init() { }


        #region IComponentListener Members
        event Func<IComponentInfo,bool> MetadataRegistering;

        bool IComponentListener.OnMetadataRegistering(IComponentInfo info)
        {
            if (Enabled && MetadataRegistering != null)
            {
                var items = MetadataRegistering.GetInvocationList();
                foreach (var item in items)
                {
                    var flag = (bool)item.DynamicInvoke(info);
                    if (!flag)
                        return false;
                }
                //MetadataRegistering(bindingInfo);
                return true;
            }

            return true;
        }
        event Action<IComponentInfo> MetadataRegistered;

        void IComponentListener.OnMetadataRegistered(IComponentInfo info)
        {
            if (Enabled && MetadataRegistered != null)
                MetadataRegistered(info);
        }

        event Action<IComponentInfo> MetadataUnregistered;
        void IComponentListener.OnMetadataUnregistered(IComponentInfo info)
        {
            if (Enabled && MetadataUnregistered != null)
                MetadataRegistered(info);
        }

        event Action<IComponentContext> PreCreation;
        void IComponentListener.OnPreCreation(NLite.Mini.Context.IComponentContext ctx)
        {
            if (Enabled && PreCreation != null)
                PreCreation(ctx);
        }

        event Action<IComponentContext> PostCreation;
        void IComponentListener.OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
        {
            if (Enabled && PostCreation != null)
                PostCreation(ctx);
        }

        event Action<IComponentContext> Initialization;
        void IComponentListener.OnInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            if (Enabled && Initialization != null)
                Initialization(ctx);
        }

        event Action<IComponentContext> PostInitialization;
        void IComponentListener.OnPostInitialization(NLite.Mini.Context.IComponentContext ctx)
        {
            if (Enabled && PostInitialization != null)
                PostInitialization(ctx);
        }

        event Action<IComponentContext> Fetch;
        void IComponentListener.OnFetch(NLite.Mini.Context.IComponentContext ctx)
        {
            if (Enabled && Fetch != null)
                Fetch(ctx);
        }

        event Action<IComponentInfo,object> PreDestroy;
        void IComponentListener.OnPreDestroy(IComponentInfo info, object instance)
        {
            if (Enabled && PreDestroy != null)
                PreDestroy(info,instance);
        }

        event Action<IComponentInfo> PostDestroy;
        void IComponentListener.OnPostDestroy(IComponentInfo info)
        {
            if (Enabled && PostDestroy != null)
                PostDestroy(info);
        }

        #endregion

    }
}
