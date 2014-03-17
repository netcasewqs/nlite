using System.Reflection;
using System.Reflection.Emit;

namespace NLite.DynamicProxy
{
    interface IArgumentHandler
    {
        void PushArguments(ParameterInfo[] parameters, ILGenerator IL, bool isStatic);
    }
}