#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using NLite.Interceptor;
using System.Reflection.Emit;
using System.Threading;
using NLite.Mini.Proxy.Internal;
using NLite.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Activation;
using System.Security.Permissions;

namespace NLite.Mini.Proxy
{
    public static class ProxyFactory
    {
        public static readonly IProxyFactory Default = new RealProxyFactory();
        //public static readonly IProxyFactory Emit = new EmitProxyFactory();
    }
    namespace Internal
    {
        class RealProxyFactory : IProxyFactory
        {
            public object Create(Context.IComponentContext ctx, object instance, params object[] constructorArguments)
            {
                if (instance != null)
                {
                    var interfaces = ctx.Component.Contracts.Where(i => i.IsInterface).ToArray();
                    var marshallableObj = instance as MarshalByRefObject;

                    if (interfaces.Length == 0 && marshallableObj == null)
                        return instance;

                    var methods = ctx.Component.ExtendedProperties["methods"] as MethodInfo[];

                    InvocationDelegate onInvocation = (target, method, parameters) =>
                        {
                            var m = method as MethodInfo;
                            if (methods.FirstOrDefault(p=>p.Name == m.Name) == null)
                                return m.Invoke(target, parameters);

                            var interceptorBroker = new InterceptorBroker();
                            var invocationCtx = new InvocationContext { Target = target, Method = m, Arguments = parameters };

                            try
                            {
                                interceptorBroker.OnInvocationExecuting(invocationCtx);
                                invocationCtx.Result = m.Invoke(target, parameters);
                            }
                            catch (Exception ex)
                            {
                                invocationCtx.Exception = ex;
                                interceptorBroker.OnException(invocationCtx);
                            }
                            finally
                            {
                                interceptorBroker.OnInvocationExecuted(invocationCtx);
                            }

                            return invocationCtx.Result;

                        };
                    if (marshallableObj != null)
                        return new MarshallableDynamicProxyImpl(marshallableObj, onInvocation)
                            .GetTransparentProxy();

                    return new InterfaceDynamicProxyImpl(instance, onInvocation, true, interfaces)
                        .GetTransparentProxy();
                }
                throw new NotImplementedException();
            }

            public bool RequiresTargetInstance(IKernel kernel, IComponentInfo model)
            {
                return true;
            }

            delegate object InvocationDelegate(object target, MethodBase method, object[] parameters);
            interface IDynamicProxy
            {
            }
            class InterfaceDynamicProxyImpl : RealProxy, IRemotingTypeInfo, IDynamicProxy
            {
                private object proxyTarget;
                private bool strict;
                private Type[] supportedTypes;
                private InvocationDelegate invocationHandler;

                protected internal InterfaceDynamicProxyImpl(object proxyTarget, InvocationDelegate invocationHandler, bool strict, Type[] supportedTypes)
                    : base(typeof(IDynamicProxy))
                {
                    this.proxyTarget = proxyTarget;
                    this.invocationHandler = invocationHandler;
                    this.strict = strict;
                    this.supportedTypes = supportedTypes;
                }

                public override ObjRef CreateObjRef(System.Type type)
                {
                    throw new NotSupportedException("ObjRef for DynamicProxy isn't supported");
                }

                public bool CanCastTo(System.Type toType, object obj)
                {
                    // Assume we can (which is the default unless strict is true)
                    bool canCast = true;

                    if (strict)
                    {
                        // First check if the proxyTarget supports the cast
                        if (toType.IsAssignableFrom(proxyTarget.GetType()))
                        {
                            canCast = true;
                        }
                        else if (supportedTypes != null)
                        {
                            canCast = false;
                            // Check if the list of supported interfaces supports the cast
                            foreach (Type type in supportedTypes)
                            {
                                if (toType == type)
                                {
                                    canCast = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            canCast = false;
                        }
                    }

                    return canCast;
                }

                public override IMessage Invoke(IMessage message)
                {
                    var methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)message);
                    var method = methodMessage.MethodBase;

                    object returnValue = null;
                    if (method.DeclaringType == typeof(IDynamicProxy))
                        returnValue = method.Invoke(this, methodMessage.Args);
                    else
                    {
                        returnValue = invocationHandler(proxyTarget, method, methodMessage.Args);
                    }

                    return new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
                }

                string IRemotingTypeInfo.TypeName
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            class MarshallableDynamicProxyImpl : RealProxy, IDynamicProxy
            {
                public string URI { get; protected set; }
                public MarshalByRefObject ProxyTargetTyped { get; protected set; }
                public object ProxyTarget { get { return ProxyTargetTyped; } }
                public InvocationDelegate InvocationHandler { get; set; }

                public MarshallableDynamicProxyImpl(MarshalByRefObject targetObject, InvocationDelegate invoker)
                    : this(targetObject.GetType(), targetObject, invoker)
                {
                }


                public MarshallableDynamicProxyImpl(Type type1, MarshalByRefObject targetObject, InvocationDelegate invoker)
                    : base(type1)
                {
                    ProxyTargetTyped = targetObject;
                    InvocationHandler = invoker;

                    ObjRef myObjRef = RemotingServices.Marshal(ProxyTargetTyped);
                    URI = myObjRef.URI;
                }

                public override IMessage Invoke(IMessage message)
                {
                    if (message is IConstructionCallMessage)
                    {
                        IConstructionReturnMessage myIConstructionReturnMessage =
                           this.InitializeServerObject((IConstructionCallMessage)message);
                        ConstructionResponse constructionResponse = new
                           ConstructionResponse(null, (IMethodCallMessage)message);
                        return constructionResponse;
                    }
                    IMethodCallMessage methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)message);

                    MethodBase method = methodMessage.MethodBase;

                    object returnValue = null;
                    if (method.DeclaringType == typeof(IDynamicProxy))
                        returnValue = method.Invoke(this, methodMessage.Args);
                    else
                        returnValue = InvocationHandler(ProxyTargetTyped, method, methodMessage.Args);

                    ReturnMessage returnMessage = new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
                    return returnMessage;
                }
                public override ObjRef CreateObjRef(Type ServerType)
                {
                    CustomObjRef myObjRef = new CustomObjRef(ProxyTargetTyped, ServerType);
                    myObjRef.URI = URI;
                    return myObjRef;
                }
                public override void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                    base.GetObjectData(info, context);
                }


                public class CustomObjRef : ObjRef
                {
                    public MarshalByRefObject RealObject { get; set; }

                    public CustomObjRef(MarshalByRefObject obj, Type type)
                        : base(obj, type)
                    {
                        RealObject = obj;
                    }

                    public override void GetObjectData(SerializationInfo info,
                                                       StreamingContext context)
                    {
                        base.GetObjectData(info, context);
                    }

                    public override object GetRealObject(StreamingContext context)
                    {
                        return this.RealObject;
                    }
                }


            }
        }
        

        class InvocationContext : IInvocationExceptionContext, IInvocationExecutingContext
        {
            public Exception Exception { get; set; }

            public bool ExceptionHandled { get; set; }

            public object Result { get; set; }

            public object Target { get; set; }

            public MethodInfo Method { get; set; }

            public object[] Arguments { get; set; }
        }

        //class EmitProxyFactory : IProxyFactory
        //{

        //    public object Create(Context.IPreCreationContext ctx, object instance, params object[] constructorArguments)
        //    {
        //        if (instance != null)
        //        {
        //            var interfaces = ctx.Component.Contracts.Where(i => i.IsInterface).ToList();
                   
        //            var matchMethods = ctx.Component.ExtendedProperties["methods"] as MethodInfo[];

        //            return EmitProxy.Proxy(instance, m => matchMethods.Contains(m), OnInterceptor, interfaces.ToArray());
        //        }
        //        throw new NotImplementedException();
        //    }

        //    public bool RequiresTargetInstance(IKernel registry, IComponentInfo model)
        //    {
        //        return true;
        //    }

        //    object OnInterceptor(object target, string methodname, object[] parameters, EmitProxyExecute execute)
        //    {
        //        var interceptorBroker = new InterceptorBroker();
        //        var ctx = new InvocationContext { Target = target, Method = execute.Method, Arguments = parameters };

        //        try
        //        {
        //            interceptorBroker.OnInvocationExecuting(ctx);
        //            ctx.Result = execute(target, parameters);
        //        }
        //        catch (Exception ex)
        //        {
        //            ctx.Exception = ex;
        //            interceptorBroker.OnException(ctx);
        //        }
        //        finally
        //        {
        //            interceptorBroker.OnInvocationExecuted(ctx);
        //        }

        //        return ctx.Result;
        //    }

        //    delegate object EmitProxyInterceptor(object proxiedObject, string methodname, object[] parameters, EmitProxyExecute execute);

        //    delegate object EmitProxyExecute(object objectToExecute, params object[] parameters);

        //    static class EmitProxy
        //    {
        //        private const string ASSEMBLY_NAME = "ProxyAssembly";
        //        private const string MODULE_NAME = "ProxyModule";
        //        private const string TYPE_SUFFIX = "Proxy";
        //        private const string EXECUTE_PREFIX = "Execute";

        //        private static Type NO_ATTRIBUTE = typeof(object);

        //        private static Dictionary<Type, Type> _dynamicTypeCache = new Dictionary<Type, Type>();


        //        public static object Proxy(object objectToProxy, EmitProxyInterceptor interceptorDelegate, params Type[] interfaceTypes)
        //        {
        //            return EmitProxy.Proxy(objectToProxy, (bindingInfo) => true, interceptorDelegate, interfaceTypes);
        //        }

        //        public static object Proxy(object objectToProxy, Predicate<MethodInfo> methodSelector, EmitProxyInterceptor interceptorDelegate, params Type[] interfaceTypes)
        //        {
        //            Type proxyType = null;
        //            var interfaceList = new HashSet<Type>();
        //            var baseType = objectToProxy.GetType();
        //            var parentType = PopulateInterfaceList(baseType, interfaceTypes, interfaceList, baseType);

        //            proxyType = CreateProxy(objectToProxy, methodSelector, interceptorDelegate, interfaceList.ToArray());
        //            return Activator.CreateInstance(proxyType, objectToProxy, interceptorDelegate);
        //        }

        //        private static Type PopulateInterfaceList(Type baseType, Type[] baseInterfaces, HashSet<Type> interfaceList, Type parentType)
        //        {
        //            if (baseInterfaces != null && baseInterfaces.Length > 0)
        //                foreach (var baseInterface in baseInterfaces)
        //                    if (baseInterface != null)
        //                        interfaceList.Add(baseInterface);


        //            if (baseType.IsInterface)
        //            {
        //                parentType = typeof(object);
        //                interfaceList.Add(baseType);
        //            }

        //            Type[] interfaces = interfaceList.ToArray();
        //            foreach (Type interfaceType in interfaces)
        //                BuildInterfaceList(interfaceType, interfaceList);
        //            return parentType;
        //        }

        //        private static void BuildInterfaceList(Type currentType, HashSet<Type> interfaceList)
        //        {
        //            Type[] interfaces = currentType.GetInterfaces();
        //            if (interfaces == null || interfaces.Length == 0)
        //                return;

        //            foreach (Type current in interfaces)
        //            {
        //                interfaceList.Add(current);
        //                BuildInterfaceList(current, interfaceList);
        //            }
        //        }

        //        private static IEnumerable<MethodInfo> PopulateMethodList(
        //            IEnumerable<Type> interfaceList
        //            , IEnumerable<MethodInfo> methods)
        //        {
        //            foreach (MethodInfo method in methods)
        //            {
        //                if (method.DeclaringType == typeof(object))
        //                    continue;
        //                if (method.IsPrivate)
        //                    continue;
        //                if (method.IsFinal)
        //                    continue;
        //                if (!method.IsVirtual && !method.IsAbstract)
        //                    continue;
        //                if (method.Name == "GetObjectData")
        //                    continue;

        //                yield return method;
        //            }

        //            foreach (Type interfaceType in interfaceList)
        //            {
        //                MethodInfo[] interfaceMethods = interfaceType.GetMethods();
        //                foreach (MethodInfo interfaceMethod in interfaceMethods)
        //                    yield return interfaceMethod;
        //            }
        //        }


        //        private static Type CreateProxy(object target, Predicate<MethodInfo> methodSelector, EmitProxyInterceptor interceptorDelegate, params Type[] interfaceTypes)
        //        {
        //            ModuleBuilder moduleBuilder = DynamicAssemblyManager.moduleBuilder;

        //            var targetType = target.GetType();

        //            lock (_dynamicTypeCache)
        //            {
        //                if (_dynamicTypeCache.ContainsKey(targetType))
        //                    return _dynamicTypeCache[targetType];
        //            }


        //            TypeBuilder typeBuilder = CreateType(moduleBuilder, targetType.Name + TYPE_SUFFIX, targetType);
        //            foreach (CustomAttributeBuilder attrBuilder in CreateCustomAttributeBuilder(target.GetType()))
        //                typeBuilder.SetCustomAttribute(attrBuilder);

        //            FieldInfo[] fieldInfo = CreateFieldsAndConstrucutor(typeBuilder, targetType
        //                , "ProxiedObject",
        //                typeof(EmitProxyInterceptor), "Interceptor");


        //            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        //            var targetMethods = PopulateMethodList(interfaceTypes, methods).Distinct().ToArray();
        //            foreach (MethodInfo targetMethod in targetMethods)
        //            {
        //                ParameterInfo[] parameterInfos = targetMethod.GetParameters();
        //                Type[] parameterTypes = new Type[parameterInfos.Length];
        //                for (int i = 0; i < parameterInfos.Length; i++)
        //                    parameterTypes[i] = parameterInfos[i].ParameterType;


        //                MethodBuilder proxyMethod = methodSelector(targetMethod)
        //                    ? CreateProxyMethod(
        //                            typeBuilder
        //                            , targetMethod.DeclaringType
        //                            , targetMethod
        //                            , CreateExecuteMethod(typeBuilder, targetMethod.DeclaringType, EXECUTE_PREFIX + targetMethod.Name, targetMethod)
        //                            , fieldInfo[0]
        //                            , fieldInfo[1])
        //                    :
        //                    CreatePassThroughMethod(typeBuilder, targetMethod, fieldInfo[0]);

        //                foreach (CustomAttributeBuilder attrBuilder in CreateCustomAttributeBuilder(targetMethod))
        //                    proxyMethod.SetCustomAttribute(attrBuilder);

        //            }

        //            var proxyType = typeBuilder.CreateType();
        //            lock (_dynamicTypeCache)
        //                _dynamicTypeCache[targetType] = proxyType;
        //            return proxyType;
        //        }

              

        //        private static TypeBuilder CreateType(ModuleBuilder moduleBuilder, string typeName, Type targetType, params Type[] interfaceTypes)
        //        {
        //            TypeAttributes typeAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;

        //            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, targetType, interfaceTypes);
        //            return typeBuilder;
        //        }

        //        private static FieldInfo[] CreateFieldsAndConstrucutor(TypeBuilder typeBuilder, params object[] fieldTypeAndName)
        //        {
        //            Type[] types = new Type[fieldTypeAndName.Length / 2];
        //            string[] names = new string[fieldTypeAndName.Length / 2];

        //            //Define the types, even items are types odd items are fields
        //            for (int i = 0; i < types.Length; i++)
        //            {
        //                types[i] = (Type)fieldTypeAndName[i * 2];
        //                names[i] = (string)fieldTypeAndName[i * 2 + 1];
        //            }

        //            FieldInfo[] fieldInfos = new FieldInfo[types.Length];
        //            for (int i = 0; i < types.Length; i++)
        //                fieldInfos[i] = typeBuilder.DefineField(names[i], types[i], FieldAttributes.Public);

        //            ConstructorInfo superConstructor = typeof(object).GetConstructor(Type.EmptyTypes);
        //            ConstructorBuilder fieldPopulateConstructor = typeBuilder.DefineConstructor(
        //                    MethodAttributes.Public, CallingConventions.Standard, types);




        //            #region( "Constructor IL Code" )
        //            ILGenerator constructorIL = fieldPopulateConstructor.GetILGenerator();

        //            //loop through all the fields adding them to the correct field
        //            for (int i = 0; i < fieldInfos.Length; i++)
        //            {
        //                // Load "this"
        //                constructorIL.Emit(OpCodes.Ldarg_0);

        //                // Load parameter
        //                constructorIL.Emit(OpCodes.Ldarg, i + 1);

        //                //Set the parameter into the field
        //                constructorIL.Emit(OpCodes.Stfld, fieldInfos[i]);
        //            }

        //            // Load "this"
        //            constructorIL.Emit(OpCodes.Ldarg_0);

        //            //call super
        //            constructorIL.Emit(OpCodes.Call, superConstructor);

        //            // Constructor return
        //            constructorIL.Emit(OpCodes.Ret);
        //            #endregion

        //            return fieldInfos;

        //        }

        //        private static MethodInfo CreateExecuteMethod(TypeBuilder typeBuilder, Type targetType, string methodName, MethodInfo methodToExecute)
        //        {
        //            //get a list of parameter types
        //            ParameterInfo[] parameterInfos = methodToExecute.GetParameters();
        //            Type[] parameterTypes = new Type[parameterInfos.Length];
        //            for (int i = 0; i < parameterInfos.Length; i++)
        //                parameterTypes[i] = parameterInfos[i].ParameterType;

        //            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
        //                methodName,
        //                MethodAttributes.Public | MethodAttributes.Static,
        //                CallingConventions.Standard,
        //                typeof(object),
        //                new Type[] { targetType, typeof(object[]) });

        //            ILGenerator il = methodBuilder.GetILGenerator();



        //            il.Emit(OpCodes.Ldarg_0);
        //            for (int i = 0; i < parameterTypes.Length; i++)
        //            {
        //                il.Emit(OpCodes.Ldarg_1);   //laod arg array
        //                il.Emit(OpCodes.Ldc_I4, i); //load index
        //                il.Emit(OpCodes.Ldelem_Ref);//get element at index from arg array
        //                if (parameterTypes[i].IsValueType)
        //                    il.Emit(OpCodes.Unbox_Any, parameterTypes[i]); //unbox if necassary
        //                else
        //                    il.Emit(OpCodes.Castclass, parameterTypes[i]);

        //            }

        //            il.EmitCall(OpCodes.Call, methodToExecute, null); //call real method
        //            if (methodToExecute.ReturnType == typeof(void))
        //                il.Emit(OpCodes.Ldnull);    //if method is void return, we will return null, because we have to return something
        //            else if (methodToExecute.ReturnType.IsValueType)
        //                il.Emit(OpCodes.Box, methodToExecute.ReturnType); //we need to box our return binderType

        //            il.Emit(OpCodes.Ret);//return

        //            return methodBuilder;
        //        }

        //        private static MethodBuilder CreateProxyMethod(TypeBuilder typeBuilder, Type targetType, MethodInfo proxiedMethod, MethodInfo executeMethod, FieldInfo proxiedObjectField, FieldInfo interceptorField)
        //        {
        //            //get a list of parameter types
        //            ParameterInfo[] parameterInfos = proxiedMethod.GetParameters();
        //            Type[] parameterTypes = new Type[parameterInfos.Length];
        //            for (int i = 0; i < parameterInfos.Length; i++)
        //                parameterTypes[i] = parameterInfos[i].ParameterType;

        //            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
        //                proxiedMethod.Name,
        //                MethodAttributes.Public | MethodAttributes.Virtual,
        //                proxiedMethod.CallingConvention,
        //                proxiedMethod.ReturnType,
        //                parameterTypes);


        //            ILGenerator il = methodBuilder.GetILGenerator();
        //            il.DeclareLocal(typeof(object[])); //define local parameter array

        //            #region ("Create Parameters Array in loc_0")
        //            il.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
        //            il.Emit(OpCodes.Newarr, typeof(object)); //Create new array
        //            il.Emit(OpCodes.Stloc_0);               //Store new array at loc_0

        //            for (int i = 0; i < parameterTypes.Length; i++)
        //            {
        //                il.Emit(OpCodes.Ldloc_0);           //load array
        //                il.Emit(OpCodes.Ldc_I4, i);         //load index
        //                il.Emit(OpCodes.Ldarg, i + 1);        //load argument
        //                if (parameterTypes[i].IsValueType)
        //                    il.Emit(OpCodes.Box, parameterTypes[i]); //box if needed
        //                il.Emit(OpCodes.Stelem_Ref);        //store arguemtn at index for array
        //            }
        //            #endregion

        //            il.Emit(OpCodes.Ldarg_0);                   //load this
        //            il.Emit(OpCodes.Ldfld, interceptorField);   //load this.Interceptor

        //            il.Emit(OpCodes.Ldarg_0);                   //load this
        //            il.Emit(OpCodes.Ldfld, proxiedObjectField); //load this.ProxiedObject

        //            il.Emit(OpCodes.Ldstr, proxiedMethod.Name); //load method name
        //            il.Emit(OpCodes.Ldloc_0);                    //load paramter array


        //            il.Emit(OpCodes.Ldnull); //il.Emit(OpCodes.Ldarg_0);//                   //load execute delegate
        //            il.Emit(OpCodes.Ldftn, executeMethod);
        //            il.Emit(OpCodes.Newobj, typeof(EmitProxyExecute).GetConstructors()[0]);

        //            il.EmitCall(OpCodes.Callvirt, typeof(EmitProxyExecute).GetMethod("Invoke"), null); //call interceptor delegate

        //            if (proxiedMethod.ReturnType == typeof(void))
        //                il.Emit(OpCodes.Pop);//if we are returning void get rid of the result on the stack
        //            else if (proxiedMethod.ReturnType.IsValueType)
        //                il.Emit(OpCodes.Unbox_Any, proxiedMethod.ReturnType); //unbox to make our return binderType the value binderType the interface demands
        //            il.Emit(OpCodes.Ret);                       //return


        //            return methodBuilder;



        //        }

        //        private static MethodBuilder CreatePassThroughMethod(TypeBuilder typeBuilder, MethodInfo targetMethod, FieldInfo proxiedObjectField)
        //        {
        //            //get a list of parameter types
        //            ParameterInfo[] parameterInfos = targetMethod.GetParameters();
        //            Type[] parameterTypes = new Type[parameterInfos.Length];
        //            for (int i = 0; i < parameterInfos.Length; i++)
        //                parameterTypes[i] = parameterInfos[i].ParameterType;

        //            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
        //                targetMethod.Name,
        //                MethodAttributes.Public | MethodAttributes.Virtual,
        //                targetMethod.CallingConvention,
        //                targetMethod.ReturnType,
        //                parameterTypes);

        //            ILGenerator il = methodBuilder.GetILGenerator();
        //            il.Emit(OpCodes.Ldarg_0); //load this
        //            il.Emit(OpCodes.Ldfld, proxiedObjectField); //load object
        //            for (int i = 0; i < parameterTypes.Length; i++)
        //            {
        //                il.Emit(OpCodes.Ldarg, i + 1);   //load all parameters
        //            }

        //            il.EmitCall(OpCodes.Callvirt, targetMethod, null); //call method
        //            il.Emit(OpCodes.Ret);                       //return

        //            return methodBuilder;

        //        }

        //        private static IEnumerable<CustomAttributeBuilder> CreateCustomAttributeBuilder(MemberInfo oldMember)
        //        {
        //            foreach (CustomAttributeData att in CustomAttributeData.GetCustomAttributes(oldMember))
        //            {
        //                List<object> namedFieldValues = new List<object>();
        //                List<FieldInfo> namedFields = new List<FieldInfo>();

        //                List<object> namedPropertyValues = new List<object>();
        //                List<PropertyInfo> namedProperties = new List<PropertyInfo>();

        //                List<object> constructorArguments = new List<object>();

        //                //populate the constructor arguments
        //                foreach (CustomAttributeTypedArgument cata in att.ConstructorArguments)
        //                    constructorArguments.Add(cata.Value);

        //                //populate hte field and property values
        //                foreach (CustomAttributeNamedArgument cana in att.NamedArguments)
        //                {
        //                    if (cana.MemberInfo is FieldInfo)
        //                    {
        //                        namedFields.Add((FieldInfo)cana.MemberInfo);
        //                        namedFieldValues.Add(cana.TypedValue.Value);
        //                    }
        //                    else if (cana.MemberInfo is PropertyInfo)
        //                    {
        //                        namedProperties.Add((PropertyInfo)cana.MemberInfo);
        //                        namedPropertyValues.Add(cana.TypedValue.Value);
        //                    }
        //                }
        //                yield return new CustomAttributeBuilder(
        //                    att.Constructor,
        //                    constructorArguments.ToArray(),
        //                    namedProperties.ToArray(),
        //                    namedPropertyValues.ToArray(),
        //                    namedFields.ToArray(),
        //                    namedFieldValues.ToArray());
        //            }
        //        }

        //    }

        //}
    }
}
#endif