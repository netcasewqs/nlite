
using System;
namespace NLite.Messaging
{
    /// <summary>
    /// 发布者接口
    /// </summary>
    /// <remarks>采用设计模式中的观察者模式(发布者订阅者）模式，该接口担当了抽象发布者角色</remarks>
    public interface IPublisher:IDisposable
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        IMessageResponse Publish(IMessageRequest req);

    }

    
}
