using System;
using System.Runtime.Serialization;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// 
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public class ActivatorException : NLiteException
    {
        /// <summary>
        /// 
        /// </summary>
        public ActivatorException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ActivatorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ActivatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="context"></param>
        protected ActivatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }
}
