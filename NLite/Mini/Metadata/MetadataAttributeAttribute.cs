using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
                    AllowMultiple = false, Inherited = true)]
    public sealed class MetadataAttributeAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public MetadataAttributeAttribute()
        {
        }
    }
}
