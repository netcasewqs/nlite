using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Test.IoC.Addins.Components
{
    //插件接口
    [Contract]
    public interface IAddIn
    {
        void Start();//启动插件
    }

    //插件配置元数据标签
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [MetadataAttributeAttribute]
    public class AddInAttribute : ComponentAttribute
    {
        public string Name { get; set; }
        public string Author;
        public string Url;
        public string Version;
    }

    //插件元数据接口
    public interface IAddInMetadata
    {
        string Name { get; }
        string Author { get; }
        string Url { get; }
        string Version { get; }
    }


    public abstract class AddInBase : IAddIn
    {
        public virtual void Start() { }
    }
}
