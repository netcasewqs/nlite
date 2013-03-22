using System;
namespace NLite.Domain
{
    /// <summary>
    /// Encapsulates domain service response information
    /// </summary>
    public interface IServiceResponse 
    {
        /// <summary>
        /// Success / failure 
        /// </summary>
        bool Success { get; }
        /// <summary>
        /// Get or set response result
        /// </summary>
        object Result { get; set; }
        /// <summary>
        /// 得到所有的出错状态
        /// </summary>
        ErrorState ErrorState { get; }
        /// <summary>
        /// 得到异常
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// 得到所有的异常集合 
        /// </summary>
        Exception[] Exceptions { get; }
    }

}
