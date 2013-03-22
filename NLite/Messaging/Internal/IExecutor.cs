
namespace NLite.Messaging.Internal
{
    interface IExecutor
    {
        IDelegateInvoker DelegateInvoker { get; set; }
        IMessageListenerManager ListnerManager { get; set; }
        ISubject Subject { get; set; }
        IMessageResponse Execute();
    }
}
