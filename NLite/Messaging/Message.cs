using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Linq;
using NLite.Threading;
using NLite.Messaging.Internal;

namespace NLite.Messaging
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 消息名称/消息主题（用以标志一个消息）
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 消息数据
        /// </summary>
        object Data { get; }
    }

    /// <summary>
    /// 消息接口
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IMessage<TData> : IMessage
    {
        /// <summary>
        ///  消息数据
        /// </summary>
        new TData Data { get; }
    }

    /// <summary>
    /// 消息请求对象接口
    /// </summary>
    public interface IMessageRequest
    {
        /// <summary>
        /// 得到或设置是否同步发布消息
        /// </summary>
        bool IsAsync { get; set; }
        /// <summary>
        /// 得到或设置消息数据
        /// </summary>
        object Data { get; set; }
        /// <summary>
        /// 得到或设置消息主题（用以标志一个消息）
        /// </summary>
        string Topic { get; set; }
        /// <summary>
        /// 得到或设置消息发送者
        /// </summary>
        object Sender { get; set; }

        /// <summary>
        /// 转换为消息对象
        /// </summary>
        IMessage ToMessage();
    }

    /// <summary>
    /// 消息请求对象接口
    /// </summary>
    /// <typeparam name="TData">泛型消息数据</typeparam>
    public interface IMessageRequest<TData> : IMessageRequest
    {
        /// <summary>
        ///  得到或设置消息数据
        /// </summary>
        new TData Data { get; set; }
    }

    /// <summary>
    /// 消息请求对象
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class MessageRequest : IMessageRequest
    {
        public MessageRequest()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 得到或设置是否同步发布消息
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Timestamp { get; private set; }
        /// <summary>
        /// 得到或设置消息发送者
        /// </summary>
        public object Sender { get; set; }
        /// <summary>
        /// 得到或设置消息主题（用以标志一个消息）
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 得到或设置消息数据
        /// </summary>
        public object Data { get; set; }
       

        /// <summary>
        /// 转换为消息对象
        /// </summary>
        public virtual IMessage ToMessage()
        {
            return MessageFactory.Make(Topic, Data, Timestamp);
        }
    }

    /// <summary>
    /// 消息请求对象
    /// </summary>
    /// <typeparam name="TData"></typeparam>
     #if !SILVERLIGHT
    [Serializable]
    #endif
    public class MessageRequest<TData> : MessageRequest
    {
        /// <summary>
        /// 得到或设置消息数据
        /// </summary>
        public new TData Data { get { return (TData)base.Data; } set { base.Data = value; } }

        /// <summary>
        /// 转换为消息对象
        /// </summary>
        /// <returns></returns>
        public override IMessage ToMessage()
        {
            return MessageFactory.Make<TData>(Topic, Data, Timestamp);
        }
    }



    /// <summary>
    /// 消息响应结果接口
    /// </summary>
    public interface IMessageResponse
    {
       
        /// <summary>
        /// 得到消息执行的结果列表
        /// </summary>
        object[] Results { get; }
        /// <summary>
        /// 得到一个布尔值用来指示是否处理完毕
        /// </summary>
        bool IsCompleted { get; }

        Exception[] Exceptions { get; }
        Exception Exception { get; }

        /// <summary>
        /// 得到或设置取消消息调用
        /// </summary>
        bool Canceled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Wait();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        bool Wait(int timeout);
    }

    /// <summary>
    /// 消息上下文接口
    /// </summary>
    public interface IMessageContext
    {
        /// <summary>
        /// 消息请求对象
        /// </summary>
        IMessageRequest Request { get; }
        /// <summary>
        /// 消息响应对象
        /// </summary>
        IMessageResponse Response { get; }
      
    }


    namespace Internal
    {
         #if !SILVERLIGHT
    [Serializable]
    #endif
        class MessageResult : BooleanDisposable, IMessageResponse, IMessageContext
        {
            private readonly object Mutex = new object();
            /// <summary>
            /// 
            /// </summary>
            internal readonly IMessageRequest Request;
            private bool isCompleted;
            private ResetEvent resetEvent;
          
            private bool canceled = false;
            public static readonly IMessageResponse Empty = new MessageResult();

            internal List<Exception> InnerExceptions = new List<Exception>();

            public MessageResult(IMessageRequest req)
            {
                Request = req;
            }

            private MessageResult()
            {
                isCompleted = true;
            }


          
            /// <summary>
            /// 得到一个布尔值用来指示是否处理完毕
            /// </summary>
            public bool IsCompleted
            {
                get { return isCompleted; }
                internal set
                {
                    lock (Mutex)
                    {
                        if (isCompleted != value)
                        {
                            isCompleted = value;
                            if (isCompleted)
                            {
                                if (resetEvent != null)
                                    resetEvent.Set();
                                resetEvent = null;
                            }
                        }
                    }
                }
            }

           


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool Wait()
            {
                return Wait(Timeout.Infinite);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public bool Wait(int timeout)
            {
                if (IsCompleted) return true;
                if (Canceled) return true;

                CheckNotDisposed();

                if (resetEvent == null)
                    resetEvent = new ResetEvent(false);
                return resetEvent.Wait(timeout);
            }

            internal List<object> results = new List<object>();

            /// <summary>
            /// 得到消息执行的结果列表
            /// </summary>
            public object[] Results
            {
                get
                {
                    Wait();
                    CheckNotDisposed();
                    return results.ToArray();
                }
            }

            public Exception[] Exceptions
            {
                get
                {
                    Wait();
                    CheckNotDisposed();
                    return InnerExceptions.ToArray();
                }
            }

            public Exception Exception
            {
                get
                {
                    Wait();
                    CheckNotDisposed();
                    return InnerExceptions.FirstOrDefault();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool Canceled
            {
                get { return canceled; }
                set
                {
                    lock (Mutex)
                    {
                        CheckNotDisposed();
                        if (canceled != value)
                        {
                            canceled = value;
                        }
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (!isCompleted)
                    {
                        resetEvent.Wait();
                    }
                }
            }

            #region IMessageContext Members

            IMessageRequest IMessageContext.Request
            {
                get { return Request; }
            }

            IMessageResponse IMessageContext.Response
            {
                get { return this; }
            }

            #endregion
        }

        

        /// <summary>
        /// 
        /// </summary>
        #if !SILVERLIGHT
    [Serializable]
    #endif
        class Message : IMessage
        {

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; private set; }


            /// <summary>
            /// 
            /// </summary>
            public DateTime Timestamp { get; internal set; }
            internal readonly object _data;
            /// <summary>
            /// 
            /// </summary>
            public object Data { get { return _data; } }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="data"></param>
            /// <param name="timestamp"></param>
            public Message(string name, object data, DateTime timestamp)
            {
                //Assumes.NotNullOrEmpty(name);
                Name = name;
                _data = data;
                Timestamp = timestamp;
            }



            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return _data != null ? _data.ToString() : null;
            }




        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        #if !SILVERLIGHT
        [Serializable]
        [DataContract]
        #endif
        class Message<TData> : Message, IMessage<TData>
        {
            /// <summary>
            /// 
            /// </summary>
            public new TData Data { get { return (TData)base._data; } }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="data"></param>
            /// <param name="timestamp"></param>
            public Message(string name, TData data, DateTime timestamp) : base(name, data, timestamp) { }
        }

        /// <summary>
        /// 
        /// </summary>
        sealed class MessageFactory
        {
            static readonly Type MessageType = typeof(IMessage);

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TData"></typeparam>
            /// <param name="name"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public static IMessage Make<TData>(string name, TData data, DateTime dateTime)
            {
                return MessageFactory.MessageType.IsAssignableFrom(GetDataType(data)) ? (IMessage)data : new Message<TData>(name, data, dateTime);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public static IMessage Make(string name, object data, DateTime dateTime)
            {
                return MessageFactory.MessageType.IsAssignableFrom(GetDataType(data)) ? (IMessage)data : new Message(name, data, dateTime);
            }

            static Type GetDataType(object data)
            {
                return data == null ? null : data.GetType();
            }

        }
    }
}
