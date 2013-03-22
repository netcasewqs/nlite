
namespace NLite.Messaging
{
    /// <summary>
    /// 消息监听管理器接口
    /// </summary>
    public interface IMessageListenerManager : IListenerManager<IMessageListener>, IMessageListener
    {
    }
    
}
