using System;

namespace NLite.Messaging
{
    /// <summary>
    /// 消息异常事件参数
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class MessageExceptionEventArgs : MessageEventArgs
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="ctx"></param>
       /// <param name="ex"></param>
        public MessageExceptionEventArgs(IMessageContext ctx, Exception ex)
            : base(ctx)
        {
            Error = ex;
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Error { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Canceled { get; set; }
    }
}
