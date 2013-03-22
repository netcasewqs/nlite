
namespace NLite.Globalization
{
    /// <summary>
    /// 资源定位器接口
    /// </summary>
    /// <typeparam name="TResource">资源类型</typeparam>
    public interface IResourceLocator<TResource>
    {
        /// <summary>
        /// 通过资源名称得到指定的资源对象
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <returns></returns>
        TResource Get(string name);
    }
}
