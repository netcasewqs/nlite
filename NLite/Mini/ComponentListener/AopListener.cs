using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLite.Interceptor;
using NLite.Interceptor.Matcher;
using NLite.Interceptor.Metadata;
using NLite.Mini.Proxy;
using NLite.Reflection;
using NLite.Interceptor.Internal;

namespace NLite.Mini.Listener
{
    /// <summary>
    /// Aop 监听器
    /// </summary>
    public class AopListener : ComponentListenerAdapter
    {
      
        /// <summary>
        /// 
        /// </summary>
        protected override void Init()
        {
            Kernel.RegisterInstance(ProxyFactory.Default);
        }

        /// <summary>
        /// 在组件元数据注册后进行监听，如果符合代理条件的就在元数据的扩展属性里面添加一个"proxy"标记位
        /// </summary>
        /// <param name="info"></param>
        public override void OnMetadataRegistered(IComponentInfo info)
        {
            Aspect.CheckAndRegisterAspectByInterceptorAttribute(info.Implementation);

            if (HasMatch(info))
                info.ExtendedProperties["proxy"] = true;

            base.OnMetadataRegistered(info);
        }

     

        private bool HasMatch(IComponentInfo info)
        {
            //得到所有切点
            var pointCuts = Aspect.GetPointCuts(info.Implementation);

            if (pointCuts == null || pointCuts.Length == 0)
                return false;

            //得到所有Advice
            var advices = Aspect.GetAdvices(pointCuts);

            if (advices.Count == 0)
                return false;

            //得到所有的接入点
            var joinPoints = Aspect.GetJointPoints(info.Implementation,info.Contracts,pointCuts);

            if (joinPoints == null || joinPoints.Count == 0)
                return false;

            Aspect.RegisterJointPoints(info.Implementation, joinPoints, advices);

            info.ExtendedProperties["interceptors"] = advices.Values.Distinct().ToArray();
            info.ExtendedProperties["methods"] = joinPoints.Select(p => p.Key).Distinct().ToArray();

            return true;
        }

       
    }
}
