
namespace NLite.Mini.Fluent
{
    /// <summary>
    /// 组件注册表达式接口
    /// </summary>
    public interface IRegistryExpression
    {
        /// <summary>
        /// 得到注册表
        /// </summary>
        IServiceRegistry Registry { get; }
    }
}
