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

namespace NLite.Interceptor.Matcher
{
    /// <summary>
    /// 
    /// </summary>
    public class JoinPointMatcher : IJoinPointMatcher
    {
        private IEnumerable<ICutPointInfo> _pointcuts;
        private static Dictionary<string, string> _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        static JoinPointMatcher()
        {
            _map["int"] = typeof(int).FullName;
            _map["string"] = typeof(string).FullName;
            _map["float"] = typeof(float).FullName;
            _map["double"] = typeof(double).FullName;
            _map["byte"] = typeof(byte).FullName;
            _map["long"] = typeof(long).FullName;
            _map["short"] = typeof(Int16).FullName;
            _map["int32"] = typeof(Int32).FullName;
            _map["int64"] = typeof(Int64).FullName;
            _map["single"] = typeof(Single).FullName;
            _map["single"] = typeof(Single).FullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointcuts"></param>
        public JoinPointMatcher(IEnumerable<ICutPointInfo> pointcuts)
        {
            _pointcuts = pointcuts.ToArray();
        }

        #region IJoinPointMatcher Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual IEnumerable<ICutPointInfo> Match(Type targetType, MethodInfo method)
        {

            foreach (var pointcut in _pointcuts)
            {
                var signature = pointcut.Signature;

                var deep = targetType.GetInheriteDeep(method.DeclaringType);
                if (deep < 0 || deep > signature.Deep)
                    continue;

                if (!FlagsMatchMethodType(method, signature))
                    continue;

                if (signature.AllMethods())
                    yield return pointcut;
                else
                {
                    if (!NameMatch(signature, method, signature.Flags) ||
                           !ReturnTypeMatch(signature, method) ||
                           !AccessMatch(signature, method) ||
                           !ArgumentsMatch(signature, method))
                        continue;

                    yield return pointcut;
                }
            }

        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="method"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        protected virtual bool NameMatch(IMethodSignature signature, MethodInfo method, CutPointFlags flags)
        {
            String sign = signature.Method;
            String name = method.Name;

            if (sign.IndexOf('*') != -1)
                return Regex.IsMatch(name, sign);

            if ((method.IsSpecialName && (((int)(flags & CutPointFlags.Property)) != 0)) ||
                (name.StartsWith("get_") && (((int)(flags & CutPointFlags.PropertyRead)) != 0)) ||
                (name.StartsWith("set_") && (((int)(flags & CutPointFlags.PropertyWrite)) != 0)))
                name = name.Substring(4);

            return name.Equals(sign);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected virtual bool ReturnTypeMatch(IMethodSignature signature, MethodInfo method)
        {
            if (signature.ReturnType == "*")
                return true;

            return TypeMatch(signature.ReturnType, method.ReturnType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected virtual bool AccessMatch(IMethodSignature signature, MethodInfo method)
        {
            if (signature.Flags == CutPointFlags.All)
                return true;

            return ((signature.Access & AccessFlags.Public) == AccessFlags.Public && method.IsPublic)
                || ((signature.Access & AccessFlags.Protected) == AccessFlags.Protected && method.IsFamilyOrAssembly);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected virtual bool ArgumentsMatch(IMethodSignature signature, MethodInfo method)
        {
            if (signature.AllArguments())
                return true;

            var arguments = signature.Arguments.ToArray();
            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < arguments.Length; i++)
            {
                String argName = arguments[i];

                if (argName.Equals("*"))
                    break;

                if (i == parameters.Length)
                    return false;
                if (i == arguments.Length - 1 && arguments.Length != parameters.Length)
                    return false;

                if (!TypeMatch(argName, parameters[i].ParameterType))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argSignature"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        protected virtual bool TypeMatch(String argSignature, Type parameterType)
        {
            argSignature = NormalizeTypeName(argSignature);
            String name = parameterType.FullName;

            if (argSignature.IndexOf('*') != -1)
                return Regex.IsMatch(name, argSignature);
            else
                return String.Compare(name, argSignature, true) == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual String NormalizeTypeName(String type)
        {
            return _map.ContainsKey(type) ? _map[type] : type;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="pointcut"></param>
        /// <returns></returns>
        protected virtual bool FlagsMatchMethodType(MethodInfo method, IMethodSignature pointcut)
        {
            if (!method.IsSpecialName && ((int)(pointcut.Flags & CutPointFlags.Method)) == 0)
                return false;

            if (method.IsSpecialName)
            {
                if (pointcut.Flags == CutPointFlags.Method)
                    return false;

                if (((int)(pointcut.Flags & CutPointFlags.Property)) == 0)
                {
                    bool isPropertyGet = method.Name.StartsWith("get");

                    if ((!isPropertyGet && ((int)(pointcut.Flags & CutPointFlags.PropertyRead)) != 0) ||
                        (isPropertyGet && ((int)(pointcut.Flags & CutPointFlags.PropertyWrite) != 0)))
                        return false;
                }
            }

            return true;
        }
    }
}
