using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NLite
{
    /// <summary>
    /// 重复注册异常类
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class RepeatRegistrationException : NLiteException
    {
        /// <summary>
        /// 
        /// </summary>
        public RepeatRegistrationException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RepeatRegistrationException(string message)
            : base(message)
        {
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public RepeatRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }
}
