using System.Reflection;
using System.Reflection.Emit;

namespace NLite.DynamicProxy
{
    interface IMethodBodyEmitter
    {
        void EmitMethodBody(ILGenerator IL, MethodInfo method,
                            FieldInfo invocationHandlerField);
    }
}