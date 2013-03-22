using System;
using System.Collections.Generic;
namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAspectInfo
    {
        /// <summary>
        /// 
        /// </summary>
        ITargetTypeInfo TargetType { get; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<ICutPointInfo> PointCuts { get; }
    }
}
