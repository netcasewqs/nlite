using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Internal;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
     [AttributeUsage(AttributeTargets.Property
                    | AttributeTargets.Method
                    | AttributeTargets.Class
                    , AllowMultiple = true
                    , Inherited = true)]
    public class InterceptorAttribute : Attribute
    {
        public Type InterceptorType { get; private set; }
       
        /// <summary>
        ///   Constructs the InterceptorAttribute pointing to
        ///   a service
        /// </summary>
        /// <param name="interceptorType"></param>
        public InterceptorAttribute(Type interceptorType)
        {
            Guard.NotNull(interceptorType, "interceptorType");

            if (!typeof(NLite.Interceptor.IInterceptor).IsAssignableFrom(interceptorType))
            {
                throw new ArgumentException("Invalid interceptor type '" + interceptorType.FullName + "'. interceptor type must be inherite 'NLite.Interceptor.IInterceptor'!"); 
            }

            this.InterceptorType = interceptorType; ;
        }
    }
}
