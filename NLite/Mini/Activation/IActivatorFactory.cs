
using System;
using NLite.Mini.Context;
using System.Collections.Generic;
namespace NLite.Mini.Activation
{
    /// <summary>
    /// 
    /// </summary>
    public interface IActivatorFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="creator"></param>
        void Register(string type, Func<IActivator> creator);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        void Unregister(string type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IActivator Create(string type);
    }
}
