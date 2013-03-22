using System;
namespace NLite.Domain.Listener
{
    /// <summary>
    /// 服务元数据被发现上下文信息接口
    /// </summary>
    public interface IServiceDescriptorFoundContext
    {
        /// <summary>
        /// 得到或设置是否取消服务调用 
        /// </summary>
        bool Cancelled { get; set; }
        /// <summary>
        /// 服务请求对象
        /// </summary>
        IServiceRequest Request { get; }
        /// <summary>
        /// 服务响应队形
        /// </summary>
        IServiceResponse Response { get; }
        /// <summary>
        /// 服务元数据
        /// </summary>
        IServiceDescriptor ServiceDescriptor { get; }
    }
}
