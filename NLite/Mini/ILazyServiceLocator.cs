using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 懒惰服务定位器
    /// </summary>
    public interface ILazyServiceLocator:IServiceLocator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<Lazy<T>> LazyGetAll<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <returns></returns>
        IEnumerable<Lazy<T, TMetadata>> LazyMetadataGetAll<T, TMetadata>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Lazy<T> LazyGet<T>(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Lazy<T, TMetadata> LazyMetadataGet<T, TMetadata>(string id);
    }
}
