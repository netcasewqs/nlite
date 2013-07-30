using System;
#if !SDK42
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLite.Reflection;

namespace NLite
{
    /// <summary>
    /// Lazy对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lazy<T>
    {
        private T _value = default(T);
        private volatile bool _isValueCreated = false;
        private Func<T> _valueFactory = null;
        private object _lock;

        public event Action<object> ValueCreated;

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        public Lazy()
            : this(() => Activator.CreateInstance<T>())
        {
        }

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="isThreadSafe">是否线程安全</param>
        public Lazy(bool isThreadSafe)
            : this(() => Activator.CreateInstance<T>(), isThreadSafe)
        {
        }

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="valueFactory">对象工厂</param>
        public Lazy(Func<T> valueFactory) :
            this(valueFactory, true)
        {
        }

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="valueFactory">对象工厂</param>
        /// <param name="isThreadSafe">是否线程安全</param>
        public Lazy(Func<T> valueFactory, bool isThreadSafe)
        {
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

            if (isThreadSafe)
                this._lock = new object();

            this._valueFactory = valueFactory;
        }

        /// <summary>
        /// 得到对象值
        /// </summary>
        public T Value
        {
            get
            {
                return GetValue();
            }
        }

       

        private T GetValue()
        {
            if (!this._isValueCreated)
            {
                if (this._lock != null)
                    Monitor.Enter(this._lock);

                try
                {
                    T value = this._valueFactory.Invoke();
                    this._valueFactory = null;
                    Thread.MemoryBarrier();
                    this._value = value;
                    this._isValueCreated = true;
                    if (ValueCreated != null)
                        ValueCreated(_value);
                }
                finally
                {
                    if (this._lock != null)
                        Monitor.Exit(this._lock);
                }
            }
            return this._value;
        }
    }

    
}
#endif


namespace NLite
{
    /// <summary>
    /// Lazy对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TMetadata">对象的元数据</typeparam>
#if SDK4 && !SILVERLIGHT
    [Serializable]
#endif
    public class Lazy<T, TMetadata> : Lazy<T>
    {
        private TMetadata _metadata;

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="valueFactory">对象工厂</param>
        /// <param name="metadata">对象元数据</param>
        public Lazy(Func<T> valueFactory, TMetadata metadata) :
            base(valueFactory)
        {
            this._metadata = metadata;
        }

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="metadata">对象元数据</param>
        public Lazy(TMetadata metadata) :
            base()
        {
            this._metadata = metadata;
        }

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="metadata">对象元数据</param>
        /// <param name="isThreadSafe">是否线程安全</param>
        public Lazy(TMetadata metadata, bool isThreadSafe) :
            base(isThreadSafe)
        {
            this._metadata = metadata;
        }

        /// <summary>
        /// 创建Lazy对象
        /// </summary>
        /// <param name="valueFactory">对象工厂</param>
        /// <param name="metadata">对象元数据</param>
        /// <param name="isThreadSafe">是否线程安全</param>
        public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe) :
            base(valueFactory, isThreadSafe)
        {
            this._metadata = metadata;
        }

        /// <summary>
        /// 对象元数据
        /// </summary>
        public TMetadata Metadata
        {
            get
            {
                return this._metadata;
            }
        }
    }

}
