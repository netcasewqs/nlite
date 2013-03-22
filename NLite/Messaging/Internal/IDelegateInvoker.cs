
namespace NLite.Messaging
{
    /// <summary>
    ///  委派调用器接口
    /// </summary>
    public interface IDelegateInvoker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        object Invoke<TMessage>(IObserverHandler<TMessage> handler,object sender, TMessage msg) where TMessage : IMessage;
    }

}