using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using NLite.Interceptor.Metadata;

namespace NLite.Interceptor.Fluent
{
    /// <summary>
    /// 切面表达式
    /// </summary>
    public interface IAspectExpression
    {
        /// <summary>
        /// 得到切点表达式
        /// </summary>
        /// <returns></returns>
        IPointCutExpression PointCut();
        /// <summary>
        /// 得到切点表达式
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        IPointCutExpression PointCut(CutPointFlags flags);
        /// <summary>
        /// 得到切面表达式
        /// </summary>
        /// <returns></returns>
        IAspectInfo ToAspect();
    }
    /// <summary>
    /// 命名空间表达式
    /// </summary>
    public interface INamespaceExpression : IAspectExpression
    {
        /// <summary>
        /// 排除特定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        INamespaceExpression Exclude<T>();
    }

    /// <summary>
    /// 切点表达式
    /// </summary>
    public interface IPointCutExpression : IAdviceExpression
    {
        /// <summary>
        /// 匹配特定的方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        IMethodSignatureExpression Method(string methodName);
        /// <summary>
        /// 匹配特定的方法
        /// </summary>
        /// <returns></returns>
        IMethodSignatureExpression Method();
    }

    /// <summary>
    /// 方法签名表达式
    /// </summary>
    public interface IMethodSignatureExpression : IAdviceExpression
    {
        /// <summary>
        /// 方法深度
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        IMethodSignatureExpression Deep(int deep);
        /// <summary>
        /// 方法参数
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        IMethodSignatureExpression Argument(string argument);
        /// <summary>
        /// 方法访问权限
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        IMethodSignatureExpression Access(AccessFlags access);
        /// <summary>
        /// 方法返回值类型
        /// </summary>
        /// <param name="methodType"></param>
        /// <returns></returns>
        IMethodSignatureExpression ReturnType(string methodType);
    }

   
    /// <summary>
    /// 拦截器表达式
    /// </summary>
    public interface IAdviceExpression
    {
        /// <summary>
        /// 注册特定的拦截器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IAdviceExpression Advice<T>();
    }

}
