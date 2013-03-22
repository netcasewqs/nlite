/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;


namespace NLite.Domain
{
	/// <summary>
     /// Provides the context for the OperationExecuting method of the OperationFilterAttribute class.
     /// </summary>
     public interface IOperationExecutingContext
     {
         /// <summary>
         /// 得到ServiceRequest
         /// </summary>
         IServiceRequest Request { get; }
         /// <summary>
         /// 得到OperationDescriptor
         /// </summary>
         IOperationDescriptor OperationDescriptor { get; }
         /// <summary>
         /// 得到服务方法参数
         /// </summary>
         IDictionary<string, object> Arguments { get; }
         /// <summary>
         /// 得到ServiceResponse
         /// </summary>
         IServiceResponse Response { get; set; }
         /// <summary>
         /// 
         /// </summary>
         bool Cancelled { get; set; }
     }

     /// <summary>
     /// Provides the context for the OperationExecuted method of the OperationFilterAttribute class.
     /// </summary>
     public interface IOperationExecutedContext
     {
         /// <summary>
         /// 得到DomainService
         /// </summary>
         Object Service { get; }
         /// <summary>
         /// 得到ServiceRequest
         /// </summary>
         IServiceRequest Request { get; }
         /// <summary>
         /// 得到OperationDescriptor
         /// </summary>
         IOperationDescriptor OperationDescriptor { get; }
         /// <summary>
         /// 得到服务方法参数
         /// </summary>
         IDictionary<string, object> Arguments { get; }
         /// <summary>
         /// 得到ServiceResponse
         /// </summary>
         IServiceResponse Response { get; set; }
     }

}
