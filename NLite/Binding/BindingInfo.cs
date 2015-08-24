using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using NLite.Reflection;
namespace NLite.Binding
{
    /// <summary>
    /// 绑定元数据类
    /// </summary>
    public class BindingInfo
    {
        /// <summary>
        /// 绑定的成员名称
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 成员类型
        /// </summary>
        public readonly Type Type;
        /// <summary>
        /// 缺省值
        /// </summary>
        public readonly Lazy<object> DefaultValue;

        /// <summary>
        /// 排除项列表
        /// </summary>
        public readonly ICollection<string> Exclude;

        /// <summary>
        /// 包含项列表
        /// </summary>
        public readonly ICollection<string> Include;

        /// <summary>
        /// 成员前缀
        /// </summary>
        public readonly string Prefix;
        /// <summary>
        /// 模型绑定器
        /// </summary>
        public readonly IModelBinder ModelBinder;

        private readonly bool IsNullPrefix = true;
        private static readonly DefaultModelBinder _DefaultModelBinder = new DefaultModelBinder();
        //TODO:参考：http://www.cnblogs.com/tristanguo/archive/2009/02/28/1400186.html
        /// <summary>
        /// 构造参数模型绑定器元数据
        /// </summary>
        /// <param name="p"></param>
        public BindingInfo(ParameterInfo p)
            : this(p.Name, p.ParameterType, GetDefaultValue(p, p.ParameterType))
        {
        }

        private static object GetDefaultValue(ParameterInfo attributeProvider, Type type)
        {
            object defaultValue;
            if (!ParameterInfoUtil.TryGetDefaultValue(attributeProvider, out defaultValue))
                defaultValue = ParameterInfoUtil.GetDefaultValue(type);
            return defaultValue;
        }

        /// <summary>
        /// 构造模型绑定器
        /// </summary>
        /// <param name="name">成员名称</param>
        /// <param name="type">成员类型</param>
        /// <param name="defaultValue">缺省值</param>
        public BindingInfo(string name, Type type, object defaultValue)
        {
            Name = name;
            Type = type;
            DefaultValue = new Lazy<object>(() => defaultValue);

            BindAttribute attr = type.GetAttribute<BindAttribute>(false);
            if (attr == null)
                Exclude = Include = new string[0];
            else
            {
                if (string.IsNullOrEmpty(attr.Prefix))
                    IsNullPrefix = false;
                else
                    Prefix = attr.Prefix.ToLower() + ".";

                Exclude = new ReadOnlyCollection<string>(SplitString(attr.Exclude));
                Include = new ReadOnlyCollection<string>(SplitString(attr.Include));
            }

            var binderAttr = type.GetAttribute<CustomModelBinderAttribute>(false);
            if (binderAttr != null)
                ModelBinder = binderAttr.GetBinder();
            if (ModelBinder == null)
                ModelBinder = ModelBinders.GetBinder(type);
            if (ModelBinder == null)
                ModelBinder = _DefaultModelBinder;
        }

        /// <summary>
        /// 执行模型绑定
        /// </summary>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        public object BindModel(IDictionary<string, object> valueProvider)
        {
            return ModelBinder.BindModel(this, valueProvider);
        }

        /// <summary>
        /// 过滤ValueProvider
        /// </summary>
        /// <param name="valueProvider"></param>
        public void FilterValueProvider(IDictionary<string, object> valueProvider)
        {

            if (!string.IsNullOrEmpty(Prefix))
            {
                var keys = valueProvider.Keys.ToArray();
                foreach (var key in keys)
                {
                    if (key.StartsWith(Prefix) && !key.StartsWith(Prefix + "["))
                        valueProvider.Remove(key);
                }
            }

            foreach (var item in Exclude)
            {
                if (valueProvider.ContainsKey(item))
                    valueProvider.Remove(item);
            }

            foreach (var item in Include)
                if (!valueProvider.ContainsKey(item))
                    valueProvider.Remove(item);

        }

        string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
                return new string[0];

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select IsNullPrefix ? trimmed : Prefix + trimmed;
            return split.ToArray();
        }
    }
}
