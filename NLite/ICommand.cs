
namespace NLite
{
    /// <summary>
    /// 无参数命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行命令方法
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// 复合命令接口
    /// </summary>
    public interface ICompositeCommand : ICommand
    {
        /// <summary>
        /// 添加命令
        /// </summary>
        /// <param name="cmd"></param>
        void Add(ICommand cmd);
    }

    /// <summary>
    /// 有参数命令接口
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ICommand<TContext>
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="ctx"></param>
        void Execute(TContext ctx);
    }

    /// <summary>
    /// 命令接口
    /// </summary>
    /// <typeparam name="TContext">命令上下文参数</typeparam>
    /// <typeparam name="TReturnValue">返回值</typeparam>
    public interface ICommand<TContext,TReturnValue>
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="ctx">命令上下文参数</param>
        /// <returns></returns>
        TReturnValue Execute(TContext ctx);
    }
}
