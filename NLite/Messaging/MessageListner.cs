using System;

namespace NLite.Messaging
{
    /// <summary>
    /// 缺省消息监听器
    /// </summary>
    public class MessageListner:BooleanDisposable, IMessageListener
    {
        /// <summary>
        /// 
        /// </summary>
        public MessageListner() : this(MessageListnerType.None) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hookType"></param>
        public MessageListner(MessageListnerType hookType)
        {
            this.Type = hookType;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual MessageListnerType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnSending(MessageEventArgs e) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnSent(MessageEventArgs e) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnReceivingException(MessageExceptionEventArgs e) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnReceiving(MessageReceivingEventArgs e) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnReceived(MessageEventArgs e) { }
    }
}
