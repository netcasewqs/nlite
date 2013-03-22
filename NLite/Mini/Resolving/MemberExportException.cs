using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NLite.Mini.Resolving
{
    class MemberExportException : Exception
    {
          /// <summary>
        /// 
        /// </summary>
        public MemberExportException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MemberExportException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MemberExportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="context"></param>
        protected MemberExportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }
}
