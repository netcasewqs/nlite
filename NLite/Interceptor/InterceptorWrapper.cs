using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Interceptor;
using System.Reflection;

namespace NLite.DynamicProxy
{
    class InterceptorWrapper : IInterceptor
    {
        private string[] methods;
        private object target;
        public InterceptorWrapper(object target, string[] methods)
        {
            this.target = target;
            this.methods  = methods;
        }

        private NLite.Interceptor.IInterceptor[] GetInterceptors(IInterceptorRepository Repository,Type type, MethodInfo methodInfo)
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

        private static MethodInfo PopulateMethod(Type type, MethodInfo m)
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
                                if (args[i].Name != args2[i].Name)
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

        public object Intercept(NLite.DynamicProxy.InvocationInfo info)
        {
            if (!methods.Contains(info.TargetMethod.Name))
            {
                return info.TargetMethod.Invoke(target, info.Arguments);
            }

            var Repository = ServiceLocator.Get<IInterceptorRepository>();

            var interceptors = GetInterceptors(Repository,target.GetType(), info.TargetMethod);
            if (interceptors.Length == 0)
            {
                return info.TargetMethod.Invoke(target, info.Arguments);
            }

            var ctx = new InvocationWrapper { innerInvocation = info, Target = target };

            try
            {
                foreach (var interceptor in interceptors)
                    interceptor.OnInvocationExecuting(ctx);

                ctx.Result = info.TargetMethod.Invoke(target, info.Arguments);
            }
            catch (Exception ex)
            {
                ctx.Exception = ex;
                foreach (var interceptor in interceptors)
                    interceptor.OnException(ctx);
            }
            finally
            {
                foreach (var interceptor in interceptors)
                    interceptor.OnInvocationExecuted(ctx);
            }

            return ctx.Result;
        }
    }
}
