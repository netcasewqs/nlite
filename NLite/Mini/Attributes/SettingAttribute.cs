using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 标记某个字段，属性被DI容器自动从AppSetting设置或者属性文件中注入进来，
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
                        | AttributeTargets.Field
                        , AllowMultiple = false
                        , Inherited = true)]
    public class SettingAttribute:Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public SettingAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
        }
    }
}
