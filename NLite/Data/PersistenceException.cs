using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace NLite.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DatabaseException : DbException
    {
        /// <summary>
        /// 
        /// </summary>
        public DatabaseException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public DatabaseException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 
    /// </summary>
     [Serializable]
    public class ConnectionException : DatabaseException
    {
         /// <summary>
         /// 
         /// </summary>
        public ConnectionException() { }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="message"></param>
        public ConnectionException(string message) : base(message) { }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="message"></param>
         /// <param name="innerException"></param>
        public ConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PersistenceException : DatabaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public PersistenceException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public PersistenceException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InsertException : PersistenceException
    {
        /// <summary>
        /// 
        /// </summary>
        public InsertException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InsertException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InsertException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DeleteException : PersistenceException
    {
        /// <summary>
        /// 
        /// </summary>
        public DeleteException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public DeleteException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DeleteException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UpdateException : PersistenceException
    {
        /// <summary>
        /// 
        /// </summary>
        public UpdateException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UpdateException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UpdateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
