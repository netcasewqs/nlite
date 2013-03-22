using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace NLite.Collections
{
    /// <summary>
    /// 数据状态集合对象，主要用来解耦Asp.net内置的状态对象，如：Session，Application，Cache等
    /// </summary>
    public interface IDataCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[object key] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(object key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void Remove(object key);

        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// ApplicationState 接口，用来解耦Asp.net内置的ApplicationState
    /// </summary>
    //[Contract]
    public interface IApplication : IDataCollection { }

    
    /// <summary>
    /// 
    /// </summary>
   // [Contract]
    public interface ISession : IDataCollection { }

    /// <summary>
    /// 
    /// </summary>
    public interface IDataContext
    {
        /// <summary>
        /// 
        /// </summary>
        IDataCollection Data { get; }
    }
}
