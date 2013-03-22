using System;
using System.Diagnostics;

namespace NLite.Globalization
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResourceItem 
        #if !SILVERLIGHT
        : ICloneable
        #endif
    {
        /// <summary>
        /// 
        /// </summary>
        string BaseResourceName { get; }
        /// <summary>
        /// 
        /// </summary>
        string ResourceFile { get; }
        /// <summary>
        /// 
        /// </summary>
        string AssemblyFile { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        new IResourceItem Clone();
    }

    /// <summary>
    /// 
    /// </summary>
    #if !SILVERLIGHT
    [Serializable]
    #endif
    [DebuggerDisplay("BaseResourceName={BaseResourceName},ResourceFile={ResourceFile}")]
    public class ResourceItem : IResourceItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string BaseResourceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResourceFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AssemblyFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IResourceItem Clone()
        {
            return new ResourceItem
            {
                BaseResourceName = this.BaseResourceName,
                ResourceFile = this.ResourceFile,
                AssemblyFile = this.AssemblyFile,
            };
        }

        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endif
    }
}
