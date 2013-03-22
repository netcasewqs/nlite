using System;

namespace NLite.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class MessageReceivingEventArgs : MessageEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="receiver"></param>
        public MessageReceivingEventArgs(IMessageContext message, object receiver)
            : base(message)
        {
            Receiver = receiver;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Receiver { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Ignored { get; set; }
    }
}
