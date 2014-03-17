using System.Reflection;
using System.Reflection.Emit;

namespace NLite.DynamicProxy
{
    interface IProxyMethodBuilder
    {
        void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder);
    }
}