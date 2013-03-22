/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NLite.Domain.Mapping;

namespace NLite.Domain
{
	  /// <summary>
    /// Encapsulates domain service response information.
    /// </summary>
    [Serializable]
    class ServiceResponse : IServiceResponse
    {
        /// <summary>
        /// Success / failure 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Get or set Execption
        /// </summary>
        public Exception[] Exceptions { get { return exceptions.ToArray(); } }

        public Exception Exception
        {
            get { return exceptions.FirstOrDefault(); }
        }

        /// <summary>
        /// Get error collection
        /// </summary>
         public ErrorState ErrorState { get; private set; }

        /// <summary>
        /// Get or set response result
        /// </summary>
        public virtual object Result { get; set; }


        private List<Exception> exceptions;
        /// <summary>
        /// 
        /// </summary>
        public ServiceResponse()
        {
            ErrorState = new ErrorState();
            exceptions = new List<Exception>();
        }

        public ServiceResponse(Exception ex)
        {
            ErrorState = new ErrorState();
            exceptions = new List<Exception>();
            AddException(ex);
        }


        internal void AddException(Exception ex)
        {
            if (ex == null)
                return;

            Success = false;
            exceptions.Add(ex);
            ex.Handle();
        }
    }
}
