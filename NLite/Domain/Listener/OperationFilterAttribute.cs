using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain.Listener
{
    /// <summary>
    /// 操作过滤器注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class OperationFilterAttribute : Attribute
    {
        #region IServiceFilter Members
        /// <summary>
        /// 执行后监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnOperationExecuted(IOperationExecutedContext ctx)
        {
        }

        /// <summary>
        /// 执行前监听
        /// </summary>
        /// <param name="ctx"></param>
        public virtual void OnOperationExecuting(IOperationExecutingContext ctx)
        {
        }

        #endregion
    }

}
