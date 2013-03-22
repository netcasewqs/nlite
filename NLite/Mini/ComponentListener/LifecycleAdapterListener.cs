using System;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class LifecycleAdapterListener : ComponentListenerAdapter
    {
        private LifecycleListnerManager Manager;

        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            base.Init();

            Kernel.Register<LifecycleListnerManager>();
            Manager = Kernel.Get<LifecycleListnerManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnPostCreation(NLite.Mini.Context.IComponentContext ctx)
        {
            var instance = ctx.Instance;
            if (instance != null && instance.GetType() != typeof(LifecycleListnerManager))
                Manager.ForEach(lisnter => lisnter.OnCreated(instance));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="instance"></param>
        public override void OnPreDestroy(IComponentInfo info, object instance)
        {
            if (instance != null && instance != Manager)
                Manager.ForEach(listner => listner.OnDestroying(instance));
        }

    }
}
