using System.Collections.Generic;

namespace NLite.Test.IoC.Contract
{
    /// <summary>
    /// 业务集合服务接口
    /// </summary>
    public interface IBusinessList<T, C>
    {
        /// <summary>
        /// 获取 Csla 实体集合
        /// </summary>
        /// <param name="criteria">查询参数</param>
        /// <returns>Csla 实体集合</returns>
        IEnumerable<C> Fetch(ReflectCriteria<T> criteria);

        /// <summary>
        /// 更新 Csla 实体集合
        /// </summary>
        /// <param name="entity">Csla 实体集合</param>
        string Update(T entity);
    }
}
