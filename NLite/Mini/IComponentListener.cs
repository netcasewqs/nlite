
using NLite.Mini.Context;
namespace NLite
{
    /// <summary>
    /// 组件监听器接口，在组件元数据注册，组件创建前后，组件初始化前后以及组件释放前后进行监听
    /// </summary>
    //[Contract]
    public interface IComponentListener:IListener
    {
        /// <summary>
        /// 初始化监听器
        /// </summary>
        /// <param name="kernel"></param>
        void Init(IKernel kernel);

        /// <summary>
        /// 得到内核容器对象
        /// </summary>
        IKernel Kernel { get;  }
        /// <summary>
        /// 在组件元数据注册前进行监听，例如Aop监听器
        /// </summary>
        /// <param name="info"></param>
        bool OnMetadataRegistering(IComponentInfo info);
        /// <summary>
        /// 在组件元数据注册后进行监听，例如Aop监听器
        /// </summary>
        /// <param name="info"></param>
        void OnMetadataRegistered(IComponentInfo info);

        /// <summary>
        /// 在组件元数据注销后进行监听，例如Aop监听器
        /// </summary>
        /// <param name="info"></param>
        void OnMetadataUnregistered(IComponentInfo info);

        /// <summary>
        /// 在组件创建前进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnPreCreation(IComponentContext ctx);

        /// <summary>
        /// 在组件创建后进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnPostCreation(IComponentContext ctx);

        /// <summary>
        /// 在组件创建后对组件初始化进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnInitialization(IComponentContext ctx);

        /// <summary>
        /// 在组件初始化后进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnPostInitialization(IComponentContext ctx);

        /// <summary>
        /// 在组件获取后进行监听
        /// </summary>
        /// <param name="ctx"></param>
        void OnFetch(IComponentContext ctx);

        /// <summary>
        /// 在组件释放前进行监听
        /// </summary>
        /// <param name="info"></param>
        /// <param name="instance"></param>
        void OnPreDestroy(IComponentInfo info, object instance);

        /// <summary>
        /// 在组件释放后进行监听
        /// </summary>
        /// <param name="info"></param>
        void OnPostDestroy(IComponentInfo info);
    }
}