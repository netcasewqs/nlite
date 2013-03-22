using System;
using System.Collections.Generic;
using NLite.Collections;
using NLite.Internal;
using NLite.Collections.Internal;

namespace NLite.Messaging.Internal
{
    class MessageListnerManager : ListenerManager<IMessageListener>, IMessageListenerManager, IMessageListener
    {
        //public MessageListnerManager() : base(new SyncList<IMessageListener>()) { }
       

        #region IMessageListner Members

        public void OnSending(MessageEventArgs e)
        {
            ForEach(item => item.OnSending(e));   
        }

        public void OnSent( MessageEventArgs e)
        {
            ForEach(item => item.OnSent(e));   
        }

        public void OnReceivingException(MessageExceptionEventArgs e)
        {
            ForEach(item => item.OnReceivingException(e));   
        }

        public void OnReceiving(MessageReceivingEventArgs e)
        {
            ForEach(item => item.OnReceiving(e));   
        }

        public void OnReceived(MessageEventArgs e)
        {
            ForEach(item => item.OnReceived(e));   
        }

        #endregion

        #region IListener<MessageListnerType> Members

        public MessageListnerType Type
        {
            get { return MessageListnerType.None; }
        }

        #endregion
    }
}
