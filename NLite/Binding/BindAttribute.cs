using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Binding
{
    /// <summary>
    /// 绑定注解,可以绑定在Class、结构、参数上
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct| AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class BindAttribute : Attribute
    {

        private string _exclude;
        private string[] _excludeSplit = new string[0];
        private string _include;
        private string[] _includeSplit = new string[0];

        /// <summary>
        /// 排除的成员列表，用逗号分隔
        /// </summary>
        public string Exclude
        {
            get
            {
                return _exclude ?? String.Empty;
            }
            set
            {
                _exclude = value;
                _excludeSplit = SplitString(value);
            }
        }

        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }

        /// <summary>
        /// 包含的成员列表，用逗号分隔
        /// </summary>
        public string Include
        {
            get
            {
                return _include ?? String.Empty;
            }
            set
            {
                _include = value;
                _includeSplit = SplitString(value);
            }
        }

        /// <summary>
        /// 得到或设置成员前缀
        /// </summary>
        public string Prefix
        {
            get;
            set;
        }

        internal static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
        {
            // We allow a property to be bound if its both in the include list AND not in the exclude list.
            // An empty include list implies all properties are allowed.
            // An empty exclude list implies no properties are disallowed.
            bool includeProperty = (includeProperties == null) || (includeProperties.Length == 0) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            bool excludeProperty = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            return includeProperty && !excludeProperty;
        }

        /// <summary>
        /// 判断指定的属性是否支持绑定
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsPropertyAllowed(string propertyName)
        {
            return IsPropertyAllowed(propertyName, _includeSplit, _excludeSplit);
        }
    }


}
