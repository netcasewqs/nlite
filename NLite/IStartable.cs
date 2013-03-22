
using NLite.Reflection;
using System;
using System.Linq;

namespace NLite
{
    /// <summary>
    /// 启动接口
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }

   
}
