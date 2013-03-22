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
    public class LoggerInterceptor : DefaultInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnInvocationExecuting(IInvocationExecutingContext ctx)
        {
            var method = ctx.Method;
            var logger = LogManager.GetLogger(method.DeclaringType.Name);

            if (method.IsSpecialName)
            {
                Log(logger, "Invoking  property " + method.Name);
            }
            else
            {
                Type[] args = Type.GetTypeArray(ctx.Arguments);

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
        public override void OnInvocationExecuted(IInovacationExecutedContext ctx)
        {
            var method = ctx.Method;
            var logger = LogManager.GetLogger(method.DeclaringType.Name);

            if (method.IsSpecialName)
            {
                Log(logger, "Invoked  property " + method.Name);
            }
            else
            {
                Type[] args = Type.GetTypeArray(ctx.Arguments);

                String argMessage = "(";
                foreach (Type arg in args)
                {
                    argMessage += " " + arg.Name;
                }
                argMessage += ")";

                Log(logger, "Invoked  method " + method.Name + argMessage);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnException(IInvocationExceptionContext ctx)
        {
            var method = ctx.Method;
            var logger = LogManager.GetLogger(method.DeclaringType.Name);

            if (method.IsSpecialName)
            {
                Log(logger, "Invoke  property " + method.Name + " exception : " + ctx.Exception.Message);
            }
            else
            {
                Type[] args = Type.GetTypeArray(ctx.Arguments);

                String argMessage = "(";
                foreach (Type arg in args)
                {
                    argMessage += " " + arg.Name;
                }
                argMessage += ")";

                Log(logger, "Invoke method " + method.Name + argMessage + " exception : " + ctx.Exception.Message);
            }

            logger.Error(ctx.Exception.Message, ctx.Exception);
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


    }
}
