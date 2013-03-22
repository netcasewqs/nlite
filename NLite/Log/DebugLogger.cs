using System;
using System.IO;

namespace NLite.Log.Internal
{
    class DebugLogger : ILog
    {
        readonly TextWriter writer;
        internal string Name;

        public DebugLogger()
        {
        }

        public DebugLogger(TextWriter writer)
        {
            this.writer = writer;
        }



        public void Debug(object message)
        {
            if (IsDebugEnabled)
            {
                Log(LogLevel.Debug, message, null);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        public void Info(object message)
        {
            if (IsInfoEnabled)
            {
                Log(LogLevel.Info, message, null);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            Info(string.Format(format, args));
        }

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
                Log( LogLevel.Fatal, message, exception);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            Fatal(string.Format(format, args));
        }


        private void Log(LogLevel level, object message, Exception exception)
        {
            string msg = null;
            if (exception == null)
                msg = string.Format("[{0}] - [{1}] {2} {3}", DateTime.Now.ToString(), Name, level, message);
            else
                msg = string.Format("[{0}] - [{1}] {2} {3} : {4} {5}", DateTime.Now.ToString(), Name, level, exception.GetType().FullName, exception.Message, exception.StackTrace);

            if (writer != null)
                writer.WriteLine(msg);
            else
            {
                System.Diagnostics.Debug.WriteLine(msg);
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
