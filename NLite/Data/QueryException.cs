using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace NLite.Data
{
    [Serializable]
    public class QueryException : DatabaseException
    {
        public QueryException() { }
        public QueryException(string message) : base(message) { }
        public QueryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
