using System;

namespace NLite.Test.IoC.Contract
{
    /// <summary>
    /// 服务加载标准 - 参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ReflectCriteria<T>
    {
        /// <summary>
        /// 公共构造函数
        /// </summary>
        /// <param name="service">调用服务</param>
        /// <param name="operation">调用方法</param>
        /// <param name="args">调用参数</param>
        public ReflectCriteria(string service, string operation, object args)
        {
            ServiceName = service;
            OperationName = operation;
            Arguments = args;
        }

        /// <summary>
        /// 调用服务
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 调用方法
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// 调用参数
        /// </summary>
        public object Arguments { get; set; }
    }
}
