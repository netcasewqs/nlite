using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain
{
    /// <summary>
    /// 领域异常 
    /// </summary>
    [Serializable]
    public class DomainException : NLiteException
    {
        /// <summary>
        /// 异常编码
        /// </summary>
        public readonly int ExceptionId;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionId">异常编码</param>
        /// <param name="message">异常消息</param>
        public DomainException(int exceptionId, string message)
            : base(message)
        {
            ExceptionId = exceptionId;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="excetpionId">异常编码</param>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public DomainException(int excetpionId, string message, Exception innerException)
            : base(message, innerException)
        {
            ExceptionId = excetpionId;
        }
    }
}
