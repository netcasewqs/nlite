using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using NLite.Reflection;
using NLite.Internal;
using NLite.Interceptor.Matcher;
using NLite.Interceptor.Metadata;
using NLite.Interceptor.Internal;

namespace NLite.Interceptor.Matcher
{
    /// <summary>
    /// 
    /// </summary>
    public class AspectMatcher : IAspectMatcher
    {
        private IDictionary<Type, IClassMatcher> Cache = new Dictionary<Type, IClassMatcher>();
        /// <summary>
        /// 
        /// </summary>
        public AspectMatcher()
        {
            Cache[typeof(SignleTargetTypeInfo)] = new SingleTypeMatcher();
            Cache[typeof(NamespaceTargetTypeInfo)] = new NamespaceMatcher();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTargetTypeInfo"></typeparam>
        /// <typeparam name="TClassMatcher"></typeparam>
        public void RegisterClassMatcher<TTargetTypeInfo, TClassMatcher>() 
            where TTargetTypeInfo:ITargetTypeInfo
            where TClassMatcher :IClassMatcher,new()
        {
            var key = typeof(TTargetTypeInfo);
            lock (Cache)
            {
                if (!Cache.ContainsKey(key))
                    Cache[key] = new TClassMatcher();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="aspects"></param>
        /// <returns></returns>
        public IEnumerable<IAspectInfo> Match(Type targetType, IEnumerable<IAspectInfo> aspects)
        {
            return (from aspect in aspects
                    where GetClassMatcher(aspect).Match(targetType, aspect)
                    select aspect).ToArray();
        }

        private IClassMatcher GetClassMatcher(IAspectInfo aspect)
        {
            var key = aspect.TargetType.GetType();
            if (Cache.ContainsKey(key))
                return Cache[key];
           
            return null;
        }
    }

   
}
