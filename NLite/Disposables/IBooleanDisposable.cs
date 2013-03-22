using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBooleanDisposable : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsDisposed { get; }
    }
}
