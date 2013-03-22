using System;
using NLite.Mini.Context;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// 组件工厂
    /// </summary>
    public interface IActivator
    {
        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="ctx">创建上下文</param>
        /// <returns>返回所创建的组件</returns>
        object Create(IComponentContext ctx);
    }
}
