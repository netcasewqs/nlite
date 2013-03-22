using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ResourceAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string BaseResourceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResourceFile { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class StringResourceAttribute : ResourceAttribute
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ImageResourceAttribute : ResourceAttribute
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class IconResourceAttribute : ResourceAttribute
    {
    }
}
