using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Interceptor.Metadata;

namespace NLite.Interceptor.Matcher
{
    /// <summary>
    /// 切面匹配器接口
    /// </summary>
    //[Contract]
    public interface IAspectMatcher
    {
        /// <summary>
        /// 注册类型匹配器
        /// </summary>
        /// <typeparam name="TTargetTypeInfo"></typeparam>
        /// <typeparam name="TClassMatcher"></typeparam>
        void RegisterClassMatcher<TTargetTypeInfo, TClassMatcher>()
            where TTargetTypeInfo : ITargetTypeInfo
            where TClassMatcher : IClassMatcher, new();

        /// <summary>
        /// 在指定的切面集合中匹配特定的目标类型
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="aspects">切面集合</param>
        /// <returns>返回匹配的切面集合</returns>
        IEnumerable<IAspectInfo> Match(Type targetType, IEnumerable<IAspectInfo> aspects);
    }

    /// <summary>
    /// 类型匹配器接口
    /// </summary>
    public interface IClassMatcher
    {
        /// <summary>
        /// 判断指定的切面是否匹配特定的目标类型
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="aspect">切面</param>
        /// <returns></returns>
        bool Match(Type targetType, IAspectInfo aspect);
    }

    /// <summary>
    /// 连接点匹配器/方法匹配器接口
    /// </summary>
    public interface IJoinPointMatcher
    {
        /// <summary>
        /// 计算目标方法所匹配的切点集合
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        IEnumerable<ICutPointInfo> Match(Type targetType, MethodInfo method);
    }

}
