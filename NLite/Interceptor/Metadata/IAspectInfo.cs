using System;
using System.Collections.Generic;
namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 切面接口信息定义
    /// </summary>
    public interface IAspectInfo
    {
        /// <summary>
        /// 得到切面类型
        /// </summary>
        ITargetTypeInfo TargetType { get; }
        /// <summary>
        /// 得到切点列表
        /// </summary>
        IEnumerable<ICutPointInfo> PointCuts { get; }
    }
}
