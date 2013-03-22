/*
 * Created by SharpDevelop.
 * User: qswang
 * Date: 2011-3-29
 * Time: 14:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace NLite.Domain.Mapping
{
    public interface IRedirectToErrorResult : INavigationResult
    {
        string ControllerName { get; set; }
        string ActionName { get; set; }
        bool IsSaveModelState { get; set; }
    }
	 /// <summary>
     /// 定义领域服务方法执行失败后，返回给mvc控制器的路由规则
     /// </summary>
     [Serializable]
     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
     public class RedirectToErrorAttribute : Attribute,IRedirectToErrorResult
     {
         /// <summary>
         /// Get or set controller serviceDispatcherName
         /// </summary>
         public string ControllerName { get; set; }

         /// <summary>
         /// get or set action serviceDispatcherName
         /// </summary>
         public string ActionName { get; set; }
         /// <summary>
         /// 得到或设置是否保存模型状态
         /// </summary>
         public bool IsSaveModelState { get; set; }

         string INavigationResult.Type
         {
             get { return ActionResultTypes.Error; }
         }
     }

}
