
using System.Collections.Generic;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILifestyleManagerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ILifestyleManager Create(LifestyleFlags type);
    }
}