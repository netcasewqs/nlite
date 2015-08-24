using System;
using System.Runtime.Serialization;

namespace NLite
{
    /// <summary>
    /// 循环依赖异常
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class LoopDependencyException:NLiteException
    {
        /// <summary>
        /// 构造循环依赖异常对象
        /// </summary>
        public LoopDependencyException() { }
        /// <summary>
        /// 构造循环依赖异常对象
        /// </summary>
        /// <param name="message"></param>
       public LoopDependencyException(string message) : base(message) { }

         #if !SILVERLIGHT
        /// <summary>
       /// 构造循环依赖异常对象
        /// </summary>
       /// <param name="info"></param>
        /// <param name="context"></param>
       protected LoopDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }
}
