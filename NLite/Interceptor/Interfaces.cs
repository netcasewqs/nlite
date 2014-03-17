using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Log;
using NLite.Collections;
using NLite.Interceptor.Fluent;
using NLite.Internal;
using NLite.Interceptor.Metadata;
using NLite.Interceptor.Internal;
using NLite.Mini.Context;
using System.Collections;

namespace NLite.Interceptor
{
    /// <summary>
    /// 拦截类型
    /// </summary>
    [Flags]
    public enum AdviceType
    {
        /// <summary>
        /// 不拦截
        /// </summary>
        None = 1,
        /// <summary>
        /// 调用前拦截
        /// </summary>
        Before = 2,
        /// <summary>
        /// 调用后拦截
        /// </summary>
        After = 4,
        /// <summary>
        /// 异常拦截
        /// </summary>
        Exception = 8,
        /// <summary>
        /// 调用前和调用后拦截
        /// </summary>
        Around = Before | After,
        /// <summary>
        /// 调用前、异常和调用后都拦截
        /// </summary>
        All = Around | Exception
    }

    /// <summary>
    /// 拦截器接口
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// 调用前拦截
        /// </summary>
        /// <param name="ctx"></param>
        void OnInvocationExecuting(IInvocationExecutingContext ctx);
        /// <summary>
        /// 调用后拦截
        /// </summary>
        /// <param name="ctx"></param>
        void OnInvocationExecuted(IInovacationExecutedContext ctx);
        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <param name="ctx"></param>
        void OnException(IInvocationExceptionContext ctx);
    }

    /// <summary>
    /// 调用上下文
    /// </summary>
    public interface IInvocationContext
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        object Target { get; }
        /// <summary>
        /// 目标方法
        /// </summary>
        MethodInfo Method { get; }
        /// <summary>
        /// 方法参数
        /// </summary>
        object[] Arguments { get; }
    }

    /// <summary>
    /// 执行前调用上下文
    /// </summary>
    public interface IInvocationExecutingContext : IInvocationContext
    {
        
    }

    /// <summary>
    /// 执行后调用上下文
    /// </summary>
    public interface IInovacationExecutedContext : IInvocationContext
    {
        /// <summary>
        /// 得到或设置返回结果
        /// </summary>
        object Result { get; set; }
    }

    /// <summary>
    /// 异常调用上下文
    /// </summary>
    public interface IInvocationExceptionContext : IInovacationExecutedContext
    {
       /// <summary>
        /// 得到或设置异常
        /// </summary>
        Exception Exception { get; set; }
        /// <summary>
        /// 得到或设置一个值，用来指示是否已经处理了异常
        /// </summary>
        bool ExceptionHandled { get; set; }
    }

    /// <summary>
    /// 缺省拦截器
    /// </summary>
    public class DefaultInterceptor : IInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnInvocationExecuting(IInvocationExecutingContext  ctx)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnInvocationExecuted(IInovacationExecutedContext ctx)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnException(IInvocationExceptionContext ctx)
        {
        }
    }

   


    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public sealed class InterceptorBroker:IInterceptor
    {
        private IInterceptorRepository Repository;

        /// <summary>
        /// 
        /// </summary>
        public InterceptorBroker()
        {
            Repository = ServiceLocator.Get<IInterceptorRepository>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public void OnInvocationExecuting(IInvocationExecutingContext ctx)
        {
            var interceptors = GetInterceptors(ctx.Target.GetType(), ctx.Method);
            if (interceptors.Length == 0)
                return;

            foreach (var interceptor in interceptors)
                interceptor.OnInvocationExecuting(ctx);
        }

        private static MethodInfo PopulateMethod(Type type,MethodInfo m)
        {
            
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
               
                foreach (var method in genericType.GetMethods())
                {
                    if (method.IsPublic && method.Name == m.Name)
                    {
                        var args = m.GetParameters();
                        var args2 = method.GetParameters();

                        if (args.Length == args2.Length)
                        {
                            bool matched = true;
                            for (var i = 0; i < args2.Length; i++)
                            {
                                if (args[i].Name != args2[i].Name )
                                {
                                    matched = false;
                                }

                                if (args[i].IsOut != args2[i].IsOut)
                                {
                                    matched = false;
                                }

                                if (args[i].IsIn != args2[i].IsIn)
                                {
                                    matched = false;
                                }
                                if (args[i].IsOptional != args2[i].IsOptional)
                                {
                                    matched = false;
                                }

                                if (args[i].IsRetval != args2[i].IsRetval)
                                {
                                    matched = false;
                                }

                            }

                            if (matched)
                                return method;
                        }
                    }
                }

                if (m.IsGenericMethod)
                    return m.GetGenericMethodDefinition();
               
                return m;
            }
            else if (m.IsGenericMethod)
                return m.GetGenericMethodDefinition();
            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public void OnInvocationExecuted(IInovacationExecutedContext ctx)
        {
            var interceptors = GetInterceptors(ctx.Target.GetType(), ctx.Method);
            if (interceptors.Length == 0)
                return;
            foreach (var interceptor in interceptors)
                interceptor.OnInvocationExecuted(ctx);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public void OnException(IInvocationExceptionContext ctx)
        {
            var interceptors = GetInterceptors(ctx.Target.GetType(), ctx.Method);
            if (interceptors.Length == 0)
                return;
            foreach (var interceptor in interceptors)
            {
                interceptor.OnException(ctx);
                if (ctx.ExceptionHandled)
                    break;
            }
        }

        private IInterceptor[] GetInterceptors(Type type , MethodInfo methodInfo)
        {

            var interceptors = Repository.Get(methodInfo);
            if (interceptors.Count == 0)
            {
                var method = PopulateMethod(type, methodInfo);
                var tmp = Repository.Get(method);
                foreach (var item in tmp)
                    interceptors.Add(item);

                method = PopulateMethod(method.DeclaringType, methodInfo);
                tmp = Repository.Get(method);
                foreach (var item in tmp)
                    if (!interceptors.Contains(item))
                        interceptors.Add(item);
            }
            

            return interceptors.ToArray();
        }
    }

    /// <summary>
    /// 切面工厂类
    /// </summary>
    public static class Aspect
    {
        /// <summary>
        /// 定义命名空间切面
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static INamespaceExpression FromNamespace(string @namespace)
        {
            var aspect = new NamespaceExpression(@namespace);
            ServiceLocator.Get<IAspectRepository>().Register(aspect.ToAspect());
            return aspect;
        }
        /// <summary>
        /// 定义类型切面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IAspectExpression For<T>()
        {
            return For(typeof(T));
        }
        /// <summary>
        /// 定义类型切面
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static IAspectExpression For(Type componentType)
        {
            var aspect = new SingleTypeExpression(componentType);
            ServiceLocator.Get<IAspectRepository>().Register(aspect.ToAspect());
            return aspect;
        }
    }


    /// <summary>
    /// 拦截器仓储接口
    /// </summary>
    //[Contract]
    public interface IInterceptorRepository
    {
        /// <summary>
        /// 得到指定方法上的所有拦截器
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        ICollection<IInterceptor> Get(MethodInfo method);
    }

    
    /// <summary>
    /// 拦截器仓储
    /// </summary>
    public class InterceptorRepository : IInterceptorRepository
    {
        private IDictionary<MethodBase, ICollection<IInterceptor>> Cache = new Dictionary<MethodBase, ICollection<IInterceptor>>();

        /// <summary>
        /// 得到指定方法上的所有拦截器
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public ICollection<IInterceptor> Get(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            ICollection<IInterceptor> interceptors;

            lock (Cache)
            {
                if (!Cache.TryGetValue(method, out interceptors))
                    Cache[method.GetBaseDefinition()] = interceptors = new List<IInterceptor>();
            }

            return interceptors;
            //return Cache.GetOrAdd(method.GetBaseDefinition(), () => new List<IInterceptor>());
        }
    }
}
