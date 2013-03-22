
namespace NLite.Globalization
{
    /// <summary>
    /// 资源指责库接口
    /// </summary>
    public interface IResourceRepository:ILanguageChangedListner
    {
        /// <summary>
        /// 注册资源注册表
        /// </summary>
        /// <param name="key">注册表名称</param>
        /// <param name="resourceMgr">注册表</param>
        void Register(string key, IResourceRegistry resourceMgr);
        /// <summary>
        /// 得到资源注册表
        /// </summary>
        /// <param name="key">注册表名称</param>
        /// <returns></returns>
        IResourceRegistry Get(string key);
        /// <summary>
        /// 得到资源定位器
        /// </summary>
        /// <typeparam name="TResource">资源定位器名称</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        IResourceLocator<TResource> Get<TResource>(string key);
    }
}
