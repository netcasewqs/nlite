
namespace NLite
{
    /// <summary>
    /// 组件生命周期监听器接口
    /// </summary>
    public interface ILifecycleListner:IListener
    {
        /// <summary>
        /// 监听组件被创建
        /// </summary>
        /// <param name="o">组件</param>
        void OnCreated(object o);

        /// <summary>
        /// 监听组件被释放
        /// </summary>
        /// <param name="o">组件</param>
        void OnDestroying(object o);
    }

    /// <summary>
    /// 件生命周期监听管理器接口
    /// </summary>
    //[Contract]
    public interface ILifecycleListnerManager:IListenerManager<ILifecycleListner>
    {

    }
}
