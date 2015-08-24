using System;
using NLite.Log.Internal;
using NLite.Internal;

namespace NLite.Log
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// 当前的日志管理接口
        /// </summary>
        private static ILogManager CurrentLogManager = new TraceLogManager();

        /// <summary>
        /// 设置日志管理器
        /// </summary>
        /// <param name="logManager"></param>
        public static void SetLogManager(ILogManager logManager)
        {
            Guard.NotNull(logManager, "logManager");
            LogManager.CurrentLogManager = logManager;
        }


        /// <summary>
        /// 得到一个日志记录器接口
        /// </summary>
        /// <param name="name">记录器的名称</param>
        /// <returns>一个日志记录器接口</returns>
        public static ILog GetLogger(string name)
        {
            Guard.NotNull(CurrentLogManager, "currentLogManager");
            Guard.NotNullOrEmpty(name, "name");
            return CurrentLogManager.GetLogger(name);
        }

        /// <summary>
        /// 关闭日志管理器
        /// </summary>
        public static void Shutdown()
        {
            Guard.NotNull(CurrentLogManager, "currentLogManager");
            CurrentLogManager.Shutdown();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class Log4nLogManager : ILogManager
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILog GetLogger(string name)
        {
            Guard.NotNullOrEmpty(name, "name");
            return new Log4NetLogger(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Shutdown()
        {
            var method = Log4NetLogger.LogManagerType.GetMethod("Shutdown");
            if (method != null)
            {
                try
                {
                    method.Invoke(null, null);
                }
                finally { }
            }
        }
    }
   
}
