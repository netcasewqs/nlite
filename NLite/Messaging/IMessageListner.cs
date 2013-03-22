using System;

namespace NLite.Messaging
{
    /// <summary>
    /// 消息监听器接口
    /// </summary>
    public interface IMessageListener:IListener,IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void OnSending(MessageEventArgs e);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void OnSent(MessageEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void OnReceivingException(MessageExceptionEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void OnReceiving(MessageReceivingEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void OnReceived(MessageEventArgs e);
    }
}
