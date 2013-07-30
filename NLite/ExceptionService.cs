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


        ///// <summary>
        ///// Gets or sets exception handled
        ///// </summary>
        //public bool ExceptionHandled
        //{
        //    get;
        //    set;
        //}


    }

    /// <summary>
    /// 异常代码字典接口
    /// </summary>
    //[Contract]
    public interface IExceptionCode
    {
        /// <summary>
        /// 成功的代码编号
        /// </summary>
        int Success { get; }
        /// <summary>
        /// 未知异常的代码编号
        /// </summary>
        int UnknowExceptionCode { get; }

        /// <summary>
        /// Db异常范围-开始代码编号
        /// </summary>
        int DbExceptionStart { get; }
        /// <summary>
        /// Db异常范围-结束代码编号
        /// </summary>
        int DbExceptionEnd { get; }
        /// <summary>
        /// 查询异常的代号
        /// </summary>
        int QueryException { get; }
        /// <summary>
        /// 持久化（增删改）的异常代码编号
        /// </summary>
        int PersistenceException { get; }
        /// <summary>
        /// 添加的异常代码编号
        /// </summary>
        int InsertException { get; }
        /// <summary>
        /// 删除的异常代码编号
        /// </summary>
        int DeleteException { get; }
        /// <summary>
        /// 更新的异常代码编号
        /// </summary>
        int UpdateException { get; }

        /// <summary>
        /// 领域服务的异常范围-开始代码编号
        /// </summary>
        int DomainExceptionStart { get; }
        /// <summary>
        /// 领域服务的异常范围-结束代码编号
        /// </summary>
        int DomainExceptionEnd { get; }

        /// <summary>
        /// 服务分发器异常范围：从10000开始
        /// </summary>
        int ServiceDispatcherExceptionStart { get; }

        /// <summary>
        /// 服务分发器异常范围：以19999结束
        /// </summary>
        int ServiceDispatcherExceptionEnd { get; }

        /// <summary>
        /// 自定义的异常范围-开始代码编号
        /// </summary>
        int CustomExceptionStart { get; }
        /// <summary>
        /// 自定义的异常范围-结束代码编号
        /// </summary>
        int CustomExceptionEnd { get; }
    }

    /// <summary>
    ///  异常代码字典
    /// </summary>
    public class ExceptionCode : IExceptionCode
    {
        /// <summary>
        /// 成功的代码编号，缺省是1
        /// </summary>
        public virtual int Success
        {
            get { return 1; }
        }

        /// <summary>
        /// 未知异常的代码编号，缺省是-1
        /// </summary>
        public virtual int UnknowExceptionCode
        {
            get { return -1; }
        }


        /// <summary>
        /// Db异常范围-20000开始
        /// </summary>
        public virtual int DbExceptionStart
        {
            get { return 20000; }
        }

        /// <summary>
        /// Db异常范围-29999结束
        /// </summary>
        public virtual int DbExceptionEnd
        {
            get { return 29999; }
        }

        /// <summary>
        /// 查询异常代码：20001
        /// </summary>
        public virtual int QueryException
        {
            get { return 20001; }
        }

        /// <summary>
        /// 持久化异常代码：20002
        /// </summary>
        public virtual int PersistenceException
        {
            get { return 20002; }
        }

        /// <summary>
        /// 添加异常代码：20003
        /// </summary>
        public virtual int InsertException
        {
            get { return 20003; }
        }

        /// <summary>
        /// 删除异常代码：20004
        /// </summary>
        public virtual int DeleteException
        {
            get { return 20004; }
        }

        /// <summary>
        /// 更新异常代码：20005
        /// </summary>
        public virtual int UpdateException
        {
            get { return 20005; }
        }

        /// <summary>
        /// 领域异常范围-30000开始
        /// </summary>
        public virtual int DomainExceptionStart
        {
            get { return 30000; }
        }

        /// <summary>
        /// 领域异常范围-59999结束
        /// </summary>
        public virtual int DomainExceptionEnd
        {
            get { return 59999; }
        }

        /// <summary>
        /// 服务分发器异常范围：从10000开始
        /// </summary>
        public virtual int ServiceDispatcherExceptionStart
        {
            get { return 10000; }
        }

        /// <summary>
        /// 服务分发器异常范围：以19999结束
        /// </summary>
        public virtual int ServiceDispatcherExceptionEnd
        {
            get { return 19999; }
        }

        /// <summary>
        /// 自定义异常范围-60000开始
        /// </summary>
        public virtual int CustomExceptionStart
        {
            get { return 60000; }
        }

        /// <summary>
        /// 自定义异常范围-99999结束
        /// </summary>
        public virtual int CustomExceptionEnd
        {
            get { return 99999; }
        }
    }
    ///// <summary>
    ///// 异常的扩展类
    ///// </summary>
    //public static class ExceptionService
    //{
    //    /// <summary>
    //    /// 处理异常
    //    /// </summary>
    //    /// <param name="ex"></param>
    //    public static Exception Handle(this Exception ex)
    //    {
    //        return ExceptionManager.HandleException(ex);
    //    }

    //    /// <summary>
    //    /// 处理异常
    //    /// </summary>
    //    /// <typeparam name="TNewExcption"></typeparam>
    //    /// <param name="ex"></param>
    //    /// <param name="message"></param>
    //    public static TNewExcption Handle<TNewExcption>(this Exception ex, string message) where TNewExcption : Exception, new()
    //    {
    //        return ExceptionManager.HandleAndWrapper<TNewExcption>(ex, message);
    //    }

    //    /// <summary>
    //    /// 处理异常
    //    /// </summary>
    //    /// <typeparam name="TNewExcption"></typeparam>
    //    /// <param name="ex"></param>
    //    public static TNewExcption Handle<TNewExcption>(this Exception ex) where TNewExcption : Exception, new()
    //    {
    //        return ExceptionManager.HandleAndWrapper<TNewExcption>(ex, ex.Message);
    //    }
    //}

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