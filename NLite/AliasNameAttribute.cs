using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 别名注解,可以对领域服务或者领域方法启个别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AliasNameAttribute : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string AliasNames
        {
            get;
            private set;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aliasNames">别名</param>
        public AliasNameAttribute(string aliasNames)
        {
            this.AliasNames = aliasNames;
        }
    }
}
