using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Log;

namespace NLite.Test
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void Test()
        {
           // LogManager.Instance = LogManager.Log4NetManager;

            var log = LogManager.GetLogger("LoggerTest");
            InternalTest(log);

           // log = NullLogger.Instance;
            InternalTest(log);
        }

        private static void InternalTest(ILog log)
        {
            const string message = "add record in db";


            log.Debug(message);
            log.Debug(message, new Exception());
            log.DebugFormat(message);

            log.Error(message);
            log.Error(message, new Exception());
            log.ErrorFormat(message);

            log.Fatal(message);
            log.Fatal(message, new Exception());
            


            log.Info(message);
            log.Info(message, new Exception());
            log.InfoFormat(message);

            log.Warn(message);
            log.Warn(message, new Exception());
            log.WarnFormat(message);

            Console.WriteLine(log.IsDebugEnabled);
            Console.WriteLine(log.IsErrorEnabled);
            Console.WriteLine(log.IsFatalEnabled);
            Console.WriteLine(log.IsInfoEnabled);
            Console.WriteLine(log.IsWarnEnabled);

            //log.TryLogFail(ErrorAction).Exception(e => Console.WriteLine(e.Message));
            //log.TryLogFail(SuccessAction).Success(()=>{});

        }

        static void ErrorAction()
        {
            throw new NotSupportedException();
        }

        static void SuccessAction()
        {
        }
    }

    //static class LogExtensions
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <modelExp name="log"></modelExp>
    //    /// <modelExp name="action"></modelExp>
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
