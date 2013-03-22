using System;


namespace NLite.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(
                   AttributeTargets.Method
                  , AllowMultiple = false
                  , Inherited = true)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// 得到或设置订阅模式
        /// </summary>
        public SubscribeMode Mode { get; set; }

        /// <summary>
        /// 得到或设置消息主题
        /// </summary>
        public string Topic { get; set; }
    }

    /// <summary>
    /// 消息订阅模式
    /// </summary>
    [Flags]
    public enum SubscribeMode
    {
        /// <summary>
        /// 同步订阅
        /// </summary>
        Sync,

        /// <summary>
        /// 异步订阅
        /// </summary>
        Async,

        /// <summary>
        /// 并行订阅
        /// </summary>
        Parall,
    }

    /// <summary>
    /// 订阅信息
    /// </summary>
    public interface ISubscribeInfo
    {
        /// <summary>
        /// 主题
        /// </summary>
        string Topic { get; }
        /// <summary>
        /// 订阅模式
        /// </summary>
        SubscribeMode Mode { get; }
        /// <summary>
        /// 消息数据类型
        /// </summary>
        Type MessageType { get; }
        /// <summary>
        /// 订阅处理器
        /// </summary>
        IObserverHandler<IMessage> Handler { get; }
    }

    /// <summary>
    ///  订阅信息
    /// </summary>
    public class SubscribeInfo : ISubscribeInfo
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 订阅模式
        /// </summary>
        public SubscribeMode Mode { get; set; }
        /// <summary>
        /// 消息数据类型
        /// </summary>
        public Type MessageType { get;private  set;}
        /// <summary>
        /// 订阅处理器
        /// </summary>
        public IObserverHandler<IMessage> Handler { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SubscribeInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="handler"></param>
        public SubscribeInfo(Type messageType, IObserverHandler<IMessage> handler)
        {
            MessageType = messageType;
            Handler = handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(IObserverHandler<IMessage> handler) : this(null, handler) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Func<object> handler):this(ObserverHandler.CreateA<object>(handler))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Action handler):this(ObserverHandler.Create(handler)) 
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubscribeInfo<T> : SubscribeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Func<T> handler):base(ObserverHandler.CreateA(handler))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Func<object, T, object> handler):base(typeof(T), ObserverHandler.CreateC<T,object>(handler))
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Func<T, object> handler):base(typeof(T), ObserverHandler.CreateB<T, object>(handler))
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Action<object, T> handler):base(typeof(T),ObserverHandler.Create2<T>(handler))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Action<T> handler):base(typeof(T),ObserverHandler.Create1<T>(handler))
        {
        }
    }

    /// <summary>
    /// 订阅信息
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SubscribeInfo<TMessage, TResult> : SubscribeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Func<object, TMessage, TResult> handler):base(typeof(TMessage),ObserverHandler.CreateC<TMessage, TResult>(handler))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SubscribeInfo(Func<TMessage, TResult> handler):base(typeof(TMessage), ObserverHandler.CreateB<TMessage, TResult>(handler))
        {
        }
    }


   

    /// <summary>
    /// 消息总线接口
    /// </summary>
    //[Contract]
    public interface IMessageBus :IPublisher, IDisposable
    {
        /// <summary>
        /// 订阅主题消息
        /// </summary>
        /// <returns></returns>
        IDisposable Subscribe(ISubscribeInfo info);
        
        /// <summary>
        /// 注销订阅的主题消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        void Unsubscribe(string topic, Type messageType, IObserverHandler<IMessage> handler);

        /// <summary>
        /// 注册消息监听器
        /// </summary>
        /// <param name="listeners">监听器集合</param>
        void RegisterListner(params IMessageListener[] listners);
        /// <summary>
        /// 注销消息监听器
        /// </summary>
        /// <param name="listeners">监听器集合</param>
        void UnRegisterListner(params IMessageListener[] listners);

      
        /// <summary>
        /// 移除主题消息
        /// </summary>
        /// <param name="topic">消息主题</param>
        /// <param name="binderType">类型</param>
        void Remove(string topic, Type type);
        
        /// <summary>
        /// 移除所有主题消息
        /// </summary>
        void RemoveAll();
    }
}