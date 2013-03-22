using System;
using System.Collections.Generic;
namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 切点信息
    /// </summary>
    public interface ICutPointInfo
    {
        /// <summary>
        /// 方法签名
        /// </summary>
        IMethodSignature Signature { get; }
        /// <summary>
        /// 拦截器类型集合
        /// </summary>
        IEnumerable<Type> Advices { get; }
    }
}
