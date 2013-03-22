using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Collections;

namespace NLite
{
    /// <summary>
    /// 元数据注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MetadataAttribute: Attribute,IMetadata
    {
        /// <summary>
        /// 构造元数据注解
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public MetadataAttribute(string name, object value)
        {
            Name = name ?? string.Empty;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; private set; }
    }
}
