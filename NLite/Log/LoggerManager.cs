using System;
using NLite.Log.Internal;
using NLite.Internal;

namespace NLite.Log
{
    /// <summary>
    /// 日志工厂类
    /// </summary>
    public class LogManager
    {
        /// <summary>
        /// 创建日志器
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public virtual ILog CreateLogger(Type type)
        {
            Guard.NotNull(type, "type");
            return new DebugLogger { Name = type.Name };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ILog CreateLogger(string name)
        {
            Guard.NotNullOrEmpty(name, "name");
            return new DebugLogger { Name = name };
        }
        /// <summary>
        /// 
        /// </summary>
        public LogManager() { }

        private static LogManager instance = new LogManager();
        
        /// <summary>
        /// 
        /// </summary>
        public static LogManager Instance
        {
            get
            {
                return instance;
            }
            set
            {
                Guard.NotNull(value, "value");
                instance = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            Guard.NotNull(type, "type");
            return Instance.CreateLogger(type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            Guard.NotNullOrEmpty(name, "name");
            return Instance.CreateLogger(name);
        }

        
    }

    /// <summary>
    /// 
    /// </summary>
    public class Log4nLogManager : LogManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override ILog CreateLogger(string name)
        {
            Guard.NotNullOrEmpty(name, "name");
            return new Log4NetLogger(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public override ILog CreateLogger(Type type)
        {
            Guard.NotNull(type, "type");
            return new Log4NetLogger(type);
        }
    }
   
}
