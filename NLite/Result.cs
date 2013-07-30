//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using NLite.Validation;

//namespace NLite
//{
//    /// <summary>
//    /// 结果对象
//    /// </summary>
//    public struct Result
//    {
//        /// <summary>
//        /// 成功结果
//        /// </summary>
//        public static Result OK { get; private set; }

//        private readonly Exception exception;
//        private readonly bool success;

//        /// <summary>
//        /// 创建结果对象
//        /// </summary>
//        /// <param name="success">是否成功</param>
//        /// <param name="ex">异常对象</param>
//        public Result(bool success, Exception ex)
//        {
//            this.success = success;
//            this.exception = ex;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="handler"></param>
//        /// <returns></returns>
//        public Result Exception(Action<Exception> handler)
//        {
//            if (!this.success)
//                handler(this.exception);
//            return this;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="handler"></param>
//        /// <returns></returns>
//        public Result Success(Action handler)
//        {
//            if (this.success)
//            {
//                handler();
//            }
//            return this;
//        }

//        static Result()
//        {
//            OK = new Result(true, null);
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public struct Result<T>
//    {
//        private readonly Exception exception;
//        private readonly bool success;
//        private readonly T parameter;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="success"></param>
//        /// <param name="ex"></param>
//        /// <param name="param"></param>
//        public Result(bool success, Exception ex, T param)
//        {
//            this.success = success;
//            this.parameter = param;
//            this.exception = success ? null : ex;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="handler"></param>
//        /// <returns></returns>
//        public Result<T> Exception(Action<Exception> handler)
//        {
//            if (!this.success)
//                handler(this.exception);
//            return this;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="handler"></param>
//        /// <returns></returns>
//        public Result<T> Success(Action<T> handler)
//        {
//            if (this.success)
//                handler(this.parameter);
//            return this;
//        }


//    }
	
//    /// <summary>
//    /// Combines a boolean succes/fail flag with a error/status message.
//    /// </summary>
//     #if !SILVERLIGHT
//    [Serializable]
//    #endif
//    public class BoolMessage
//    {
		

//        /// <summary>
//        /// Success / failure ?
//        /// </summary>
//        public bool Success { get;set;}

//        /// <summary>
//        /// Get error collection
//        /// </summary>
//        public ErrorState ErrorState { get; private set; }
		
//        /// <summary>
//        /// Get or set data
//        /// </summary>
//        public virtual object Data { get; set;}
//        /// <summary>
//        /// Get or set Execption
//        /// </summary>
//        public Exception Exception { get; set;}

//        /// <summary>
//        /// 
//        /// </summary>
//        public BoolMessage()
//        {
//            ErrorState = new ErrorState();
//        }
		
       
//    }
	
//    /// <summary>
//    /// Combines a boolean succes/fail flag with a error/status message.
//    /// </summary>
//    /// <typeparam name="TData"></typeparam>
//     #if !SILVERLIGHT
//    [Serializable]
//    #endif
//    public class BoolMessage<TData>:BoolMessage
//    {
//        /// <summary>
//        /// Get or set data
//        /// </summary>
//        public new TData Data
//        { 
//            get
//            {
//                return (TData)base.Data;
//            }
//            set
//            {
//                base.Data = value;
//            }
//        }
//    }

//}
