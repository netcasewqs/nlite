using System;
using NLite.Mini.Context;
using System.Collections.Generic;
using System.Reflection;
using NLite.Reflection.Internal;

namespace NLite.Mini.Listener
{
  
    
   
    /// <summary>
    /// 组件监听适配器
    /// </summary>
    public class ComponentListenerAdapter :  IComponentListener
    {
        /// <summary>
        /// 
        /// </summary>
        public IKernel Kernel { get; private set; }

        void IComponentListener.Init(IKernel kernel)
        {
            Kernel = kernel;
            if (kernel != null)
                Init();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Init() { }
        /// <summary>
        /// 在组件元数据注册前进行监听，例如Aop监听器
        /// </summary>
        /// <param name="bindingInfo"></param>
        public virtual bool OnMetadataRegistering(IComponentInfo info)
        {
            return true;
        }
        /// <summary>
        /// 在组件元数据注册后进行监听，例如Aop监听器
        /// </summary>
        /// <param name="bindingInfo"></param>
        public virtual void OnMetadataRegistered(IComponentInfo info)
        {
        }

        /// <summary>
        /// 在组件元数据注销后进行监听，例如Aop监听器
        /// </summary>
        /// <param name="bindingInfo"></param>
        public virtual void OnMetadataUnregistered(IComponentInfo info)
        {
        }

        /// <summary>
        /// 在组件创建前进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnPreCreation(IComponentContext ctx)
        {
        }

        /// <summary>
        /// 在组件创建后进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnPostCreation(IComponentContext ctx)
        {
        }

        /// <summary>
        /// 在组件创建后对组件初始化进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnInitialization(IComponentContext ctx)
        {
        }

        /// <summary>
        /// 在组件初始化后进行监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnPostInitialization(IComponentContext ctx)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnFetch(IComponentContext ctx)
        {
        }
        /// <summary>
        /// 在组件释放前进行监听
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="instance"></param>
        public virtual void OnPreDestroy(IComponentInfo info, object instance)
        {
        }

        /// <summary>
        /// 在组件释放后进行监听
        /// </summary>
        /// <param name="bindingInfo"></param>
        public virtual void OnPostDestroy(IComponentInfo info)
        {
        }
    }
}
