using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 切点类型
    /// </summary>
    [Flags]
    public enum CutPointFlags
    {
        /// <summary>
        /// 未指定
        /// </summary>
        Unspecified,
        /// <summary>
        /// 方法
        /// </summary>
        Method = 1,
        /// <summary>
        /// 属性
        /// </summary>
        Property = 2,
        /// <summary>
        /// 只读属性
        /// </summary>
        PropertyRead = 4,
        /// <summary>
        /// 只写属性
        /// </summary>
        PropertyWrite = 8,
        /// <summary>
        /// 全部
        /// </summary>
        All = Method | CutPointFlags.Property | CutPointFlags.PropertyRead | CutPointFlags.PropertyWrite,
    }

    /// <summary>
    /// 访问权限类型
    /// </summary>
    [Flags]
    public enum AccessFlags
    {
        /// <summary>
        /// 公共的
        /// </summary>
        Public = 1,
        /// <summary>
        /// 受保护的
        /// </summary>
        Protected = 2,
        /// <summary>
        /// 全部
        /// </summary>
        All = Public | Protected,
    }
}
