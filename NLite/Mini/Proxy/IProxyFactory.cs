using NLite.Mini.Context;

namespace NLite.Mini.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    //[Contract]
    public interface IProxyFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="instance"></param>
        /// <param name="constructorArguments"></param>
        /// <returns></returns>
        object Create(IComponentContext ctx, object instance, params object[] constructorArguments);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool RequiresTargetInstance(IKernel kernel, IComponentInfo model);
        //bool ShouldCreateProxy(IComponentInfo model);
    }
}
