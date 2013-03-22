using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    public interface IErrorItem
    {
       
        /// <summary>
        /// 
        /// </summary>
        string Message { get; }
        /// <summary>
        /// 
        /// </summary>
        string Key { get; }

    }
}
