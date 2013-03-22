using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Interceptor.Fluent;
using NLite.Internal;
using NLite.Collections.Internal;

namespace NLite.Interceptor.Metadata
{
    /// <summary>
    /// 切面仓储接口
    /// </summary>
   // [Contract]
    public interface IAspectRepository
    {
        /// <summary>
        /// 注册切面
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        IAspectRepository Register(IAspectInfo aspect);
        /// <summary>
        /// 切面列表
        /// </summary>
        IEnumerable<IAspectInfo> Aspects { get; }
    }

    /// <summary>
    /// 切面仓储
    /// </summary>
    public class AspectRepository : IAspectRepository
    {
        private ICollection<IAspectInfo> aspects = new SyncList<IAspectInfo>();
        /// <summary>
        /// 注册切面
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public IAspectRepository Register(IAspectInfo aspect)
        {
            aspects.Add(aspect);
            return this;
        }
        /// <summary>
        /// 切面列表
        /// </summary>
        public IEnumerable<IAspectInfo> Aspects { get { return aspects; } }
    }
}
