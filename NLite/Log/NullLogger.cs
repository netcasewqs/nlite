using System;
using System.Linq.Expressions;
using NLite.Reflection;
using System.Reflection;

namespace NLite.Log.Internal
{
    sealed class NullLogger : ILog
    {
        public static readonly NullLogger Instance = new NullLogger();

        public NullLogger()
        {
        }

        public void Debug(object message)
        {
        }

        public void Debug(object message, Exception exception)
        {
        }

        public void DebugFormat(string format, params Object[] args)
        {
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public void Info(object message)
        {
        }

        public void Info(object message, Exception exception)
        {
        }

        public void InfoFormat(string format, params Object[] args)
        {
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public void Warn(object message)
        {
        }

        public void Warn(object message, Exception exception)
        {
        }

        public void WarnFormat(string format, params Object[] args)
        {
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }

        public void Error(object message)
        {
        }

        public void Error(object message, Exception exception)
        {
        }

        public void ErrorFormat(string format, params Object[] args)
        {
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public void Fatal(object message)
        {
        }

        public void Fatal(object message, Exception exception)
        {
        }

        public void FatalFormat(string format, params Object[] args)
        {
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

    }

    class Log4NetLogger : ILog
    {

        private readonly object logger;

        internal static readonly System.Type LogManagerType;

        private static readonly Func GetLoggerByNameDelegate;
        private static readonly Func GetLoggerByTypeDelegate;

        private static readonly Getter IsErrorEnabledDelegate;
        private static readonly Getter IsFatalEnabledDelegate;
        private static readonly Getter IsDebugEnabledDelegate;
        private static readonly Getter IsInfoEnabledDelegate;
        private static readonly Getter IsWarnEnabledDelegate;

        private static readonly Proc ErrorDelegate;
        private static readonly Proc ErrorExceptionDelegate;
        private static readonly Proc ErrorFormatDelegate;

        private static readonly Proc FatalDelegate;
        private static readonly Proc FatalExceptionDelegate;

        private static readonly Proc DebugDelegate;
        private static readonly Proc DebugExceptionDelegate;
        private static readonly Proc DebugFormatDelegate;

        private static readonly Proc InfoDelegate;
        private static readonly Proc InfoExceptionDelegate;
        private static readonly Proc InfoFormatDelegate;

        private static readonly Proc WarnDelegate;
        private static readonly Proc WarnExceptionDelegate;
        private static readonly Proc WarnFormatDelegate;


        static Log4NetLogger()
        {
            try
            {
                var asm = NLite.Reflection.AssemblyLoader.Load("log4net");
                if (asm == null)
                    return;

                LogManagerType = asm.GetType("log4net.LogManager");
                if (LogManagerType == null)
                    return;

                GetLoggerByNameDelegate = LogManagerType.GetMethod("GetLogger", new Type[] { typeof(string) }).GetFunc();
                GetLoggerByTypeDelegate = LogManagerType.GetMethod("GetLogger", new Type[] { typeof(Type) }).GetFunc();

                var loggerType = LogManagerType.Assembly.GetType("log4net.ILog");

                IsErrorEnabledDelegate = loggerType.GetProperty("IsErrorEnabled").GetGetter();
                IsFatalEnabledDelegate = loggerType.GetProperty("IsFatalEnabled").GetGetter();
                IsDebugEnabledDelegate = loggerType.GetProperty("IsDebugEnabled").GetGetter();
                IsInfoEnabledDelegate = loggerType.GetProperty("IsInfoEnabled").GetGetter();
                IsWarnEnabledDelegate = loggerType.GetProperty("IsWarnEnabled").GetGetter();

                ErrorDelegate = loggerType.GetMethod("Error", new Type[] { typeof(object) }).GetProc();
                ErrorExceptionDelegate = loggerType.GetMethod("Error", new Type[] { typeof(object), typeof(Exception) }).GetProc();
                ErrorFormatDelegate = loggerType.GetMethod("ErrorFormat", new Type[] { typeof(string), typeof(object[]) }).GetProc();

                FatalDelegate = loggerType.GetMethod("Fatal", new Type[] { typeof(object) }).GetProc();
                FatalExceptionDelegate = loggerType.GetMethod("Fatal", new Type[] { typeof(object), typeof(Exception) }).GetProc();

                DebugDelegate = loggerType.GetMethod("Debug", new Type[] { typeof(object) }).GetProc();
                DebugExceptionDelegate = loggerType.GetMethod("Debug", new Type[] { typeof(object), typeof(Exception) }).GetProc();
                DebugFormatDelegate = loggerType.GetMethod("DebugFormat", new Type[] { typeof(string), typeof(object[]) }).GetProc();

                InfoDelegate = loggerType.GetMethod("Info", new Type[] { typeof(object) }).GetProc();
                InfoExceptionDelegate = loggerType.GetMethod("Info", new Type[] { typeof(object), typeof(Exception) }).GetProc();
                InfoFormatDelegate = loggerType.GetMethod("InfoFormat", new Type[] { typeof(string), typeof(object[]) }).GetProc();

                WarnDelegate = loggerType.GetMethod("Warn", new Type[] { typeof(object) }).GetProc();
                WarnExceptionDelegate = loggerType.GetMethod("Warn", new Type[] { typeof(object), typeof(Exception) }).GetProc();
                WarnFormatDelegate = loggerType.GetMethod("WarnFormat", new Type[] { typeof(string), typeof(object[]) }).GetProc();
            }
            catch
            {
            }
            finally
            {
            }
        }

        public Log4NetLogger(string name)
        {
            this.logger = GetLoggerByNameDelegate(null, name);
        }

        public Log4NetLogger(Type type)
        {
            this.logger = GetLoggerByTypeDelegate(null, type);
        }

        public bool IsErrorEnabled
        {
            get { return (bool)IsErrorEnabledDelegate(logger); }
        }

        public bool IsFatalEnabled
        {
            get { return (bool)IsFatalEnabledDelegate(logger); }
        }

        public bool IsDebugEnabled
        {
            get { return (bool)IsDebugEnabledDelegate(logger); }
        }

        public bool IsInfoEnabled
        {
            get { return (bool)IsInfoEnabledDelegate(logger); }
        }

        public bool IsWarnEnabled
        {
            get { return (bool)IsWarnEnabledDelegate(logger); }
        }

        public void Error(object message)
        {
            ErrorDelegate(logger, message);
        }

        public void Error(object message, Exception exception)
        {
            ErrorExceptionDelegate(logger, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            ErrorFormatDelegate(logger, format, args);
        }

        public void Fatal(object message)
        {
            FatalDelegate(logger, message);
        }

        public void Fatal(object message, Exception exception)
        {
            FatalExceptionDelegate(logger, message, exception);
        }

        public void Debug(object message)
        {
            DebugDelegate(logger, message);
        }

        public void Debug(object message, Exception exception)
        {
            DebugExceptionDelegate(logger, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            DebugFormatDelegate(logger, format, args);
        }

        public void Info(object message)
        {
            InfoDelegate(logger, message);
        }

        public void Info(object message, Exception exception)
        {
            InfoExceptionDelegate(logger, message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            InfoFormatDelegate(logger, format, args);
        }

        public void Warn(object message)
        {
            WarnDelegate(logger, message);
        }

        public void Warn(object message, Exception exception)
        {
            WarnExceptionDelegate(logger, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            WarnFormatDelegate(logger, format, args);
        }
    }
}
