using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace NLite.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicAssemblyManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static void SaveAssembly()
        {
#if !SILVERLIGHT
            lock (typeof(DynamicAssemblyManager))
            {
                assemblyBuilder.Save(assemblyName.Name + ".dll");
            }
#else
          throw new NotImplementedException("DynamicAssemblyManager.SaveAssembly");
#endif
        }

        private static AssemblyName assemblyName;
        private static AssemblyBuilder assemblyBuilder;
        internal static readonly ModuleBuilder moduleBuilder;
        internal static readonly Module Module;

        static DynamicAssemblyManager()
        {
#if !SILVERLIGHT
            assemblyName = new AssemblyName("NLiteDynamicAssembly");
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave
                );

            moduleBuilder = assemblyBuilder.DefineDynamicModule(
                assemblyName.Name,
                assemblyName.Name + ".dll",
                true);

            Module = assemblyBuilder.GetModules().FirstOrDefault();
           
#else
            assemblyName = new AssemblyName("EmitMapperAssembly.SL");
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                  assemblyName,
                  AssemblyBuilderAccess.Run
                  );
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);
#endif
        }

        private static string CorrectTypeName(string typeName)
        {
            if (typeName.Length >= 1042)
            {
                typeName = "type_" + typeName.Substring(0, 900) + Guid.NewGuid().ToString().Replace("-", "");
            }
            return typeName;
        }


        internal static TypeBuilder DefineType(string typeName, Type parent)
        {
            lock (typeof(DynamicAssemblyManager))
            {
                return moduleBuilder.DefineType(
                    CorrectTypeName(typeName),
                    TypeAttributes.Public,
                    parent,
                    null
                    );
            }
        }

    }
}
