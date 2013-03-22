using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Globalization;
using NLite.Collections;

namespace NLite
{
    /// <summary>
    /// 
    /// </summary>
    //[Contract]
    public interface ITagFormatProvider
    {
        /// <summary>
        /// 
        /// </summary>
        bool SupportColon { get; }
        /// <summary>
        /// 
        /// </summary>
        string Tag { get;}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string Format(string str,params string[] args);
    }

   
}
