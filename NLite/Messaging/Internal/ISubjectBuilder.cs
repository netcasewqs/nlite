using System;

namespace NLite.Messaging.Internal
{
    /// <summary>
    /// 主题对象构造器
    /// </summary>
    /// <remarks>
    /// 采用设计模式中的Builder模式
    /// </remarks>
    interface ISubjectBuilder
    {
        /// <summary>
        /// 根据消息主题名称和消息类型构造主题对象并返回
        /// </summary>
        /// <param name="name">主题名称</param>
        /// <returns>返回创建后的主题对象</returns>
        ISubject Build(string name);
    }

  

}
