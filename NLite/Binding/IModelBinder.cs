using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Binding
{
    /// <summary>
    /// 模型绑定器接口
    /// </summary>
    public interface IModelBinder
    {
        /// <summary>
        /// 执行模型绑定
        /// </summary>
        /// <param name="info">绑定元数据</param>
        /// <param name="valueProvider">valueProvider</param>
        /// <returns></returns>
        object BindModel(BindingInfo info, IDictionary<string, object> valueProvider);
    }

    /// <summary>
    /// 模型绑定器字典
    /// </summary>
    public static class ModelBinders
    {
        /// <summary>
        /// 绑定器列表
        /// </summary>
        public static readonly IDictionary<Type, IModelBinder> Binders = new Dictionary<Type, IModelBinder>();

        /// <summary>
        /// 注册模型绑定器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TBinder"></typeparam>
        /// <param name="binder"></param>
        public static void RegisterBinder<T, TBinder>(TBinder binder) where TBinder : IModelBinder
        {
            if (binder == null)
                throw new ArgumentNullException("binder");
            Binders.Add(typeof(T), binder);
        }

        /// <summary>
        /// 得到指定类型的模型绑定器
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public static IModelBinder GetBinder(Type binderType)
        {
            if (binderType == null)
                throw new ArgumentNullException("binderType");
            IModelBinder binder = null;
            Binders.TryGetValue(binderType, out binder);
            return binder;
        }
    }
}
