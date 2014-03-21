namespace NLite.DynamicProxy
{
    public interface IInvocationHandler
    {
        object Invoke(InvocationInfo info);
    }
}