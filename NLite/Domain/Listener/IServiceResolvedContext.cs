using System;
namespace NLite.Domain.Listener
{
    /// <summary>
    /// 服务对象解析上下文接口
    /// </summary>
    public interface IServiceResolvedContext:IOperationDescriptorFoundContext
    {
        /// <summary>
        /// 得到或设置服务对象
        /// </summary>
        object Service { get; set; }
    }
}
