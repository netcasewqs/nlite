using System;
using System.Runtime.Serialization;

namespace NLite.Globalization
{
    /// <summary>
    /// 
    /// </summary>
    #if !SILVERLIGHT
    [Serializable()]
    #endif
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        public ResourceNotFoundException(string resource)
            : base("Resource not found : " + resource)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ResourceNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <param name="context"></param>
        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }
}
