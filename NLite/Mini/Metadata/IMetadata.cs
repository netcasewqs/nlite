using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 元数据接口
    /// </summary>
    public interface IMetadata
    {
        /// <summary>
        /// 得到元数据名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 得到元数据Value 
        /// </summary>
        object Value { get; }
    }
}
