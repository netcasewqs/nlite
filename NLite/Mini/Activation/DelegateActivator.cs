using System;
using NLite.Mini.Context;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// 委派组件工厂，通过委托函数（组件在注册时自己提供了基于委托函数的工厂）创建并返回
    /// </summary>
    public class DelegateActivator : AbstractActivator
    {
        /// <summary>
        /// 委托函数
        /// </summary>
        public Func<IComponentContext, object> Creator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object InternalCreate(IComponentContext context)
        {
            return Creator(context);
        }
    }
}
