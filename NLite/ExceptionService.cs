using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using NLite.Collections;
using NLite.Log;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NLite.Internal;
using System.Diagnostics;
using NLite.Threading;

namespace NLite
{
    /// <summary>
    /// Represents errors that occur in the framework.
    /// </summary>
     #if !SILVERLIGHT
    [Serializable]
    #endif
    public class NLiteException:Exception 
    {
        /// <summary>
        /// 
        /// </summary>
        public ErrorState ErrorState { get; private set; }

         #region Ctor
        /// <summary>
        /// Initializes a new instance of the <c>NLiteException</c> class.
        /// </summary>
        public NLiteException() : base() { ErrorState = new ErrorState(); }
        /// <summary>
        /// Initializes a new instance of the <c>NLiteException</c> class with the specified
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NLiteException(string message) : base(message) { ErrorState = new ErrorState(); }
        
        /// <summary>
        /// Initializes a new instance of the <c>NLiteException</c> class with the specified
        /// error message and the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception.</param>
        public NLiteException(string message, Exception innerException)
            : base(message, innerException)
        {
            var nliteException = innerException as NLiteException;
            ErrorState = nliteException == null ? new ErrorState() : nliteException.ErrorState;
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected NLiteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
        #endregion

    }

  

    /// <summary>
    /// 异常处理器接口
    /// </summary>
    internal interface IExceptionHandler
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex">将要被处理的异常</param>
        /// <param name="customInformation">自定义的异常信息</param>
        void HandleException(Exception ex, string customInformation = null);
    }

    /// <summary>
    /// 异常解析器接口
    /// </summary>
    public interface IExceptionResolver
    {
        /// <summary>
        /// 获取/设置解析器的顺序号。
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 获取/设置异常呈现器集合
        /// </summary>
        IExceptionRender[] ExceptionRenders { get; set; }

        /// <summary>
        /// 是否支持特定类型异常的解析
        /// </summary>
        /// <param name="ex">特定的异常</param>
        /// <returns>ture表示支持，false表示不支持</returns>
        bool IsSupport(Exception ex);

        /// <summary>
        /// 处理指定的异常
        /// </summary>
        /// <param name="ex">将要被处理的异常</param>
        /// <param name="customInformation">自定义消息</param>
        void HandleException(Exception ex, string customInformation = null);
    }

    /// <summary>
    /// 异常呈现器
    /// </summary>
    public interface IExceptionRender
    {
        /// <summary>
        /// 呈现异常
        /// </summary>
        /// <param name="ex">将要被呈现的异常</param>
        /// <param name="customInformation"></param>
        void RenderException(Exception ex, string customInformation = null);
    }



    /// <summary>
    /// 异常处理器
    /// </summary>
    internal class ExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// 异常解析器集合
        /// </summary>
        private IExceptionResolver[] _resolvers;

        public ExceptionHandler()
        {
            Init(new[] { new UnknowExceptionResolver() }
                 , new[] { new LogExceptionRender() });
        }

        /// <summary>
        /// 初始化异常处理器
        /// </summary>
        /// <param name="resolvers">异常解析器集合</param>
        /// <param name="renders">异常呈现器集合</param>
        internal void Init(IExceptionResolver[] resolvers, IExceptionRender[] renders)
        {
            _resolvers = resolvers;

            resolvers.Where(p => p.ExceptionRenders == null || p.ExceptionRenders.Length == 0)
                .ToList()
                .ForEach(p => p.ExceptionRenders = renders);
        }

        /// <summary>
        /// 处理异常，按照解析器的Order从小到大进行排序然后依次处理
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="customInformation"></param>
        public void HandleException(Exception ex, string customInformation = null)
        {
            if (_resolvers == null || _resolvers.Length == 0)
            {
                throw new Exception("No any exceptionResolver");
            }

            var resolver = _resolvers.OrderBy(p => p.Order).FirstOrDefault(p => p.IsSupport(ex));
            if (resolver != null)
            {
                try
                {
                    resolver.HandleException(ex, customInformation);
                }
                catch
                {
                    Trace.TraceError(ex.Message + "\t" + ex.StackTrace);
                }
            }
        }
    }

    /// <summary>
    /// 异常解析器基类
    /// </summary>
    public abstract class ExceptionResolver : IExceptionResolver
    {

        /// <summary>
        /// 获取/设置异常呈现器集合
        /// </summary>
        public IExceptionRender[] ExceptionRenders { get; set; }

        /// <summary>
        /// 获取/设置异常解析器的序号
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否支持特定类型异常的解析
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns>ture表示支持，false表示不支持</returns>
        public abstract bool IsSupport(Exception ex);

        /// <summary>
        /// 解析异常
        /// </summary>
        /// <param name="ex">将要被解析的异常</param>
        /// <param name="customInformation">自定义信息</param>
        protected abstract void OnResolve(Exception ex, string customInformation = null);

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex">将要被处理的异常</param>
        /// <param name="customInformation">自定义信息</param>
        public void HandleException(Exception ex, string customInformation = null)
        {
            if (ExceptionRenders == null || ExceptionRenders.Length == 0)
            {
                throw new ArgumentNullException("ExceptionRender");
            }

            OnResolve(ex, customInformation);
        }

        /// <summary>
        /// 呈现异常
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="customInformation">自定义信息</param>
        protected void RenderException(Exception ex, string customInformation = null)
        {
            ExceptionRenders.ToList().ForEach(p => p.RenderException(ex, customInformation));
        }
    }

    /// <summary>
    /// 异常管理器
    /// </summary>
    public static class ExceptionManager
    {
        /// <summary>
        /// 异常处理器
        /// </summary>
        private static ExceptionHandler ExceptionHandler;

        /// <summary>
        /// 最后一次成功处理的异常
        /// </summary>
        [ThreadStatic]
        private static Exception LastException;

        /// <summary>
        /// 设置异常处理器
        /// </summary>
        ///<param name="resolvers">异常解析器集合</param>
        ///<param name="renders">异常呈现器集合</param>
        public static void Init(IExceptionResolver[] resolvers, IExceptionRender[] renders)
        {
            Guard.NotNull(resolvers, "resolvers");
            Guard.NotNull(renders, "renders");

            ExceptionHandler = new ExceptionHandler();
            ExceptionHandler.Init(resolvers, renders);
        }


        /// <summary>
        /// 处理指定的异常
        /// </summary>
        /// <param name="ex">将要被处理的异常对象</param>
        /// <param name="customInformation">自定义异常消息</param>
        public static Exception Handle(Exception ex, string customInformation = null)
        {
           

            if (ExceptionHandler != null)
            {
                if (LastException != ex)
                {
                    ExceptionHandler.HandleException(ex, customInformation);
                    LastException = ex; ;
                }
            }

            return ex;
        }
    }

    /// <summary>
    /// 未知异常解析器
    /// </summary>
    public sealed class UnknowExceptionResolver : ExceptionResolver
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UnknowExceptionResolver()
        {
            Order = int.MaxValue;
        }

        /// <summary>
        /// 是否支持指定的异常类型
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override bool IsSupport(Exception ex)
        {
            // 支持所有的异常
            return true;
        }

        /// <summary>
        /// 解析异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="customInformation"></param>
        protected override void OnResolve(Exception ex, string customInformation = null)
        {
            RenderException(ex, customInformation);
        }
    }

    /// <summary>
    /// 日志异常呈现器
    /// </summary>
    public class LogExceptionRender : IExceptionRender
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogExceptionRender).Name);

        /// <summary>
        /// 呈现异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="customInformation"></param>
        public void RenderException(Exception ex, string customInformation = null)
        {
            log.Error(customInformation ?? ex.Message, ex);
        }
    }

   
}