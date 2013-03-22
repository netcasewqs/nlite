using System;
using System.Collections.Generic;
using NLite.Mini.Lifestyle;
using NLite.Mini.Context;



namespace NLite
{
    /// <summary>
    /// 容器接口
    /// </summary>
    public interface IKernel : ILazyServiceLocator, IServiceRegistry, IEnumerable<IComponentInfo>
    {
        /// <summary>
        /// 得到或设置父容器
        /// </summary>
        IKernel Parent { get; set; }
      
        /// <summary>
        /// 得到组件上下文
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IComponentContext GetComponentContextByNamedArgs(Type contract, IDictionary<string, object> args);
        /// <summary>
        /// 得到组件上下文
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IComponentContext GetComponentContextByOptionArgs(Type contract, params object[] args);
        /// <summary>
        /// 得到组件上下文
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IComponentContext GetComponentContextByNamedArgs(string id, IDictionary<string, object> args);
        /// <summary>
        /// 得到组件上下文
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IComponentContext GetComponentContextByOptionArgs(string id, params object[] args);
        /// <summary>
        /// 得到组件上下文列表
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        IComponentContext[] GetComponentContextList(Type contract);
    }
  
}
