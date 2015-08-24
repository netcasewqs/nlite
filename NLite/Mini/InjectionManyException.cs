using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NLite.Mini
{
    /// <summary>
    /// 批量注入异常
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class InjectManyException : NLiteException
    {
         /// <summary>
        /// 
        /// </summary>
        public InjectManyException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InjectManyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InjectManyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InjectManyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }
}
