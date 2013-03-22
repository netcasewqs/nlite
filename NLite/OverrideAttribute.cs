using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 方法重载注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OverrideAttribute : Attribute
    {
        /// <summary>
        /// 重载方法的别名
        /// </summary>
        public string AliasNames
        {
            get;
            private set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aliasNames">重载方法的别名</param>
        public OverrideAttribute(string aliasNames)
        {
            if (string.IsNullOrEmpty(aliasNames))
            {
                throw new ArgumentNullException("aliasNames");
            }
            this.AliasNames = aliasNames;
        }
    }
}
