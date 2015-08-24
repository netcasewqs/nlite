using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Log;
using NLite.Collections;
using NLite.Interceptor.Fluent;
using NLite.Internal;
using NLite.Interceptor.Metadata;
using NLite.Interceptor.Internal;
using NLite.Mini.Context;
using System.Collections;
using NLite.Reflection.Dynamic.Internal;
using NLite.Reflection.Dynamic.Internal.Emit;
using NLite.Reflection;
using NLite.Interceptor.Matcher;

namespace NLite.Interceptor
{
    /// <summary>
    /// 拦截类型
    /// </summary>
    [Flags]
    public enum AdviceType
    {
        /// <summary>
        /// 不拦截
        /// </summary>
        None = 1,
        /// <summary>
        /// 调用前拦截
        /// </summary>
        Before = 2,
        /// <summary>
        /// 调用后拦截
        /// </summary>
        After = 4,
        /// <summary>
        /// 异常拦截
        /// </summary>
        Exception = 8,
        /// <summary>
        /// 调用前和调用后拦截
        /// </summary>
        Around = Before | After,
        /// <summary>
        /// 调用前、异常和调用后都拦截
        /// </summary>
        All = Around | Exception
    }

    /// <summary>
    /// 拦截器接口
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// 拦截
        /// </summary>
        /// <param name="invocationContext">调用上下文</param>
        /// <returns></returns>
        object Intercept(IInvocationContext invocationContext);
    }

    /// <summary>
    /// 调用上下文
    /// </summary>
    public interface IInvocationContext
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        object Target { get; }
        /// <summary>
        /// 目标方法
        /// </summary>
        MethodInfo Method { get; }
        /// <summary>
        /// 方法参数
        /// </summary>
        object[] Parameters { get; }

        /// <summary>
        /// 处理
        /// </summary>
        /// <returns></returns>
        object Proceed();
    }


    [Serializable]
    internal sealed class InvocationContext : IInvocationContext
    {
        private readonly object _target;
        private readonly MethodInfo _methodInfo;
        private readonly object[] _parameters;
        private readonly IInterceptor[] _interceptors;
        private int _nextInterceptorIndex;

      
        public InvocationContext(object target, MethodInfo methodInfo, object[] parameters, IInterceptor[] interceptors)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            if (interceptors == null)
                throw new ArgumentNullException("interceptors");

            var staticInterceptor = new StaticTargetInterceptor(target);

            var list = interceptors.ToList();
            list.Add(staticInterceptor);

            interceptors = list.ToArray();

            _target = target;
            _methodInfo = methodInfo;
            _parameters = parameters;
            _interceptors = interceptors;

            _nextInterceptorIndex = 0;
        }

        [Serializable]
        class StaticTargetInterceptor : IInterceptor
        {
            private readonly object _target;

            public StaticTargetInterceptor(object target)
            {
                if (target == null)
                    throw new ArgumentNullException("target");

                _target = target;
            }

            #region IInterceptor Members

            /// <inheritdoc/>
            public object Intercept(IInvocationContext invocationContext)
            {
                var methodInfo = invocationContext.Method;

                return methodInfo.Invoke(_target, invocationContext.Parameters);
            }

            #endregion
        }
        private IInterceptor GetNextInterceptor()
        {
            if (_nextInterceptorIndex >= _interceptors.Length)
                throw new InvalidOperationException(Resources.NoMoreInterceptorsInTheInterceptorChain);

            return _interceptors[_nextInterceptorIndex++];
        }

        #region IInvocationContext Members

        /// <inheritdoc/>
        public object Target
        {
            get { return _target; }
        }

        /// <inheritdoc/>
        public MethodInfo Method
        {
            get { return _methodInfo; }
        }

        /// <inheritdoc/>
        public object[] Parameters
        {
            get { return _parameters; }
        }

        /// <inheritdoc/>
        public object Proceed()
        {
            var interceptor = GetNextInterceptor();

            return interceptor.Intercept(this);
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class InterceptorInvocationHandr : IInvocationHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="interfaceTypes"></param>
        public InterceptorInvocationHandr(Type declaringType, Type[] interfaceTypes)
        {
            Aspect.CheckAndRegisterAspectByInterceptorAttribute(declaringType);
            Aspect.RegisterJointPoints(declaringType, interfaceTypes);
        }

        private InvocationHandler InnerHandler = new InvocationHandler();

        /// <inheritdoc/>
        public object Invoke(object target, MethodInfo method, params object[] parameters)
        {
           return InnerHandler.Invoke(target,method,parameters);
        }
    }

    class InvocationHandler : NLite.Reflection.IInvocationHandler
    {
        private NLite.Interceptor.IInterceptor[] GetInterceptors(Type type, MethodInfo methodInfo)
        {
            var interceptors = InterceptorRepository.Instance.Get(methodInfo);
            if (interceptors.Count == 0)
            {
                var method = PopulateMethod(type,methodInfo);
                var tmp = InterceptorRepository.Instance.Get(method);
                foreach (var item in tmp)
                    interceptors.Add(item);

                method = PopulateMethod(method.DeclaringType, methodInfo);
                tmp = InterceptorRepository.Instance.Get(method);
                foreach (var item in tmp)
                    if (!interceptors.Contains(item))
                        interceptors.Add(item);
            }


            return interceptors.Select(p => p()).ToArray();
        }

        private static MethodInfo PopulateMethod(Type type, MethodInfo m)
        {

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                foreach (var method in genericType.GetMethods())
                {
                    if (method.IsPublic && method.Name == m.Name)
                    {
                        var args = m.GetParameters();
                        var args2 = method.GetParameters();

                        if (args.Length == args2.Length)
                        {
                            bool matched = true;
                            for (var i = 0; i < args2.Length; i++)
                            {
                                if (args[i].Name != args2[i].Name)
                                {
                                    matched = false;
                                }

                                if (args[i].IsOut != args2[i].IsOut)
                                {
                                    matched = false;
                                }

                                if (args[i].IsIn != args2[i].IsIn)
                                {
                                    matched = false;
                                }
                                if (args[i].IsOptional != args2[i].IsOptional)
                                {
                                    matched = false;
                                }

                                if (args[i].IsRetval != args2[i].IsRetval)
                                {
                                    matched = false;
                                }

                            }

                            if (matched)
                                return method;
                        }
                    }
                }

                if (m.IsGenericMethod)
                    return m.GetGenericMethodDefinition();

                return m;
            }
            else if (m.IsGenericMethod)
                return m.GetGenericMethodDefinition();
            return m;
        }

        public object Invoke(object target, MethodInfo methodInfo, object[] parameters)
        {
            MethodInfoBase methodBase = methodInfo as MethodInfoBase;
            var rawMethod = methodBase != null ? methodBase._methodInfo : methodInfo;

            var interceptors = GetInterceptors(target.GetType(), rawMethod);
            if (interceptors.Length == 0)
            {
                return methodInfo.Invoke(target, parameters);
            }

            var ctx = new InvocationContext(target, methodInfo, parameters, interceptors);

            return ctx.Proceed();
        }
    }
    /// <summary>
    /// 切面工厂类
    /// </summary>
    public static class Aspect
    {
        private static AspectRepository aspectRepository = new AspectRepository();
        private static IAspectMatcher aspectMatcher= new AspectMatcher();

        private static readonly HashSet<Type> jointPointRegisterCheckTable = new HashSet<Type>();

        internal static IAspectRepository AspectRepository
        {
            get { return aspectRepository; }

        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {
            aspectRepository = new AspectRepository();
            aspectMatcher = new AspectMatcher();

            jointPointRegisterCheckTable.Clear();
        }

        internal static IAspectMatcher AspectMatcher { get { return aspectMatcher; } }

        internal static void CheckAndRegisterAspectByInterceptorAttribute(Type type)
        {
            var aspect = Aspect.BuildAspectByInterceptorAttribute(type);

            if (aspect != null)
                Aspect.AspectRepository.Register(aspect);
        }

        /// <summary>
        /// 得到类型对应的所有切点信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static ICutPointInfo[] GetPointCuts(Type type)
        {
            if (Aspect.AspectRepository == null
                || Aspect.AspectRepository.Aspects.Count() == 0
                || Aspect.AspectMatcher == null)
                return null;

            //得到所有切面
            var aspects = Aspect.AspectMatcher.Match(type, Aspect.AspectRepository.Aspects).ToArray();
            if (aspects.Length == 0)
                return null;

            //得到所有切点
            var pointCuts = (from aspect in aspects
                             from pointCut in aspect.PointCuts
                             select pointCut)
                             .Distinct()
                             .ToArray();
            return pointCuts;
        }



        //得到所有的接入点
        internal static IDictionary<MethodInfo,ICutPointInfo[]> GetJointPoints(Type type,Type[] contractTypes,ICutPointInfo[] pointCuts)
        {
            if (pointCuts == null || pointCuts.Length == 0)
                return null;

            var matcher = new JoinPointMatcher(pointCuts);

            //得到所有的接入点
            var joinPoints = (from method in type.GetMethods().Union(contractTypes.SelectMany(p => p.GetMethods()).Distinct())
                              from pointCut in pointCuts
                              let result = matcher.Match(type, method).ToArray()
                              where result.Length > 0
                              select new { Method = method, PointCuts = result })
                         .ToDictionary(k=>k.Method, v=>v.PointCuts);

            return joinPoints;
        }

        internal static IDictionary<Type, Func<IInterceptor>> GetAdvices(ICutPointInfo[] pointCuts)
        {
            //得到所有Advice
            var advices = (from adviceType in
                               (
                                   from pointCut in pointCuts
                                   from type in pointCut.Advices
                                   select type).Distinct()
                           select
                           new
                           {
                               Type = adviceType,
                               Factory = new Func<IInterceptor>(() => Activator.CreateInstance(adviceType) as IInterceptor),
                           })
                            .ToDictionary(p => p.Type, p => p.Factory);

            return advices;

        }

        internal static void RegisterJointPoints(Type type, IDictionary<MethodInfo, ICutPointInfo[]> joinPoints, IDictionary<Type, Func<NLite.Interceptor.IInterceptor>> advices)
        {
            lock (jointPointRegisterCheckTable)
            {
                if (jointPointRegisterCheckTable.Contains(type))
                    return;
            }

            if (joinPoints != null && joinPoints.Count > 0)
            {
                foreach (var item in joinPoints)
                    RegisterJointPoint(item.Key, item.Value, advices);

                lock (jointPointRegisterCheckTable)
                {
                    jointPointRegisterCheckTable.Add(type);
                }
            }
        }

        private static void RegisterJointPoint(MethodInfo method
          , IEnumerable<ICutPointInfo> pointCuts
          , IDictionary<Type, Func<NLite.Interceptor.IInterceptor>> interceptorMap)
        {
            var temps = InterceptorRepository.Instance.Get(method);

            if (temps.Count > 0)
                return;

            foreach (var pointCut in pointCuts)
                foreach (var advice in pointCut.Advices)
                {
                    var interceptor = interceptorMap[advice];
                    if (interceptor != null)
                        temps.Add(interceptor);
                }
        }

        internal static void RegisterJointPoints(Type type, Type[] interfaceTypes)
        {
            lock (jointPointRegisterCheckTable)
            {
                if (jointPointRegisterCheckTable.Contains(type))
                    return;
            }

            //得到所有切点
            var pointCuts = Aspect.GetPointCuts(type);

            if (pointCuts == null || pointCuts.Length == 0)
                return;

            //得到所有Advice
            var advices = Aspect.GetAdvices(pointCuts);

            if (advices.Count == 0)
                return;

            //得到所有的接入点
            var joinPoints = Aspect.GetJointPoints(type, interfaceTypes, pointCuts);

            if (joinPoints == null || joinPoints.Count == 0)
                return;

            Aspect.RegisterJointPoints(type,joinPoints, advices);
        }


        /// <summary>
        /// 定义命名空间切面
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static INamespaceExpression FromNamespace(string @namespace)
        {
            var aspect = new NamespaceExpression(@namespace);
            Aspect.AspectRepository.Register(aspect.ToAspect());
            return aspect;
        }
        /// <summary>
        /// 定义类型切面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IAspectExpression For<T>()
        {
            return For(typeof(T));
        }
        /// <summary>
        /// 定义类型切面
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static IAspectExpression For(Type componentType)
        {
            var aspect = new SingleTypeExpression(componentType);
            Aspect.AspectRepository.Register(aspect.ToAspect());
            return aspect;
        }

        private static AspectInfo BuildAspectByInterceptorAttribute(Type type)
        {
            AspectInfo aspect = null;

            var adviceTypes = type
                .GetAttributes<InterceptorAttribute>(true)
                .Select(p => p.InterceptorType)
                .Union(type.GetCustomAttributes(true)
                .Where(p => typeof(IInterceptor).IsAssignableFrom(p.GetType()))
                .Select(p=>p.GetType()))
                .ToArray();

            if (adviceTypes != null && adviceTypes.Length != 0)
            {
                aspect = new AspectInfo { TargetType = new SignleTargetTypeInfo { SingleType = type } };

                var pointCut = new PointCutInfo();

                aspect.AddPointCut(pointCut);

                pointCut.Signature = new MethodSignature
                {
                    Deep = 3,
                };

                pointCut.Advices = adviceTypes;
            }

            var pointCuts = (from m in type.GetMethods().Where(m => m.IsPublic || m.IsFamily)
                             let attrs = m.GetAttributes<InterceptorAttribute>(true)
                             where attrs != null && attrs.Length > 0
                             select new PointCutInfo
                             {
                                 Advices = attrs.Select(p => p.InterceptorType).ToArray(),
                                 Signature = new MethodSignature
                                 {
                                     Method = m.Name,
                                     ReturnType = m.ReturnType.FullName,
                                     Flags = CutPointFlags.Method,
                                     Access = AccessFlags.All,
                                     Arguments = m.GetParameterTypes().Select(p => p.FullName).ToArray()
                                 }
                             })
                             .Union(from m in type.GetMethods().Where(m => m.IsPublic || m.IsFamily)
                                    let attrs = m.GetCustomAttributes(true).Where(p => typeof(IInterceptor).IsAssignableFrom(p.GetType())).ToArray()
                                    where attrs != null && attrs.Length > 0
                                    select new PointCutInfo
                                    {
                                        Advices = attrs.Select(p => p.GetType()).ToArray(),
                                        Signature = new MethodSignature
                                        {
                                            Method = m.Name,
                                            ReturnType = m.ReturnType.FullName,
                                            Flags = CutPointFlags.Method,
                                            Access = AccessFlags.All,
                                            Arguments = m.GetParameterTypes().Select(p => p.FullName).ToArray()
                                        }
                                    })
                             .ToArray();


            if (pointCuts != null && pointCuts.Length > 0)
            {
                if (aspect == null)
                    aspect = new AspectInfo();

                foreach (var pointCut in pointCuts)
                {
                    aspect.AddPointCut(pointCut);
                }
            }

            return aspect;
        }

    }


    /// <summary>
    /// 拦截器仓储
    /// </summary>
    public sealed class InterceptorRepository 
    {
        private InterceptorRepository() { }

        /// <summary>
        /// 
        /// </summary>
        public static readonly InterceptorRepository Instance = new InterceptorRepository();

        private IDictionary<MethodBase, ICollection<Func<IInterceptor>>> Cache = new Dictionary<MethodBase, ICollection<Func<IInterceptor>>>();

        /// <summary>
        /// 得到指定方法上的所有拦截器
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public ICollection<Func<IInterceptor>> Get(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            ICollection<Func<IInterceptor>> interceptors;

            lock (Cache)
            {
                if (!Cache.TryGetValue(method, out interceptors))
                    Cache[method.GetBaseDefinition()] = interceptors = new List<Func<IInterceptor>>();
            }

            return interceptors;
        }
    }
}
