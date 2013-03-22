using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain
{
    /// <summary>
    /// 服务请求对象
    /// </summary>
    public interface IServiceRequest
    {
        /// <summary>
        /// 得到服务名
        /// </summary>
        string ServiceName { get; }
        /// <summary>
        /// 得到服务方法名
        /// </summary>
        string OperationName { get; }

        /// <summary>
        /// 请求参数
        /// </summary>
        IDictionary<string,object> Arguments { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether request validation is enabled for this request.
        /// </summary>
        bool ValidateRequest { get; set; }

        /// <summary>
        /// 请求上下文
        /// </summary>
        IDictionary<string, object> Context { get;  }
    }
}
