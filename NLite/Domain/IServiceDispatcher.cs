using System;
using System.Collections.Generic;
using System.Linq;
using NLite.Log;
using NLite.Domain.Listener;
using NLite.Collections;
using NLite.Validation;
namespace NLite.Domain
{
    /// <summary>
    /// 服务分发器接口 
    /// </summary>
    //[Contract]
    public interface IServiceDispatcher
    {
        /// <summary>
        /// 服务分发
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        IServiceResponse Dispatch(IServiceRequest req);
    }

  
}
