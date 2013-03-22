using System;
namespace NLite.Domain.Listener
{
    /// <summary>
    /// 操作元数据被发现的上下文信息接口
    /// </summary>
    public interface IOperationDescriptorFoundContext:IServiceDescriptorFoundContext
    {
        /// <summary>
        /// 操作元数据
        /// </summary>
        IOperationDescriptor OperationDescriptor { get; }
    }
}
