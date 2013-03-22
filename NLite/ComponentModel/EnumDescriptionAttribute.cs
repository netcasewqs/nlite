using System;
using System.Diagnostics;

namespace NLite
{
    /// <summary>
    /// Enum 标签类
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [DebuggerDisplay("Name={Name},Value={Value},Description={Description}")]
    public class EnumAttribute : Attribute
    {
        
        /// <summary>
        /// 得到或设置枚举项缺省描述
        /// </summary>
        public string DefaultDescription { get; set; }
        /// <summary>
        /// 得到或设置枚举项序号
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 得到或设置资源Key
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// 得到枚举项名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 得到枚举项的值
        /// </summary>
        public Enum OriginalValue { get; internal set; }

        /// <summary>
        /// 得到枚举项的整数值
        /// </summary>
        public int Value { get; internal set; }
        /// <summary>
        /// 得到枚举项描述
        /// </summary>
        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(ResourceKey))
                    return ResourceKey.StringResource();
                if (!string.IsNullOrEmpty(DefaultDescription))
                    return DefaultDescription;
                return Name;
            }
        }
    }
}
