using System;
using System.Linq;
using System.Collections.Generic;
namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 方法签名
    /// </summary>
    public interface IMethodSignature
    {
        /// <summary>
        /// 方法深度
        /// </summary>
        int Deep { get; }

        /// <summary>
        /// 访问权限
        /// </summary>
        AccessFlags Access { get; }

        /// <summary>
        /// 方法参数
        /// </summary>
        IEnumerable<string> Arguments { get; }

        /// <summary>
        /// 切点类型
        /// </summary>
        CutPointFlags Flags { get; }

        /// <summary>
        /// 方法名
        /// </summary>
        string Method { get; }

        /// <summary>
        /// 返回类型
        /// </summary>
        string ReturnType { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MethodSignatureExtensions
    {
        /// <summary>
        /// 全部参数
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool AllArguments(this IMethodSignature signature)
        {
            return signature.Arguments == null || signature.Arguments.Count() == 0;
        }

        /// <summary>
        /// 全部方法
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool AllMethods(this IMethodSignature signature)
        {
            return string.IsNullOrEmpty(signature.Method) || signature.Method.Trim() == "*"; 
        }

        /// <summary>
        /// 全部访问权限
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static bool AllAccess(this IMethodSignature signature)
        {
            return string.IsNullOrEmpty(signature.Method) || signature.Method.Trim() == "*"; 
        }
    }
}
