using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 可扩展对象
    /// </summary>
    /// <typeparam name="T">扩展点</typeparam>
    [Serializable]
    public class ExtensibleObject<T> : IExtensibleObject<T>
       where T : ExtensibleObject<T>
    {
        private List<IExtension<T>> items;
        [NonSerialized]
        private readonly object Mutex = new object();

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public ExtensibleObject()
        {
            items = new List<IExtension<T>>();
        }

        /// <summary>
        /// 得到所有扩展点集合
        /// </summary>
        public IEnumerable<IExtension<T>> Items
        {
            get { return items.ToArray(); }
        }

        /// <summary>
        /// 扩展点数量
        /// </summary>
        public int Count { get { return items.Count; } }

        /// <summary>
        /// 清空所有扩展点
        /// </summary>
        public void Clear()
        {
            if (items.Count == 0)
                return;

            lock (Mutex)
            {
                foreach (var item in items.ToArray())
                    item.Detach(this as T);
                items.Clear();
            }
        }

        /// <summary>
        /// 添加扩展点
        /// </summary>
        /// <param name="extension"></param>
        public void Add(IExtension<T> extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            lock (Mutex)
            {
                extension.Attach(this as T);
                items.Add(extension);
            }
        }

        /// <summary>
        /// 添加扩展点
        /// </summary>
        /// <typeparam name="E"></typeparam>
        public void Add<E>() where E : IExtension<T>, new()
        {
            var item = new E();
            Add(item);
        }

        /// <summary>
        /// 移除扩展点
        /// </summary>
        /// <param name="extension"></param>
        public void Remove(IExtension<T> extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            lock (Mutex)
            {
                if (!items.Contains(extension))
                    throw new ArgumentException("No contain the extension!");

                extension.Detach(this as T);
                items.Remove(extension);
            }
        }
    }
}
