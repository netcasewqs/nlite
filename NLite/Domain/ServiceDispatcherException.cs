/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;

namespace NLite.Domain
{
    /// <summary>
    /// 服务分发器异常
    /// </summary>
	#if !SILVERLIGHT
    [Serializable]
    #endif
    public class ServiceDispatcherException : NLiteException
    {
        /// <summary>
        /// 异常编码
        /// </summary>
        public ServiceDispatcherExceptionCode ExceptionCode { get; private set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; internal set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperationName { get; internal set; }

        /// <summary>
        /// 构造服务分发器异常
        /// </summary>
        /// <param name="errorCode"></param>
        public ServiceDispatcherException(ServiceDispatcherExceptionCode errorCode):base(errorCode.ToString())
        {
            ExceptionCode = errorCode; 
        }

        /// <summary>
        /// 构造服务分发器异常
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="innerException"></param>
        public ServiceDispatcherException(ServiceDispatcherExceptionCode errorCode, Exception innerException)
            : base(errorCode.ToString(), innerException)
        {
            ExceptionCode = errorCode;
        }

        #if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ServiceDispatcherException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endif
    }

    /// <summary>
    /// 服务分发器异常编码枚举
    /// </summary>
    public enum ServiceDispatcherExceptionCode
    {
        /// <summary>
        /// 服务名称空
        /// </summary>
        ServiceNameIsNullOrEmpty=0,
        /// <summary>
        /// 操作名称空
        /// </summary>
        OperationNameIsNullOrEmpty,
        /// <summary>
        /// 操作参数反序列化异常
        /// </summary>
        OperationParametersDeserializeException,
        /// <summary>
        /// 找不到服务队形
        /// </summary>
        ServiceNotFound,
        /// <summary>
        /// 找不到服务操作
        /// </summary>
        OperationNotFound,
        /// <summary>
        /// 服务创建异常
        /// </summary>
        CreateServiceException,
        /// <summary>
        /// 参数绑定异常
        /// </summary>
        ParameterBindException,
        /// <summary>
        /// 参数校验异常
        /// </summary>
        ModelValidationException,
        /// <summary>
        /// 服务方法调用异常
        /// </summary>
        OperationException,
        /// <summary>
        /// 响应结果序列化异常
        /// </summary>
        ResponseSerializerException,
    }

   
}
