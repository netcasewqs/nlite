using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NLite.Globalization
{
    /// <summary>
    /// 资源注册表接口
    /// </summary>
    public interface IResourceRegistry : ILanguageChangedListner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="asm"></param>
        void Register(IEnumerable<IResourceItem> items, Assembly asm);

        /// <summary>
        /// 根据ResourceItem 决定是注册文件资源或者程序集资源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="asm"></param>
        void Register(IResourceItem item, Assembly asm);

        /// <summary>
        /// 注册Assembly资源
        /// </summary>
        /// <param name="baseResourceName"></param>
        /// <param name="assembly"></param>
        void Register(string baseResourceName, Assembly assembly);
        /// <summary>
        /// 注册文件资源
        /// </summary>
        /// <param name="fileName"></param>
        void Register(string fileName);
        /// <summary>
        /// 注册流资源
        /// </summary>
        /// <param name="stream"></param>
        void Register(Stream stream);
    }
}