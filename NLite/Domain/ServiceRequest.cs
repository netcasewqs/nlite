/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace NLite.Domain
{
	 /// <summary>
    /// 服务请求对象
    /// </summary>
    public class ServiceRequest:IServiceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="actionName"></param>
        /// <param name="args"></param>
        private ServiceRequest(string serviceName, string actionName, IDictionary<string,object> args)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ServiceDispatcherException(ServiceDispatcherExceptionCode.ServiceNameIsNullOrEmpty);
            if (string.IsNullOrEmpty(actionName))
                throw new ServiceDispatcherException(ServiceDispatcherExceptionCode.OperationNameIsNullOrEmpty);
            ServiceName = serviceName;
            OperationName = actionName;
            if(args != null)
                Arguments = new DictionaryWrapper(args);
			else
				Arguments = new DictionaryWrapper();
	
            Arguments["AutoCloseServiceContext"] = 1;

            Context = new DictionaryWrapper();
        }

        /// <summary>
        /// 构造服务请求对象
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="actionName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ServiceRequest Create(string serviceName, string actionName, object args)
        {
            return new ServiceRequest(serviceName,actionName,ToDictionary(args));
        }

        /// <summary>
        /// 构造服务请求对象
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="actionName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ServiceRequest Create(string serviceName, string actionName, IDictionary<string,object> args)
        {
            return new ServiceRequest(serviceName, actionName, args);
        }

        private static IDictionary<string,object> ToDictionary(object values)
        {
            if (values == null)
                return new DictionaryWrapper();

            IDictionary<string,object> dict = values as IDictionary<string,object>;
            if(dict != null)
                return dict;
          
            dict = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(values);
            foreach (PropertyDescriptor p in properties)
                dict[p.Name] = p.GetValue(values);

            return dict;
        }
        /// <summary>
        /// 得到服务名
        /// </summary>
        public string ServiceName { get; private set; }
        /// <summary>
        /// 得到服务方法名
        /// </summary>
        public string OperationName { get; private set; }
        /// <summary>
        /// 得到服务方法参数
        /// </summary>
         public IDictionary<string, object> Arguments { get; internal set; }

        /// <summary>
        /// Gets or sets a value that indicates whether request validation is enabled for this request.
        /// </summary>
        public bool ValidateRequest { get; set; }

        /// <summary>
        /// 得到请求上下文
        /// </summary>
        public IDictionary<string, object> Context { get;internal set; }
    }

}
