//using System.Collections.Generic;
//using System.Reflection;
//using NLite.Reflection;
//using NLite.Collections;


//namespace NLite.Reflection.Internal
//{
//    class DynamicMethodFactoryProxy
//    {

//        private static readonly IMap<MethodInfo, Proc> actionCache = new ConcurrentMap<MethodInfo, Proc>();
//        private static readonly IMap<MethodInfo, Func> funcCache = new ConcurrentMap<MethodInfo, Func>();
//        private static readonly IMap<ConstructorInfo, ConstructorHandler> factoryMethodCache = new ConcurrentMap<ConstructorInfo, ConstructorHandler>();
//        private static readonly IMap<MemberInfo, Getter> getterCache = new ConcurrentMap<MemberInfo, Getter>();
//        private static readonly IMap<MemberInfo, Setter> setterCache = new ConcurrentMap<MemberInfo, Setter>();

//        private DynamicMethodFactoryProxy() { }


//        public static Proc CreateProcMethod(System.Reflection.MethodInfo method)
//        {
//            return actionCache.GetOrAdd(method, () => DefaultDynamicMethodFactory.CreateProcMethod(method));
//        }

//        public static Func CreateFuncMethod(System.Reflection.MethodInfo method)
//        {
//            return funcCache.GetOrAdd(method, () => DefaultDynamicMethodFactory.CreateFuncMethod(method));
//        }

//        public static ConstructorHandler CreateConstructorMethod(System.Reflection.ConstructorInfo constructor)
//        {
//            return factoryMethodCache.GetOrAdd(constructor, () => DefaultDynamicMethodFactory.CreateConstructorMethod(constructor));
//        }

//        public static Getter CreateGetter(System.Reflection.MemberInfo member)
//        {
//            return getterCache.GetOrAdd(member, () => DefaultDynamicMethodFactory.CreateGetter(member));
//        }

//        public static Setter CreateSetter(System.Reflection.MemberInfo member)
//        {
//            return setterCache.GetOrAdd(member, () => DefaultDynamicMethodFactory.CreateSetter(member));
//        }
//    }
//}
