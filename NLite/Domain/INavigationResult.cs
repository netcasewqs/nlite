using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Domain
{
    /// <summary>
    /// 导航结果接口
    /// </summary>
    public interface INavigationResult
    {
        /// <summary>
        /// 得到导航类型
        /// </summary>
        string Type { get; }
    }

    /// <summary>
    /// 导航结果注解
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class NavigationResultAttribute : Attribute, INavigationResult
    {
        internal readonly  string Type;
        string INavigationResult.Type { get{ return Type;}}

        /// <summary>
        /// 构造导航注解
        /// </summary>
        /// <param name="type"></param>
        public NavigationResultAttribute(string type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type");
            Type = type;
        }
    }

   
}
