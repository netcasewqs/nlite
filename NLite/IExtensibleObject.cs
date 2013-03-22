using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace NLite
{
    /// <summary>
    /// 可扩展对象接口
    /// </summary>
    /// <typeparam name="T">扩展</typeparam>
    public interface IExtensibleObject<T> where T : IExtensibleObject<T>
    {
        /// <summary>
        /// 得到所有扩展点集合
        /// </summary>
        IEnumerable<IExtension<T>> Items { get; }
        /// <summary>
        /// 扩展点数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 添加扩展点
        /// </summary>
        /// <param name="extension"></param>
        void Add(IExtension<T> extension);
        /// <summary>
        /// 移除扩展点
        /// </summary>
        /// <param name="extension"></param>
        void Remove(IExtension<T> extension);
        /// <summary>
        /// 清空所有扩展点
        /// </summary>
        void Clear();
    }
}
