using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Log;

namespace NLite.Interceptor
{
    /// <summary>
    /// 日志拦截器
    /// </summary>
    public class LoggerInterceptor : IInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        private void OnInvocationExecuting(IInvocationContext ctx)
        {
            var method = ctx.Method;
            var logger = LogManager.GetLogger(method.DeclaringType.Name);

            if (method.IsSpecialName)
            {
                Log(logger, "Invoking  property " + method.Name);
            }
            else
            {
                Type[] args = Type.GetTypeArray(ctx.Parameters);

                String argMessage = "(";
                foreach (Type arg in args)
                {
                    argMessage += " " + arg.Name;
                }
                argMessage += ")";

                Log(logger, "Invoking  method " + method.Name + argMessage);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        private void OnInvocationExecuted(IInvocationContext ctx)
        {
            var method = ctx.Method;
            var logger = LogManager.GetLogger(method.DeclaringType.Name);

            if (method.IsSpecialName)
            {
                Log(logger, "Invoked  property " + method.Name);
            }
            else
            {
                Type[] args = Type.GetTypeArray(ctx.Parameters);

                String argMessage = "(";
                foreach (Type arg in args)
                {
                    argMessage += " " + arg.Name;
                }
                argMessage += ")";

                Log(logger, "Invoked  method " + method.Name + argMessage);
            }
        }
     
        private void OnException(IInvocationContext ctx,Exception ex)
        {
            var method = ctx.Method;
            var logger = LogManager.GetLogger(method.DeclaringType.Name);

            if (method.IsSpecialName)
            {
                Log(logger, "Invoke  property " + method.Name + " exception : " + ex.Message);
            }
            else
            {
                Type[] args = Type.GetTypeArray(ctx.Parameters);

                String argMessage = "(";
                foreach (Type arg in args)
                {
                    argMessage += " " + arg.Name;
                }
                argMessage += ")";

                Log(logger, "Invoke method " + method.Name + argMessage + " exception : " + ex.Message);
            }

            logger.Error(ex.Message,ex);
        }

        private void Log(ILog logger, string message)
        {
            switch (Level)
            {
                case LogLevel.Debug:
                    logger.Debug(message);
                    break;
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(message);
                    break;
                case LogLevel.Info:
                    logger.Info(message);
                    break;
                case LogLevel.Warn:
                    logger.Warn(message);
                    break;
            }
        }

        public object Intercept(IInvocationContext invocationContext)
        {
            object result = null;
            try
            {
                OnInvocationExecuting(invocationContext);
                result = invocationContext.Proceed();
            }
            catch (Exception ex)
            {
                OnException(invocationContext, ex);
            }
            finally
            {
                OnInvocationExecuted(invocationContext);
            }

            return result;
        }
    }
}
