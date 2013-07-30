using System;
using System.IO;
using System.Diagnostics;

namespace NLite.Log.Internal
{
    class TraceLogManager : ILogManager
    {
        public ILog GetLogger(string name)
        {
            return new TraceLogger() { Name = name };
        }

        public void Shutdown()
        {
            Trace.Close();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    class TraceLogger : ILog
    {
        internal string Name;

        /// <summary>
        /// 
        /// </summary>
        public TraceLogger()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
            if (IsDebugEnabled)
            {
                Log(LogLevel.Debug, message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void DebugFormat(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            if (IsInfoEnabled)
            {
                Log(LogLevel.Info, message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void InfoFormat(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(object message)
        {
            Warn(message, null);
        }

        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                Log(LogLevel.Warn, message, exception);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            Warn(string.Format(format, args));
        }

        public void Error(object message)
        {
            Error(message, null);
        }

        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                Log(LogLevel.Error, message, exception);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Error(string.Format(format, args));
        }

        public void Fatal(object message)
        {
            Fatal(message, null);
        }

        public void Fatal(object message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                Log(LogLevel.Fatal, message, exception);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            Fatal(string.Format(format, args));
        }

        private string BuildLogMessage(object message, Exception exception)
        {
            if (exception == null)
            {
                return string.Format("[{0}] - [{1}] {2} ", DateTime.Now.ToString(), Name, message);
            }
            else
            {
                string msg = message as string;
                msg = msg + " " + exception.Message;
                return string.Format("[{0}] - [{1}] {2} : {3} {4}",
                    DateTime.Now.ToString(), Name, exception.GetType().FullName, msg, exception.StackTrace);
            }
        }

        private void Log(LogLevel level, object message, Exception exception)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    System.Diagnostics.Debug.WriteLine(BuildLogMessage(message, exception));
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Trace.TraceError(BuildLogMessage(message, exception));
                    break;
                case LogLevel.Info:
                    Trace.TraceInformation(BuildLogMessage(message, exception));
                    break;
                case LogLevel.Warn:
                    Trace.TraceWarning(BuildLogMessage(message, exception));
                    break;
            }
        }

        #region ILog Members


        public void Debug(object message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public void Info(object message, Exception exception)
        {
            Log(LogLevel.Info, message, exception);
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }


        public bool IsFatalEnabled
        {
            get { return true; }
        }

        #endregion
    }
}
