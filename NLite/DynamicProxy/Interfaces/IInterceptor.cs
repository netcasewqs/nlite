namespace NLite.DynamicProxy
{
    public interface IInterceptor
    {
        object Intercept(InvocationInfo info);
    }
}