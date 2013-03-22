using System;

namespace NLite.Messaging
{
    /// <summary>
    /// 消息监听器监听类型
    /// </summary>
    [Flags]
    public enum MessageListnerType : uint
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        ReceivingException = 1,

     
        /// <summary>
        /// 
        /// </summary>
        Sending = ReceivingException * 2,

        /// <summary>
        /// 
        /// </summary>
        Sent = Sending * 2,

        /// <summary>
        /// 
        /// </summary>
        Send = Sending | Sent,

        /// <summary>
        /// 
        /// </summary>
        Receiving = Sent * 2,

        /// <summary>
        /// 
        /// </summary>
        Received = Receiving * 2,

        /// <summary>
        /// 
        /// </summary>
        Receive = Receiving | Received,

        /// <summary>
        /// 
        /// </summary>
        All = ReceivingException | Send | Receive
    }
}
