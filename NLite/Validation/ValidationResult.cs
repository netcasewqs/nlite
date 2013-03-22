using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLite.Validation;
using NLite.Internal;

namespace NLite
{
   
    /// <summary>
    /// 
    /// </summary>
    public interface IErrorState : IList<IErrorItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="errorMessage"></param>
        void AddError(string memberName, string errorMessage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessage"></param>
        void AddError(string errorMessage);

        /// <summary>
        /// 
        /// </summary>
        bool IsValid { get; }
    }
    
     /// <summary>
    /// 
    /// </summary>
     #if !SILVERLIGHT
    [Serializable]
    #endif
    public class ErrorState :List<IErrorItem>, IErrorState
    {
        /// <summary>
        /// 
        /// </summary>
        public ErrorState() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public ErrorState(IEnumerable<IErrorItem> items)
        {
            AddRange(items);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsValid
        {
            get { return Count == 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="errorMessage"></param>
        public void AddError(string memberName, string errorMessage)
        {
            Add(new ErrorItem { Key = memberName, Message = errorMessage });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessage"></param>
        public void AddError(string errorMessage)
        {
            Add(new ErrorItem { Message = errorMessage });
        }

       

        /// <summary>
        /// 
        /// </summary>
        public static IErrorState Empty
        {
            get { return new ErrorState(); }
        }
    }

    namespace Internal
    {

        [DebuggerDisplay("Key={Key},Message={Message}")]
#if !SILVERLIGHT
        [Serializable]
#endif
        struct ErrorItem : IErrorItem
        {
            public string Message { get; internal set; }
            public string Key { get; internal set; }
        }
    }
}
