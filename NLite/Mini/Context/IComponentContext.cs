using System;
using System.Collections.Generic;
using NLite.Mini.Lifestyle;
namespace NLite.Mini.Context
{
    /// <summary>
    /// Mini 容器上下文接口
    /// </summary>
    public interface IComponentContext
    {
        /// <summary>
        /// 得到Mini容器
        /// </summary>
        IKernel Kernel { get; }
        /// <summary>
        /// 得到组件元数据
        /// </summary>
        IComponentInfo Component { get; }

        ILifestyleManager LifestyleManager { get;}

        /// <summary>
        /// 
        /// </summary>
        Type[] GenericParameters { get; }

        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, object> NamedArgs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        object[] Args { get; set; }

        /// <summary>
        /// 
        /// </summary>
        object Instance { get; set; }
    }
}
