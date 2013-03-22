using System;

namespace NLite.Log
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug
        /// </summary>
        Debug,
        /// <summary>
        /// Info
        /// </summary>
        Info,
        /// <summary>
        /// Warn
        /// </summary>
        Warn,
        /// <summary>
        /// Error
        /// </summary>
        Error,
        /// <summary>
        /// Fatal
        /// </summary>
        Fatal
    }

    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param id="message">消息</param>
        void Debug(object message);

        /// <summary>
        /// 记录调试信息. 
        /// </summary>
        /// <param id="message">消息</param>
        /// <param id="exception">异常</param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// 记录调试信息.
        /// </summary>
        /// <param id="format">消息格式</param>
        /// <param id="args">消息格式化的参数数组</param>
        void DebugFormat(String format, params Object[] args);

        /// <summary>
        /// 判断消息的优先级是否可以进行记录调试信息.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param id="message">消息</param>
        void Info(object message);

        /// <summary>
        /// 记录信息 
        /// </summary>
        /// <param id="message">消息</param>
        /// <param id="exception">异常</param>
        void Info(object message, Exception exception);

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param id="format">消息格式</param>
        /// <param id="args">消息格式化的参数数组</param>
        void InfoFormat(String format, params Object[] args);

        /// <summary>
        /// 判断消息的优先级是否可以进行记录信息.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param id="message">消息</param>
        void Warn(object message);

        /// <summary>
        /// 记录警告信息 
        /// </summary>
        /// <param id="message">消息</param>
        /// <param id="exception">异常</param>
        void Warn(object message, Exception exception);

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param id="format">消息格式</param>
        /// <param id="args">消息格式化的参数数组</param>
        void WarnFormat(String format, params Object[] args);

        /// <summary>
        /// 判断消息的优先级是否可以进行记录警告信息.
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param id="message">消息</param>
        void Error(object message);

        /// <summary>
        /// 记录错误信息 
        /// </summary>
        /// <param id="message">消息</param>
        /// <param id="exception">异常</param>
        void Error(object message, Exception exception);

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param id="format">消息格式</param>
        /// <param id="args">消息格式化的参数数组</param>
        void ErrorFormat(String format, params Object[] args);

        /// <summary>
        /// 判断消息的优先级是否可以进行记录错误信息.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// 记录致命错误信息
        /// </summary>
        /// <param id="message">消息</param>
        void Fatal(object message);

        /// <summary>
        /// 记录致命错误信息
        /// </summary>
        /// <param id="message">消息</param>
        /// <param id="exception">异常</param>
        void Fatal(object message, Exception exception);

        ///// <summary>
        ///// 记录致命错误信息
        ///// </summary>
        ///// <param id="format">消息格式</param>
        ///// <param id="args">消息格式化的参数数组</param>
        //void FatalFormat(String format, params Object[] args);

        /// <summary>
        /// 判断消息的优先级是否可以进行记录致命错误信息.
        /// </summary>
        bool IsFatalEnabled { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    //public static class LogExtensions
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="log"></param>
    //    /// <param name="action"></param>
    //    /// <returns></returns>
    //    public static Result TryLogFail(this ILog log, Action action)
    //    {
    //        try
    //        {
    //            if (action != null)
    //                action();
    //            return Result.OK;
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(ex.Message, ex);
    //            return new Result(false, ex);
    //        }
    //    }
    //}
}