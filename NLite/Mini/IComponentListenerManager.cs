
namespace NLite
{
    /// <summary>
    /// 组件监听管理器接口
    /// </summary>
    public interface IComponentListenerManager : IListenerManager<IComponentListener>
    {
        /// <summary>
        /// 初始化组件监听管理器
        /// </summary>
        /// <param name="kernel"></param>
        void Init(IKernel kernel);
        /// <summary>
        /// 是否启用监听器
        /// </summary>
        bool Enabled { get; set; }
    }
}
