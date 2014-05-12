using System;
using System.Collections.Generic;
using NLite.Test.IoC.Contract;

namespace NLite.Test.IoC.Core
{
    /// <summary>
    /// 业务集合代理
    /// </summary>
    public class BusinessListProxy<T, C> : IBusinessList<T, C>
    {
        #region 实现接口 IBusinessList<T, C>

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="criteria">查询标准 - 条件</param>
        /// <returns></returns>
        public virtual IEnumerable<C> Fetch(ReflectCriteria<T> criteria)
        {
            Console.WriteLine("执行......");
            return new List<C>();
        }

        /// <summary>
        /// 更新集合
        /// </summary>
        /// <param name="entity">业务实体</param>
        public virtual string Update(T entity)
        {
            return "";
        }

        #endregion
    }
}
