using System;
using System.Reflection;
using System.Collections.Generic;
namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITargetTypeInfo { }

    /// <summary>
    /// 
    /// </summary>
    public interface INamespaceTargetTypeInfo : ITargetTypeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<Type> Excludes { get;  }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISingleTargetTypeInfo : ITargetTypeInfo
    {
        /// <summary>
        /// 
        /// </summary>
        Type SingleType { get; }
    }
}
