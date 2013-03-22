
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
        /// <param name="binderType"></param>
        /// <param name="creator"></param>
        void Register(string type, Func<IActivator> creator);

        void Unregister(string type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        IActivator Create(string type);
    }
}
