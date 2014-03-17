namespace NLite.DynamicProxy
{
    public interface IProxy
    {
        IInterceptor Interceptor { get; set; }
    }
}