using System;
using System.Collections.Generic;

namespace NLite.Messaging.Internal
{
    /// <summary>
    /// 订阅者接口
    /// </summary>
    /// <remarks>采用设计模式中的观察者模式(发布者订阅者）模式，该接口担当了抽象订阅者角色</remarks>
    interface ISubscriber
    {
        /// <summary>
        /// 主题名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 是否异步订阅
        /// </summary>
        bool IsAsync { get; }
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="handler">消息处理器</param>
        /// <param name="mode"></param>
        /// <returns>返回一个可注销订阅者的对象</returns>
        IDisposable Subscribe(SubscribeMode mode, IObserverHandler<IMessage> handler);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        void Unsubscribe(IObserverHandler<IMessage> handler);

     
        IMessageListenerManager ListnerManager { get; set; }
        IDictionary<IObserverHandler<IMessage>,SubscribeMode> Observers { get; }
      

        void Clear();
       
    }
    
}
