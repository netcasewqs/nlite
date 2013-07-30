using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite.Log
{
    /// <summary>
    /// 日志管理器接口
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// 得到日志器
        /// </summary>
        /// <param name="name">日志名称</param>
        /// <returns>返回一个日志记录器接口</returns>
        ILog GetLogger(string name);

        /// <summary>
        /// 关闭日志管理器
        /// </summary>
        void Shutdown();
    }
}
