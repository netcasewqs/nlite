using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Threading;

namespace NLite.Domain
{
    /// <summary>
    /// 服务上下文
    /// </summary>
    public sealed class ServiceContext
    {
        internal const string Key = "NLite_Domain_ServiceContext";

        /// <summary>
        /// 得到或设置服务上下文
        /// </summary>
        public static ServiceContext Current { get { return Local.Get(Key) as ServiceContext; } set { Local.Set(Key, value); } }

        /// <summary>
        /// 服务请求
        /// </summary>
        public IServiceRequest Request { get; internal set; }
        /// <summary>
        /// 服务响应
        /// </summary>
        public IServiceResponse Response { get; internal set; }
        /// <summary>
        /// 上下文
        /// </summary>
        public IDictionary<string, object> Context { get { return Request.Context; } }
    }

}
